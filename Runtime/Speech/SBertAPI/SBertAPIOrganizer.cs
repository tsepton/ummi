using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using UnityEngine;
using UnityEngine.Networking;

namespace Ummi.Runtime.Speech.SBertAPI {
  public class SBertAPIOrganizer : ModelOrganizer {
    private const string Uri = "http://127.0.0.1:8000/transform/";

    public SBertAPIOrganizer(ModelPaths paths) : base(paths) {
      // We don't need these as we are using a distant API
    }

    public override IModelOutput Forward(string question) {
      throw new System.NotImplementedException();
    }

    public override double[] Predict(string question) {
      return GetEmbedding(Uri + question);
    }

    public override double[][] Predict(string[] questions) {
      double[][] embeddings = new double[questions.Length][];
      foreach (var (question, i) in questions.Select((str, i) => (str, i))) {
        embeddings[i] = Predict(question);
      }

      return embeddings;
    }

    private double[] GetEmbedding(string utterance) {
      using (var client = new HttpClient(new HttpClientHandler
               { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })) {
        client.BaseAddress = new Uri(Uri);
        HttpResponseMessage response = client.GetAsync(utterance).Result;
        response.EnsureSuccessStatusCode();
        var x = response.Content.ReadAsStringAsync().Result;
        string[] strings = x.Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
        List<double> doubles = new List<double>();
        foreach (var str in strings) {
          doubles.Add(Double.Parse(str, System.Globalization.NumberStyles.Float)); // Contains scientific notation
        }

        return doubles.ToArray();
      }
    }
  }
}