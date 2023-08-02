using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Ummi.Runtime.Parser;
using UnityEngine;

namespace Ummi.Runtime {
  public class MeaningFrameFusionEngine : IFusionEngine {
    private readonly FactBase _factBase = FactBase.Instance;

    public MeaningFrameFusionEngine() { }

    public Boolean Call(AttributeParser.RegisteredMMIMethod method) {
      if (method.GetNumberOfParameters() == 0) method.Invoke();
      else {
        ParameterInfo[] parameters = method.Info.GetParameters();
        List<object> args = new List<object>();
        foreach (var param in parameters) {
          var facts = GetPotentialCompletionParameters(param);
          if (facts.Length == 0) break;
          args.Add(facts.First());
        }

        Debug.Log(args.Count);
        if (args.Count == parameters.Length) method.Invoke(args.ToArray());
        else return false;
      }

      return true;
    }

    /// <summary>
    /// Gets the potential facts to complete a method call based on <paramref name="param"/> type.
    /// </summary>
    /// <param name="param">The ParameterInfo you need to complete the MethodInfo call</param>
    /// <returns>An array of Facts ordered by their chronological order of appearance</returns>
    private Fact<object>[] GetPotentialCompletionParameters(ParameterInfo param) {
      var potentialCompletion = _factBase.GetFacts()
        .Where(fact => fact.Value.GetType() == param.ParameterType)
        .ToArray();
      return potentialCompletion;
    }
  }
}