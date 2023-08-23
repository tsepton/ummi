using Ummi.Runtime.Parser;

namespace Ummi.Runtime {
  public class InferredMethod {

    public readonly double Threshold;
    public readonly AttributeParser.RegisteredMMIMethod Method;
    
    public InferredMethod(AttributeParser.RegisteredMMIMethod method, double threshold) {
      Method = method;
      Threshold = threshold;
    }
  }
}