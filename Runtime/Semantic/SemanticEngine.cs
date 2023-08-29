using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Ummi.Runtime.Exceptions;
using Ummi.Runtime.Parser;
using Ummi.Runtime.Speech;
using Ummi.Runtime.Speech.SBert;
using UnityEngine;

namespace Ummi.Runtime {
  public class SemanticEngine: ISemanticEngine {
    
    private readonly ModelOrganizer _organizer;

    public SemanticEngine(string modelPath,string vocabularyPath) {
      _organizer = (ModelOrganizer)Activator.CreateInstance(Config.ModelOrganizer, new ModelPaths(vocabularyPath, modelPath));
    }

    private AttributeParser.RegisteredMMIMethod[] _corpus;

    /// <summary>
    /// Register the methods marked inside the different <paramref name="classes"/>
    /// </summary>
    /// <param name="classes">Type of the classes that have MMI methods to be registered</param>
    public void Register(Type[] classes) {
      AttributeParser attributeParser = new AttributeParser(classes, _organizer);
      _corpus = attributeParser.Methods;
    }
    
    public void Register(Type class_) {
      AttributeParser attributeParser = new AttributeParser(new []{ class_ }, _organizer);
      _corpus = attributeParser.Methods;
    }

    /// <summary>
    /// Returns the most suitable MMI registered method to call based on the CosSim between its MMI string
    /// description and `text` param.
    /// If multiple methods are found, the one with the highest CosSim score is preferred.
    /// If no method are suitable, null is returned.
    /// </summary>
    /// <param name="text">The text to find the most suitable MMI registered method.</param>
    /// <param name="threshold">
    ///   A value between -1 and 1 which defines what CosSim value is considered good enough for the most suitable
    ///   method to be called.
    /// </param>
    public InferredMethod[] Infer(string text, float threshold = 0.65f) {
      if (_corpus == null) throw new NoCorpusException();
      var similarities = _corpus
        .Select(method => (method, scores: method.Embeddings.Select(emb => emb.CosSim(_organizer.Predict(text)))))
        .Where(item => item.scores.Any(score => score >= threshold))
        .OrderBy(item => item.scores.Max())
        .Reverse()
        .Select(item => new InferredMethod(item.method, item.scores.Max()))
        .ToArray();
      Debug.Log($"Found {similarities.Length} method(s), with a minimum threshold of {threshold}");
      return similarities;
    }
  }

  public static class ExtensionMethods {
    /// <summary>
    /// Gives the Cosine Similarity between vector A and vector B
    /// https://en.wikipedia.org/wiki/Cosine_similarity
    /// </summary>
    /// <param name="a">double array with a length of n</param>
    /// <param name="b">double array with a length of n</param>
    /// <returns>The Cosine Similarity between vector a and vector b - A value between -1 and 1</returns>
    /// <exception cref="Exception"></exception>
    public static double CosSim(this double[] a, double[] b) {
      if (a.Length != b.Length) throw new ArgumentException("Vectors a and b don't have the same length");

      double dotProduct = 0f;
      double aMagnitude = 0f;
      double bMagnitude = 0f;
      for (var i = 0; i < a.Length; i++) {
        dotProduct += a[i] * b[i];
        aMagnitude += Math.Pow(a[i], 2);
        bMagnitude += Math.Pow(b[i], 2);
      }

      if (aMagnitude == 0 || bMagnitude == 0) throw new ArgumentException("One or both vectors have zero length.");

      return dotProduct / (Math.Sqrt(aMagnitude) * Math.Sqrt(bMagnitude));
    }
  }
}