using System;
using UnityEngine;

namespace Ummi.Runtime {
  public class UmmiEngine: MonoBehaviour {

    private ISemanticEngine _semanticEngine = Config.SemanticEngine;
    private IFusionEngine _fusionEngine = Config.FusionEngine;
    
    private void Start() {
      
    }
    
    
  }
}