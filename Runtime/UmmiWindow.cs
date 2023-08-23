using System;
using System.Collections;
using System.Collections.Generic;
using Ummi.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace ummi {
  public class UmmiWindow : MonoBehaviour {
    public UmmiSTC ummi;
    public Sprite processingSprite;
    public Sprite listeningSprite;
    public Sprite notListeningSprite;
    public Image icon;
    public TMP_Text textMeshPro;
    public GameObject ui;


    private void Start() {
      textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void FixedUpdate() {
      if (ummi.WhisperState == WhisperState.Processing) {
        icon.sprite = processingSprite;
      }
      else if (ummi.WhisperState == WhisperState.Listening) {
        icon.sprite = listeningSprite;
      }
      else {
        icon.sprite = notListeningSprite;
      }

      textMeshPro.text = ummi.SpeechToText;
    }
  }
}