using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ummi.Runtime.Speech;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = System.Object;

namespace Ummi.Runtime.Parser {
  public class AttributeParser {
    private readonly RegisteredMMIMethod[] _methods;

    public RegisteredMMIMethod[] Methods => _methods;
    public RegisteredMMIMethod[] AbstractMethods => _methods.Where(m => m.Info.IsAbstract).ToArray();
    public RegisteredMMIMethod[] StaticMethods => _methods.Where(m => m.Info.IsStatic).ToArray();
    public RegisteredMMIMethod[] ConcreteMethods => _methods.Where(m => !m.Info.IsAbstract).ToArray();

    public AttributeParser(Type[] classes, ModelOrganizer model) {
      var extractedMethods = TypeCache
        .GetMethodsWithAttribute<MultimodalInterface>()
        .Where(method => classes.Contains(method.DeclaringType));

      List<RegisteredMMIMethod> methods = new List<RegisteredMMIMethod>();
      var extracted = extractedMethods
        .Where(m => !m.IsPrivate)
        .Where(m => m.ReturnType == typeof(void))
        .Where(m => !m.IsAbstract)
        .Where(m => m.IsStatic); // Future work
      foreach (var method in extracted) {
        string[] utters = method.GetCustomAttribute<MultimodalInterface>().Utterances;
        methods.Add(new RegisteredMMIMethod(method, model.Predict(utters), utters));
      }

      _methods = methods.ToArray();
    }

    public class RegisteredMMIMethod {
      public MethodInfo Info { get; }
      public double[][] Embeddings { get; }
      public string[] Utters { get; }
      
      public RegisteredMMIMethod(MethodInfo method, double[][] embeddings, string[] utters) {
        Info = method;
        Embeddings = embeddings;
        Utters = utters;
      }

      public int GetNumberOfParameters() {
        return Info.GetParameters().Length;
      }

      public void Invoke() {
        Invoke(new object[] {});
      }
      
      public void Invoke(Object[] args) {
        Assert.AreEqual(args.Length, GetNumberOfParameters());
        Info.Invoke(null, args);
      }

      public override string ToString() {
        return $"Registered MMI Method called {Info.Name}";
      }
    }
  }
}