using System;
using UnityEngine;

//namespace ummi.Packages. {

namespace Ummi.Runtime.Parser {
  [AttributeUsage(validOn: AttributeTargets.Method)]
  public class UserAction : PropertyAttribute {
    public string[] Utterances { get; }

    public UserAction(string utterance) {
      Utterances = new[] { utterance };
    }

    public UserAction(string[] utterances) {
      Utterances = utterances;
    }
  }
}