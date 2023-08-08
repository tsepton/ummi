using System;
using System.Collections;
using Ummi.Runtime;
using ummi.Runtime.Processors;
using UnityEngine;

namespace ummi.Runtime.Processors {
  /// <summary>
  /// Process the mouse input modality and interprets it as a raycast.
  /// 
  /// When the user clicks using the left button, this processor will add a Ray to the factbase and will also add the
  /// GameObject (if a Collider is attached) that may have been hit by the ray to the factbase. 
  /// </summary>
  public class MouseProcessor : Processor {
    private void Update() {
      if (Input.GetMouseButtonDown(0)) OnLeftClick();
    }

    private Ray MouseToRay() {
      return Camera.main!.ScreenPointToRay(Input.mousePosition);
    }

    private void OnLeftClick() {
      if (Camera.main is null) return;
      Ray ray = MouseToRay();
      WriteFact(ray);
      if (Physics.Raycast(ray, out RaycastHit hit, 250)) {
        WriteFact(hit.collider.gameObject);
        WriteFact(hit.point);
      }
    }
  }
}