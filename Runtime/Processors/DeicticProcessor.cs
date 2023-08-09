using System;
using UnityEngine;

namespace ummi.Runtime.Processors {
  public abstract class DeicticProcessor : Processor {
    private void Update() {
      if (IsClicked()) OnClick(InputToRay());
    }

    private void OnClick(Ray ray) {
      if (Camera.main is null) return;
      int sequenceID = Time.frameCount;
      WriteFact(ray, sequenceID);
      if (Physics.Raycast(ray, out RaycastHit hit, 250)) {
        WriteFact(hit.collider.gameObject, sequenceID);
        WriteFact(hit.point, sequenceID);
      }
    }

    protected abstract bool IsClicked();
    protected abstract Ray InputToRay();
  }
}