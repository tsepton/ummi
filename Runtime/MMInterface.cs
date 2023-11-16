using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ummi.Runtime {
  public abstract class MMInterface : MonoBehaviour {
    public List<Type> Interfaces = new();
    
    public abstract void Start();
  }
}