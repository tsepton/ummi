using System;
using System.Collections.Generic;
using System.Linq;
using ummi.Runtime.Processors;
using UnityEngine;

namespace Ummi.Runtime {
  public class FactBase {
    private static readonly Lazy<FactBase> Lazy = new Lazy<FactBase>(() => new FactBase());

    public static FactBase Instance => Lazy.Value;
    private FactBase() { }
    private LinkedList<Fact<object>> _facts = new();

    /// <summary>
    /// Create a Fact holding <paramref name="obj"/> as a value.
    /// </summary>
    /// <param name="obj">the object that will end up as the holding value of the fact</param>
    /// <param name="source"></param>
    public void Add(object obj, Source source) {
      _facts.AddLast(new Fact<object>(obj, source));
    }

    /// <summary>
    /// This should not be called. This is for testing reason mainly.
    /// </summary>
    public void Clear() {
      _facts.Clear();
    }

    public Fact<object>[] GetFacts(TimeSpan seconds) {
      return _facts.Where(f => f.Timestamp > DateTime.UtcNow.Subtract(seconds)).ToArray();
    }

    public Fact<object>[] GetFacts(DateTime from, TimeSpan duration) {
      return _facts.Where(f => f.Timestamp > from && f.Timestamp < from.Add(duration)).ToArray();
    }

    public Fact<object>[] GetFacts() {
      return _facts.ToArray();
    }
  }
}