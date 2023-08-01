namespace Ummi.Runtime.Speech {
  public interface IModelOrganizer {
    public IModelOutput Forward(string question);
    public double[] Predict(string question);
    public double[][] Predict(string[] questions);
  }
  
}