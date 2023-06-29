namespace Ummi.Runtime.Speech {
  public interface IModelOrganizer {
    public IModelOutput Forward(string question);
    public double[] Predict(string question);
    public double[][] Predict(string[] questions);
  }

  public static class Config {
    public static IModelOrganizer GetModelOrganizer() {
      // TODO - use a config file
      return new SBert.SBert();
    }
  }
}