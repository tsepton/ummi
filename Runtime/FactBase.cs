using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ummi.Runtime {
  public class FactBase {
    private static readonly Lazy<FactBase> Lazy = new Lazy<FactBase>(() => new FactBase());

    public static FactBase Instance => Lazy.Value;

    private FactBase() { }

    private LinkedList<Fact<object>> _facts = new();

    public void Add(Fact<object> fact) {
      _facts.AddLast(fact);
    }
    
    public Fact<object>[] GetFacts(TimeSpan seconds) {
      return _facts.Where(f => f.Timestamp > DateTime.UtcNow.Subtract(seconds)).ToArray();
    }

    public Fact<object>[] GetFacts() {
      return _facts.ToArray();
    }
  }

  public class Fact<T> {
    public DateTime Timestamp { get; }

    public T Value { get; }

    public Fact(T value) {
      Value = value;
      Timestamp = DateTime.UtcNow;
    }

    public override string ToString() {
      return $"Fact happened at {Timestamp}: {Value}";
    }
  }
}