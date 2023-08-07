namespace Ummi.Runtime.Speech {
  public abstract class ModelOrganizer {
    public ModelOrganizer(ModelPaths paths) { }

    public abstract IModelOutput Forward(string question);
    public abstract double[] Predict(string question);
    public abstract double[][] Predict(string[] questions);
  }


  public class ModelPaths {
    public readonly string Vocabulary;
    public readonly string Onnx;
    public ModelPaths(string vocabularyPath, string onnxPath) {
      Vocabulary = vocabularyPath;
      Onnx = onnxPath;
    }
  }
  
}