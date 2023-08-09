using System;
using System.Collections;
using Ummi.Runtime;
using ummi.Runtime.Processors;
using UnityEngine;

namespace ummi.Runtime.Processors {
  /// <summary>
  /// This processor allows the mouse to be used as a pointing input (deictic gesture) to select items in the scene. 
  /// 
  /// When the user clicks using the left button, this processor will add to the factbase:
  ///   - a Ray (from the Camera to the mouse),
  ///   - a GameObject if a Collider have been hit by the ray, 
  ///   - a Vector3 if a Collider have been hit by the ray (indicating the precise point of intersection with it). 
  /// </summary>
  public class MouseDeicticProcessor : DeicticProcessor {
    public override ProcessorID ProcessorID { get; } = ProcessorID.DeicticMouse;
    public MouseButton buttonToUse = MouseButton.LeftClick;

    protected override bool IsClicked() {
      return Input.GetMouseButtonDown((int)buttonToUse);
    }

    protected override Ray InputToRay() {
      return Camera.main!.ScreenPointToRay(Input.mousePosition);
    }
    
  }

  public enum MouseButton {
    LeftClick = 0,
    RightClick = 1
  }
}