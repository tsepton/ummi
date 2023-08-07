using System.Collections;
using System.Collections.Generic;
using Ummi.Runtime;
using UnityEngine;

namespace ummi.Runtime.Processors {
  /// <summary>
  /// A processor is a MonoBehavior which process a modality (or a set of modalities)
  /// and infer facts from this modality behavior.
  /// </summary>
  public abstract class Processor: MonoBehaviour {

    /// <summary>
    /// Writes an object to the factbase so it becomes visible to the fusion engine.
    /// </summary>
    /// <param name="obj">Any object or struct representing something</param>
    void WriteFact(object obj) {
      FactBase.Instance.Add(obj);
    }
    
  }
}