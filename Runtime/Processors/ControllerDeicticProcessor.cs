using System;
using System.Collections.Generic;
using ummi;
using ummi.Runtime.Processors;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.XR;

namespace Ummi.Runtime.Processors {
  /// <summary>
  /// This processor allows controllers to be used as a pointing input (deictic gesture) to select items in the scene. 
  /// 
  /// When the user clicks using the left button, this processor will add to the factbase:
  ///   - a Ray (from the Camera to the mouse),
  ///   - a GameObject if a Collider have been hit by the ray, 
  ///   - a Vector3 if a Collider have been hit by the ray (indicating the precise point of intersection with it). 
  /// </summary>
  public class ControllerDeicticProcessor : DeicticProcessor {
    public Handedness controllerToUse = Handedness.Right;
    private InputDevice _controller;

    private void Awake() {
      _controller = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
      if (!_controller.isValid) {
        Debug.LogError("Controller not valid.");
        gameObject.SetActive(false);
      }
    }

    private void Start() {
      // TODO - just for debugging
      _controller.SendHapticImpulse(0, 0.5f);
    }

    protected override bool IsClicked() {
      return _controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue) && triggerValue;
    }

    protected override Ray InputToRay() {
      return Camera.main!.ScreenPointToRay(Input.mousePosition);
    }
  }

  public enum Handedness {
    Left = XRNode.LeftHand,
    Right = XRNode.RightHand
  }
}