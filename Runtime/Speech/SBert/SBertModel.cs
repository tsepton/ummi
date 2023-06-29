using System.Collections.Generic;

namespace Ummi.Runtime.Speech.SBert {
  public class SBertModel : Model<SBertInput, SBertOutput> {
    protected override Dictionary<string, int[]> ShapeDictionary {
      get {
        return new Dictionary<string, int[]> {
          { "input_ids", new[] { SBertShape.BatchSize, SBertShape.SequenceLength } },
          { "attention_mask", new[] { SBertShape.BatchSize, SBertShape.SequenceLength } },
          { "token_type_ids", new[] { SBertShape.BatchSize, SBertShape.SequenceLength } },
          { "last_hidden_state", new[] { SBertShape.BatchSize, SBertShape.SequenceLength, SBertShape.LastLayerSize } },
        };
      }
    }

    protected override string[] InputColumnNames {
      get {
        return new[] {
          "input_ids",
          "attention_mask",
          "token_type_ids"
        };
      }
    }

    protected override string[] OutputColumnNames {
      get {
        return new[] {
          "last_hidden_state"
        };
      }
    }

    public SBertModel(bool useGpu = false) :
      base(modelPath, useGpu) { }
    
    // FIXME: absolute path
    public const string  modelPath = "Assets/Packages/tsepton.ummi/Runtime/Speech/SBert/ONNX/model.onnx";
    public const string  vocabularyFilePath = "Assets/Packages/tsepton.ummi/Runtime/Speech/SBert/ONNX/vocab.txt";
  }
}