using System;
using Ummi.Runtime.Speech;
using Ummi.Runtime.Speech.SBert;
using Ummi.Runtime.Speech.SBertAPI;

namespace Ummi.Runtime {
  public static class Config {

    // Path is relative to the StreamingAssets folder
    public static string DefaultModelPath = "all-MiniLM-L6-v2/model.onnx";
    public static string DefaultVocabularyPath= "all-MiniLM-L6-v2/vocab.txt";
    
    public static Type ModelOrganizer { get; } = typeof(SBertAPIOrganizer);
    public static Type SemanticEngine { get; } = typeof(SemanticEngine);
    public static Type FusionEngine { get; } = typeof(MeaningFrameFusionEngine);
  }
}