using System;
using Ummi.Runtime.Parser;

namespace Ummi.Runtime {
  public interface ISemanticEngine {
    /// <summary>
    /// Register the methods marked inside the different <paramref name="classes"/>
    /// </summary>
    /// <param name="classes">Type of the classes that have MMI methods to be registered</param>
    public void Register(Type[] classes);

    public void Register(Type class_);

    /// <summary>
    /// Returns the most suitable MMI registered method to call based on the CosSim between its MMI string
    /// description and `text` param.
    /// If multiple methods are found, the one with the highest CosSim score is preferred.
    /// If no method are suitable, null is returned.
    /// </summary>
    /// <param name="text">The text to find the most suitable MMI registered method.</param>
    /// <param name="threshold">
    ///   A value between -1 and 1 which defines what CosSim value is considered good enough for the most suitable
    ///   method to be called.
    /// </param>
    public InferredMethod[] Infer(string text, float threshold = 0.65f);
  }
}