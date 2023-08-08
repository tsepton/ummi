using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ummi.Runtime;
using Ummi.Runtime.Parser;
using ummi.Runtime.Processors;
using Ummi.Runtime.Speech;
using UnityEngine;
using UnityEngine.Serialization;
using Whisper;
using Whisper.Utils;
using Debug = UnityEngine.Debug;

namespace Ummi.Runtime {
  public class UmmiSTC : MonoBehaviour {
    [Header("Whisper")] public WhisperManager whisper;
    public MicrophoneRecord microphoneRecord;

    [Header("SBert")] public string modelPath = Config.DefaultModelPath;
    public string vocabularyPath = Config.DefaultVocabularyPath;

    [FormerlySerializedAs("Interfaces")] [Header("Multimodal Interfaces Registration")]
    public List<MMIInterface> interfaces = new();
    
    private ISemanticEngine _semanticEngine; // TODO Needs parameter
    private IFusionEngine _fusionEngine;
    private string _buffer;

    private void Awake() {
      _semanticEngine = (ISemanticEngine)Activator.CreateInstance(Config.SemanticEngine,
        Path.Combine(Application.streamingAssetsPath, modelPath),
        Path.Combine(Application.streamingAssetsPath, vocabularyPath)
      );
      _fusionEngine = (IFusionEngine)Activator.CreateInstance(Config.FusionEngine);

      // whisper.speedUp = true; 
      whisper.OnNewSegment += OnNewSegment;
      microphoneRecord.OnRecordStop += OnRecordStop;
    }

    private void Start() {
      interfaces.ForEach(i => _semanticEngine.Register(i.Interfaces.ToArray()));
    }

    private void Update() {
      if (Input.GetMouseButtonDown(1)) ToggleRecording();
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

      // var sw = new Stopwatch();
      // sw.Start();
      var res = await whisper.GetTextAsync(data, frequency, channels);
      if (res == null)
        return;
      // var time = sw.ElapsedMilliseconds;
      // var rate = length / (time * 0.001f);
      // Debug.Log($"Time: {time} | Rate: {rate:F1}x");

      AttributeParser.RegisteredMMIMethod method = _semanticEngine.Infer(res.Result);
      if (method != null) _fusionEngine.Call(method);
    }
  }
}