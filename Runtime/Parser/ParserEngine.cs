using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ummi.Runtime.Speech;
using UnityEditor;
using UnityEngine;

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
        .Where(method => classes.Contains(method.DeclaringType)) ;

      List<RegisteredMMIMethod> methods = new List<RegisteredMMIMethod>();
      foreach (var (method, i) in extractedMethods.Select((value, i) => (value, i))) {
        if (method.IsPrivate) {
          Debug.LogWarning($"'{method.Name}' is a private method, it will not be registered.");
          continue;
        }

        if (method.ReturnType != typeof(void)) {
          Debug.LogWarning($"'{method.Name}' returns a value, it will not be registered.");
          continue;
        }

        // TODO - future works
        if (method.IsAbstract) {
          Debug.LogWarning($"'{method.Name}' is an abstract method, it will not be registered.");
          continue;
        }

        // TODO - future works
        if (!method.IsStatic) {
          Debug.LogWarning($"'{method.Name}' is not a static method, it will not be registered.");
          continue;
        }

        string[] utters = method.GetCustomAttribute<MultimodalInterface>().Utterances;
        methods.Add(new RegisteredMMIMethod(method, model.Predict(utters), utters));
      }

      _methods = methods.ToArray();
    }

    public class RegisteredMMIMethod {
      // TODO - Action are parameterless methods
      private readonly Action _action;

      public MethodInfo Info => _action.Method;
      public double[][] Embeddings { get; }
      public string[] Utters { get; }


      public RegisteredMMIMethod(MethodInfo method, double[][] embeddings, string[] utters) {
        _action = (Action)Delegate.CreateDelegate(typeof(Action), method);
        Embeddings = embeddings;
        Utters = utters;
      }

      public void Invoke() {
        if (Info.IsStatic) _action.Invoke();
        else Debug.LogWarning("Non static methods are not yet supported");
      }

      public override string ToString() {
        return $"Registered MMI Method called {Info.Name}";
      }
    }
  }
}