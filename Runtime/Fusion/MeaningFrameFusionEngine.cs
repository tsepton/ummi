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
        var potentialParameters = GetPotentialCompletionParameters(parameters);
        if (potentialParameters.Count(x => x != null) == parameters.Length) method.Invoke(potentialParameters);
        else {
          Debug.Log($"Fusion Engine - Could not invoke method {method.Info.Name} due to no completion parameters");
          return false;
        }
      }

      Debug.Log($"Fusion Engine - Invoked method {method.Info.Name}");
      return true;
    }

    private object[] GetPotentialCompletionParameters(ParameterInfo param) {
      return _factBase.GetFacts()
        .Where(fact => fact.Value.GetType() == param.ParameterType)
        .Select(e => e.Value)
        .Reverse()
        .ToArray();
    }

    private object[] GetPotentialCompletionParameters(ParameterInfo[] parameters) {
      Fact<object>[] facts = _factBase.GetFacts(TimeSpan.FromSeconds(4)).ToArray();

      Dictionary<Type, Fact<object>[]> potentialArgsPerType = new();
      foreach (var type in parameters.Select(p => p.ParameterType).Distinct()) {
        var potentialFactsForType = facts.Where(x => x.Value.GetType() == type);
        potentialArgsPerType.Add(type, potentialFactsForType.ToArray());
      }

      List<Fact<object>> mostSuitableFacts = new();
      foreach (var param in parameters) {
        var fact = potentialArgsPerType[param.ParameterType].DefaultIfEmpty(null)
          .FirstOrDefault(x => !mostSuitableFacts.Contains(x));
        if (fact != null) mostSuitableFacts.Add(fact);
      }

      return mostSuitableFacts.Select(x => x.Value).ToArray();
    }
  }
}