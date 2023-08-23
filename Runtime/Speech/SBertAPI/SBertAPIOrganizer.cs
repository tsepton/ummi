namespace Ummi.Runtime.Speech.SBertAPI {
  public class SBertAPIOrganizer: ModelOrganizer {
    public SBertAPIOrganizer(ModelPaths paths) : base(paths) { }
    public override IModelOutput Forward(string question) {
      throw new System.NotImplementedException();
    }

    public override double[] Predict(string question) {
      throw new System.NotImplementedException();
    }

    public override double[][] Predict(string[] questions) {
      throw new System.NotImplementedException();
    }
  }
}