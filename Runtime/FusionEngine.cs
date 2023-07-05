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

    public void Complete(AttributeParser.RegisteredMMIMethod method) {
      ParameterInfo[] parameters = method.Info.GetParameters();
      foreach (var param in parameters) {
        var potentialCompletion = FactBase.Instance.GetFacts()
          .Where(fact => fact.Target.GetType() == param.ParameterType);
          
      }
    }
  }
}