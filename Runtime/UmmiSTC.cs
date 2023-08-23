using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ummi.Runtime;
using Ummi.Runtime.Exceptions;
using Ummi.Runtime.Parser;
using ummi.Runtime.Processors;
using Ummi.Runtime.Speech;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Whisper;
using Whisper.Utils;
using Debug = UnityEngine.Debug;

namespace Ummi.Runtime {
  public class UmmiSTC : MonoBehaviour {
    [Header("Whisper")] public WhisperManager whisper;
    public MicrophoneRecord microphoneRecord;
    public bool displayInputDebugger = true;
    public GameObject inputDebugger;

    [Header("SBert")] public string modelPath = Config.DefaultModelPath;
    public string vocabularyPath = Config.DefaultVocabularyPath;

    [Header("Command listener")]
    // public bool automatic = false; // TODO : FUTURE WORK
    public CommandTrigger trigger = CommandTrigger.RightMouseButton;

    public WhisperState WhisperState {
      get {
        if (_isProcessingCommand) return WhisperState.Processing;
        if (microphoneRecord.IsRecording) return WhisperState.Listening;
        return WhisperState.NotListening;
      }
    }

    public string SpeechToText => _speechToText;

    [Header("Multimodal Interfaces Registration")]
    public List<MMIInterface> interfaces = new();

    private ISemanticEngine _semanticEngine; // TODO Needs parameter
    private IFusionEngine _fusionEngine;
    private string _buffer;
    private string _speechToText;
    private DateTime _commandStartedTimestamp;
    private TimeSpan _lastCommandLength;
    private bool _isProcessingCommand = false;

    private void Awake() {
      _semanticEngine = (ISemanticEngine)Activator.CreateInstance(Config.SemanticEngine,
        Path.Combine(Application.streamingAssetsPath, modelPath),
        Path.Combine(Application.streamingAssetsPath, vocabularyPath)
      );
      _fusionEngine = (IFusionEngine)Activator.CreateInstance(Config.FusionEngine);

      whisper.OnNewSegment += OnNewSegment;
      microphoneRecord.OnRecordStop += OnRecordStop;
    }

    private void Start() {
      inputDebugger.SetActive(displayInputDebugger);
      interfaces.ForEach(i => _semanticEngine.Register(i.Interfaces.ToArray()));
    }

    private void Update() {
      // TODO automatic handling
      // TODO Other modality
      if (IsCommandTriggered() && !_isProcessingCommand) ToggleRecording();
    }

    private void ToggleRecording() {
      if (!microphoneRecord.IsRecording) {
        microphoneRecord.StartRecord();
        _commandStartedTimestamp = DateTime.UtcNow;
      }
      else {
        microphoneRecord.StopRecord();
        _lastCommandLength = DateTime.UtcNow - _commandStartedTimestamp;
      }
    }

    private void OnNewSegment(WhisperSegment segment) {
      _buffer += segment.Text;
    }

    private async void OnRecordStop(float[] data, int frequency, int channels, float length) {
      _isProcessingCommand = true;
      _speechToText = "";
      _buffer = "";

      var sw = new Stopwatch();
      sw.Start();
      var res = await whisper.GetTextAsync(data, frequency, channels);
      if (res == null) {
        _isProcessingCommand = false;
        return;
      }

      _speechToText = res.Result;

      var time = sw.ElapsedMilliseconds;
      var rate = length / (time * 0.001f);
      Debug.Log($"Time: {time} | Rate: {rate:F1}x");

      try {
        InferredMethod[] methods = _semanticEngine.Infer(res.Result);
        if (methods.Length != 0) {
          AttributeParser.RegisteredMMIMethod method = methods[0].Method;
          _fusionEngine.Call(method, _commandStartedTimestamp, _lastCommandLength);
        }
      }
      catch (NoCorpusException e) {
        Debug.LogError(e);
      }

      _isProcessingCommand = false;
    }

    private bool IsCommandTriggered() {
      switch (trigger) {
        case CommandTrigger.LeftMouseButton:
          return Input.GetMouseButtonDown(0);
        case CommandTrigger.RightMouseButton:
          return Input.GetMouseButtonDown(1);
        default:
          return false;
      }
    }
  }

  public enum CommandTrigger {
    LeftMouseButton,
    RightMouseButton,
  }

  public enum WhisperState {
    NotListening,
    Listening,
    Processing
  }
}