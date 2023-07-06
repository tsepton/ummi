using System;
using System.Linq;
using System.Reflection;
using Ummi.Runtime.Parser;

namespace Ummi.Runtime {
  public class FusionEngine {
    // Singleton
    private static readonly Lazy<FusionEngine> Lazy = new Lazy<FusionEngine>(() => new FusionEngine());
    public static FusionEngine Instance => Lazy.Value;
    private FusionEngine() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    public void Complete(AttributeParser.RegisteredMMIMethod method) {
      ParameterInfo[] parameters = method.Info.GetParameters();
      foreach (var param in parameters) {
        var facts = GetPotentialCompletionParameters(param);
      }
    }

    /// <summary>
    /// Gets the potential facts to complete a method call based on <paramref name="param"/> type.
    /// </summary>
    /// <param name="param">The ParameterInfo you need to complete the MethodInfo call</param>
    /// <returns>An array of Facts ordered by their chronological order of appearance</returns>
    private Fact[] GetPotentialCompletionParameters(ParameterInfo param) {
      var potentialCompletion = FactBase.Instance.GetFacts()
        .Where(fact => fact.Target.GetType() == param.ParameterType)
        .ToArray();
      return potentialCompletion;
    }
  }
}