using System;
using UnityEngine;

//namespace ummi.Packages. {

namespace Ummi.Runtime.Parser {
  [AttributeUsage(validOn: AttributeTargets.Method)]
  public class MultimodalInterface : PropertyAttribute {
    public string[] Utterances { get; }

    public MultimodalInterface(string utterance) {
      Utterances = new[] { utterance };
    }

    public MultimodalInterface(string[] utterances) {
      Utterances = utterances;
    }
  }
}