using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Ummi.Runtime;
using UnityEngine;

namespace ummi.Runtime.Processors {
  /// <summary>
  /// A processor is a MonoBehavior which process a modality (or a set of modalities)
  /// and infer facts from this modality behavior.
  /// </summary>
  public abstract class Processor : MonoBehaviour {
    public abstract ProcessorID ProcessorID { get; }

    /// <summary>
    /// Writes an object to the factbase so it becomes visible to the fusion engine.
    /// </summary>
    /// <param name="obj">An array of value that will be hold representing something</param>
    protected void WriteFact(object obj, int eventID) {
      FactBase.Instance.Add(obj, new Source(ProcessorID, eventID));
    }
  }

  public enum ProcessorID {
    // VOICE
    Voice = 0,

    // GESTURE
    // Deictic = 1,
    DeicticMouse = 10,
    DeicticController = 11,
    DeicticGaze = 12,
    // Mimetic = 1,
  }

  public struct Source {
    public ProcessorID ProcessorID;
    public int EventID; // Events are not linked between processors

    public Source(ProcessorID processorID, int eventID) {
      ProcessorID = processorID;
      EventID = eventID;
    }

    public static bool operator ==(Source s1, Source s2) {
      return s1.ProcessorID == s2.ProcessorID && s1.EventID == s2.EventID;
    }

    public static bool operator !=(Source s1, Source s2) {
      return s1.ProcessorID != s2.ProcessorID || s1.EventID != s2.EventID;
    }
  }
}