using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;

namespace Ummi.Runtime.Speech {
  public abstract class Model<T, U>
    where T : IModelInput, new()
    where U : IModelOutput, new() {
    protected MLContext Context;

    protected PredictionEngine<T, U> PredictionEngine;

    protected abstract Dictionary<string, int[]> ShapeDictionary { get; }
    protected abstract string[] InputColumnNames { get; }
    protected abstract string[] OutputColumnNames { get; }

    protected Model(string modelPath, bool useGpu = false) {
      Context = new MLContext(seed: 0);
      var model = GetTransformerFromOnnx(modelPath, useGpu);
      PredictionEngine = Context.Model.CreatePredictionEngine<T, U>(model);
    }

    private ITransformer GetTransformerFromOnnx(string path, bool useGpu) {
      OnnxScoringEstimator pipeline = Context.Transforms
        .ApplyOnnxModel(modelFile: path,
          shapeDictionary: ShapeDictionary,
          inputColumnNames: InputColumnNames,
          outputColumnNames: OutputColumnNames,
          gpuDeviceId: useGpu ? 0 : (int?)null,
          fallbackToCpu: true);
      return pipeline.Fit(Context.Data.LoadFromEnumerable(new List<T>()));
    }

    public U Predict(T encodedInput) {
      return PredictionEngine.Predict(encodedInput);
    }
  }
}