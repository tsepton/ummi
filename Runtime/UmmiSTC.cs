using System;
using System.Diagnostics;
using System.IO;
using Ummi.Runtime.Parser;
using Ummi.Runtime.Speech;
using UnityEngine;
using Whisper;
using Whisper.Utils;
using Debug = UnityEngine.Debug;

namespace Ummi.Runtime {
  public class UmmiSTC : MonoBehaviour {
    [Header("Whisper")] public WhisperManager whisper;
    public MicrophoneRecord microphoneRecord;

    [Header("SBert")] public string modelPath = Config.DefaultModelPath;
    public string vocabularyPath = Config.DefaultVocabularyPath;

    private ISemanticEngine _semanticEngine; // TODO Needs parameter
    private IFusionEngine _fusionEngine;
    private string _buffer;

    private void Awake() {
      _semanticEngine = (ISemanticEngine)Activator.CreateInstance(Config.SemanticEngine, new ModelPaths(
        Path.Combine(Application.streamingAssetsPath, vocabularyPath),
        Path.Combine(Application.streamingAssetsPath, modelPath)
      ));
      _fusionEngine = (IFusionEngine)Activator.CreateInstance(Config.FusionEngine);

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