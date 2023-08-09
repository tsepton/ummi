using ummi.Runtime.Processors;

namespace ummi.Runtime.Processors {
  public class VoiceProcessor: Processor {
    public override ProcessorID ProcessorID { get; } = ProcessorID.Voice;

  }
}