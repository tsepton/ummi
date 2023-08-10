using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Ummi.Runtime.Parser;
using ummi.Runtime.Processors;
using UnityEngine;

namespace Ummi.Runtime {
  public class MeaningFrameFusionEngine : IFusionEngine {
    private readonly FactBase _factBase = FactBase.Instance;
    private List<Source> _usedSources = new();

    public Boolean Call(AttributeParser.RegisteredMMIMethod method) {
      return Call(method, DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(4)), TimeSpan.FromSeconds(4));
    }

    public bool Call(AttributeParser.RegisteredMMIMethod method, DateTime startedAt, TimeSpan duration) {
      if (method.GetNumberOfParameters() == 0) method.Invoke();
      else {
        Fact<object>[] factsToConsider = _factBase.GetFacts(startedAt, duration).ToArray();
        ParameterInfo[] parameters = method.Info.GetParameters();
        var potentialFacts = GetPotentialCompletionFacts(parameters, factsToConsider);
        if (potentialFacts.Count(x => x != null) == parameters.Length) {
          method.Invoke(potentialFacts
            .Select(x => x.Value)
            .ToArray()
          );
          foreach (Source source in potentialFacts.Select(x => x.Source)) {
            _usedSources.Add(source);
          }
        }
        else {
          Debug.Log($"Fusion Engine - Could not invoke method {method.Info.Name} due to no completion parameters");
          return false;
        }
      }

      Debug.Log($"Fusion Engine - Invoked method {method.Info.Name}");
      return true;
    }

    private Fact<object>[] GetPotentialCompletionFacts(ParameterInfo[] parameters, Fact<object>[] factsToConsider) {
      Dictionary<Type, Fact<object>[]> potentialArgsPerType = new();
      foreach (var type in parameters.Select(p => p.ParameterType).Distinct()) {
        var potentialFactsForType = factsToConsider.Where(x => x.Value.GetType() == type);
        potentialArgsPerType.Add(type, potentialFactsForType.ToArray());
      }

      List<Fact<object>> mostSuitableFacts = new();
      foreach (var param in parameters) {
        var fact = potentialArgsPerType[param.ParameterType]
          .DefaultIfEmpty(null)
          .FirstOrDefault(fact =>
            fact is not null &&
            // fact not already chosen has input parameter
            !mostSuitableFacts.Contains(fact) &&
            // if another linked fact has been used inside a previous command, then this fact is not valid (see issue #8)
            !_usedSources.Contains(fact.Source) &&
            // also true if a fact has been chose in a previous iteration (see issue #8)
            !mostSuitableFacts.Select(f => f.Source).Contains(fact.Source)
          );
        if (fact != null) mostSuitableFacts.Add(fact);
      }

      return mostSuitableFacts.ToArray();
    }
  }
}