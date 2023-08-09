using UnityEngine;

namespace ummi.Runtime.Processors {
  public abstract class DeicticProcessor : Processor {
    private void Update() {
      if (IsClicked()) OnClick(InputToRay());
    }

    private void OnClick(Ray ray) {
      if (Camera.main is null) return;
      WriteFact(ray);
      if (Physics.Raycast(ray, out RaycastHit hit, 250)) {
        WriteFact(hit.collider.gameObject);
        WriteFact(hit.point);
      }
    }

    protected abstract bool IsClicked();
    protected abstract Ray InputToRay();
  }
}