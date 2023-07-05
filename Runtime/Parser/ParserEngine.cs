using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ummi.Runtime.Speech;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Ummi.Runtime.Parser {
  public class AttributeParser {
    private readonly RegisteredMMIMethod[] _methods;

    public RegisteredMMIMethod[] Methods => _methods;
    public RegisteredMMIMethod[] AbstractMethods => _methods.Where(m => m.Info.IsAbstract).ToArray();
    public RegisteredMMIMethod[] StaticMethods => _methods.Where(m => m.Info.IsStatic).ToArray();
    public RegisteredMMIMethod[] ConcreteMethods => _methods.Where(m => !m.Info.IsAbstract).ToArray();

    public AttributeParser(Type[] classes, IModelOrganizer model) {
      var extractedMethods = TypeCache
        .GetMethodsWithAttribute<MultimodalInterface>()
        .Where(method => classes.Contains(method.DeclaringType));

      List<RegisteredMMIMethod> methods = new List<RegisteredMMIMethod>();
      var x = extractedMethods
        .Where(m => !m.IsPrivate)
        .Where(m => m.ReturnType == typeof(void))
        .Where(m => !m.IsAbstract)
        .Where(m => m.IsStatic); // Future work
      foreach (var method in x) {
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

      public void Invoke() {
        Invoke(new object[] {});
      }
      
      public void Invoke(Object[] args) {
        Info.Invoke(null, args);
      }

      public override string ToString() {
        return $"Registered MMI Method called {Info.Name}";
      }
    }
  }
}