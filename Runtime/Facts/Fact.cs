using System;
using ummi.Runtime.Processors;

namespace Ummi.Runtime {
  public class Fact<T> {
    public DateTime Timestamp { get; }

    public T Value { get; }

    public Source Source { get; }

    public Fact(T value, Source source) {
      Value = value;
      Timestamp = DateTime.UtcNow;
      Source = source;
    }

    public override string ToString() {
      return $"Fact happened at {Timestamp}: {Value}";
    }
  }
}