using System;
using System.Diagnostics;
using Ummi.Runtime.Parser;
using UnityEngine;
using UnityEngine.UIElements;
using Whisper;
using Whisper.Utils;
using Debug = UnityEngine.Debug;

namespace Ummi.Runtime {
  public class UmmiSTC : MonoBehaviour {
    public WhisperManager whisper;
    public MicrophoneRecord microphoneRecord;

    private ISemanticEngine _semanticEngine = Config.SemanticEngine;
    private IFusionEngine _fusionEngine = Config.FusionEngine;
    private string _buffer;

    private void Awake() {
      whisper.OnNewSegment += OnNewSegment;
      microphoneRecord.OnRecordStop += OnRecordStop;
    }
    
    public static class MMIMethodsMockup {
      
      [MultimodalInterface("Put that there")]
      public static void PutThatThere(GameObject go, Vector3 position) {
        go.transform.position = position;
      }
    }


    private void Start() {
      _semanticEngine.Register(typeof(MMIMethodsMockup));
    }

    private void Update() {
      if (Input.GetMouseButtonDown(0))
        ToggleRecording();
    }


    private void ToggleRecording() {
      if (!microphoneRecord.IsRecording) microphoneRecord.StartRecord();
      else microphoneRecord.StopRecord();
    }

    private void OnNewSegment(WhisperSegment segment) {
      _buffer += segment.Text;
    }

    private async void OnRecordStop(float[] data, int frequency, int channels, float length) {
      _buffer = "";

      var sw = new Stopwatch();
      sw.Start();

      var res = await whisper.GetTextAsync(data, frequency, channels);
      if (res == null)
        return;

      var time = sw.ElapsedMilliseconds;
      var rate = length / (time * 0.001f);
      Debug.Log($"Time: {time} | Rate: {rate:F1}x");

      AttributeParser.RegisteredMMIMethod method = _semanticEngine.Infer(res.Result);
      if (method != null) _fusionEngine.Call(method);
    }
  }
}