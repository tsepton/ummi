using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Ummi.Runtime.Parser;
using Ummi.Runtime.Speech;
using Ummi.Runtime.Speech.SBert;
using Ummi.Runtime;
using UnityEngine;

namespace Ummi.Tests {
  public class TestSpeech {
    private string[] _corpus = new[] {
      "Order this item",
      "Place the yellow coat inside my cart",
      "Rent a hotel room"
    };

    private string[] _utterances = new[] {
      "Order this item"
    };

    //   long[] input = new[] { 1L, 0L, 1L, 1L };
    //   long[][] expectedOutput = new[] {
    //     new[] { 1L, 1L, 1L, 1L },
    //     new[] { 0L, 0L, 0L, 0L },
    //     new[] { 1L, 1L, 1L, 1L },
    //     new[] { 1L, 1L, 1L, 1L }
    //   };

    [Test]
    public void TestModelTrainingNotNull() {
      IModelOrganizer model = Config.Organizer();
      Assert.IsNotNull(model);
    }

    [Test]
    public void TestSBertUncasedOutputs() {
      SBert sbert = new SBert();
      double[] tCased = sbert.Predict("Click this button");
      double[] tUncased = sbert.Predict("Click tHis buTTon");
      Assert.AreEqual(tCased, tUncased);
    }

    [Test]
    public void TestSBertOverall() {
      SBert sbert = new SBert();

      double[][] corpusOutputs = _corpus.Select(c => sbert.Predict(c)).ToArray();
      double[][] utterancesOutputs = _utterances.Select(utter => sbert.Predict(utter)).ToArray();

      foreach (var (i, c) in corpusOutputs.Select((value, i) => (i, value))) {
        foreach (var (j, u) in utterancesOutputs.Select((value, i) => (i, value))) {
          Debug.Log($"{i} - {j}: {c.CosSim(u)}");
        }
      }

      // FIXME: how can we test this?
      // Assert.AreEqual(expectedOutput, output);
    }
  }

  public class TestParser {
    abstract class MmiApiRegistrationExample {
      public const string ID = "TestParser methods";

      [MultimodalInterface(ID)]
      public static void PublicStaticMethod() { }

      [MultimodalInterface(ID)]
      public void PublicConcreteAndNotStaticMethod() { }

      // Should not be registered
      [MultimodalInterface(ID)]
      public int MethodWithReturnType() {
        return 42;
      }

      // Should not be registered - but its implementation should (Future wrk)
      [MultimodalInterface(ID)]
      public abstract void AbstractMethod();

      // Should not be registered
      [MultimodalInterface(ID)]
      private static void PrivateMethod() { }
    }

    [Test]
    public void TestConcreteMethodRegistration() {
      IModelOrganizer organizer = Config.Organizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      Assert.AreEqual(1, attributeParser.ConcreteMethods // Note: static member is also concrete here
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestAbstractMethodRegistration() {
      IModelOrganizer organizer = Config.Organizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      Assert.AreEqual(0, attributeParser.AbstractMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestStaticMethodRegistration() {
      IModelOrganizer organizer = Config.Organizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      Assert.AreEqual(1, attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestPrivateMethodRegistration() {
      IModelOrganizer organizer = Config.Organizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      Assert.AreEqual(0, attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .Where(m => m.Info.IsPrivate)
        .ToArray().Length);
    }

    [Test]
    public void TestStaticMethodInvoke() {
      IModelOrganizer organizer = Config.Organizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      var method = attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray();
      Assert.AreEqual(1, method.Length);
      Assert.DoesNotThrow(method[0].Invoke);
      // Well, I guess if it did not raise an exception, everything's fine then
    }
  }

  public class TestSemanticEngine {
    abstract class MmiApiRegistrationExample {
      [MultimodalInterface("Order this item")]
      public static void OrderThisItem() {
        Debug.Log("OrderThisItem has done its side effect magic");
      }

      [MultimodalInterface("Paint my car in a color")]
      public static void PaintThisCar() { }
    }

    [Test]
    public void TestInferFindTheMostLogicalMethod() {
      SemanticEngine se = new SemanticEngine();
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      AttributeParser.RegisteredMMIMethod method = se.Infer("Buy this stuff", threshold: 0.65f);
      if (method != null) Assert.AreEqual("OrderThisItem", method.Info.Name);
      else throw new NullReferenceException("No matching MMI registered method was found");
    }

    [Test]
    public void TestInferThresholdWorks() {
      SemanticEngine se = new SemanticEngine();
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      Assert.AreEqual(null, se.Infer("Buy this stuff", threshold: 1f));
      AttributeParser.RegisteredMMIMethod method = se.Infer("Order this item", threshold: 1f);
      if (method != null) Assert.AreEqual("OrderThisItem", method.Info.Name);
      else throw new NullReferenceException("No matching MMI registered method was found");
    }

    [Test]
    public void ComputeCosineSimilarity_ValidInput_ReturnsExpectedSimilarity() {
      double[] vector1 = { 1.0, 2.0, 3.0 };
      double[] vector2 = { 4.0, 5.0, 6.0 };
      double expectedSimilarity = 0.9746318461970762;
      double actualSimilarity = vector1.CosSim(vector2);
      Assert.AreEqual(expectedSimilarity, actualSimilarity, 1e-10);
    }

    [Test]
    public void ComputeCosineSimilarity_ZeroVectors_ThrowsArgumentException() {
      double[] vector1 = { 0.0, 0.0, 0.0 };
      double[] vector2 = { 0.0, 0.0, 0.0 };
      Assert.Throws<ArgumentException>(() => vector1.CosSim(vector2));
    }

    [Test]
    public void ComputeCosineSimilarity_DifferentVectorLength_ThrowsArgumentException() {
      double[] vector1 = { 1.0, 2.0, 3.0 };
      double[] vector2 = { 4.0, 5.0, 6.0, 7.0 };
      Assert.Throws<ArgumentException>(() => vector1.CosSim(vector2));
    }

    [Test]
    public void ComputeCosineSimilarity_OrthogonalVectors_ReturnsSimilarityZero() {
      double[] vector1 = { 1.0, 0.0 };
      double[] vector2 = { 0.0, 1.0 };
      double actualSimilarity = vector1.CosSim(vector2);
      Assert.AreEqual(0.0, actualSimilarity, 1e-10);
    }

    [Test]
    public void ComputeCosineSimilarity_OppositeDirectionVectors_ReturnsSimilarityMinusOne() {
      double[] vector1 = { 1.0, 2.0, 3.0 };
      double[] vector2 = { -1.0, -2.0, -3.0 };
      double actualSimilarity = vector1.CosSim(vector2);
      Assert.AreEqual(-1.0, actualSimilarity, 1e-10);
    }
  }
}