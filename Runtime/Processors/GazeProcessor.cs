using ummi.Runtime.Processors;

namespace ummi.Runtime.Processors {
  public class GazeProcessor : Processor {
    public override ProcessorID ProcessorID { get; } = ProcessorID.DeicticGaze;
  }
}