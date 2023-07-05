using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ummi.Runtime {
  public class FactBase {

    private LinkedList<Fact> _facts = new LinkedList<Fact>();

    public void Add(Fact fact) {
      _facts.AddLast(fact);
    }

    public Fact[] GetFacts(TimeSpan fromXSecondsAgo) {
      return _facts.Where(f => f.Timestamp > DateTime.UtcNow.Subtract(fromXSecondsAgo)).ToArray();
    }

    public Fact[] GetFacts() {
      return _facts.ToArray();
    }

  }

  public class Fact {
    
    private DateTime _timestamp;
    private GameObject _target;

    public DateTime Timestamp => _timestamp;
    public GameObject Target => _target;

    private Fact(GameObject go, DateTime timestamp) {
      _target = go;
      _timestamp = timestamp;
    }

    public static Fact FromGameObject(GameObject go) {
      return new Fact(go, DateTime.UtcNow);
    } 
    
  }
}