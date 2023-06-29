using Microsoft.ML.Data;

namespace Ummi.Runtime.Speech.SBert {
    public static class SBertShape {
        public const int BatchSize = 1;
        public const int SequenceLength = 32 ; 
        public const int LastLayerSize = 384;
    }

    public class SBertInput : IModelInput {
        [VectorType(SBertShape.BatchSize, SBertShape.SequenceLength)]
        [ColumnName("input_ids")]
        public long[] InputIds { get; set; }

        [VectorType(SBertShape.BatchSize, SBertShape.SequenceLength)]
        [ColumnName("attention_mask")]
        public long[] AttentionMask { get; set; }

        [VectorType(SBertShape.BatchSize, SBertShape.SequenceLength)]
        [ColumnName("token_type_ids")]
        public long[] TokenTypeIds { get; set; }
    }

    public class SBertOutput : IModelOutput {
        [VectorType(SBertShape.BatchSize, SBertShape.SequenceLength, SBertShape.LastLayerSize)]
        [ColumnName("last_hidden_state")]
        public VBuffer<float> LastHiddenState { get; set; }
    }
}