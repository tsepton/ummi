using System;
using Ummi.Runtime.Parser;

namespace Ummi.Runtime {
  public class FusionEngine {
    // Singleton
    private static readonly Lazy<FusionEngine> Lazy = new Lazy<FusionEngine>(() => new FusionEngine());
    public static FusionEngine Instance => Lazy.Value;
    private FusionEngine() { }

    public void Complete(AttributeParser.RegisteredMMIMethod method) {
      
    }
  }
}