using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Ummi.Runtime.Speech.Tokenizer;
using Ummi.Runtime.Speech.Tokenizer.Base;
using UnityEngine;

namespace Ummi.Runtime.Speech.SBert {
  public class SBert : IModelOrganizer {
    private List<string> _vocabulary;

    private readonly CasedTokenizer _tokenizer;
    private SBertModel _model;
    // FIXME: these absolutely horrific absolute paths
    public SBert() {
      _tokenizer = new BertUncasedCustomVocabulary(SBertModel.vocabularyFilePath);
      
      _model = new SBertModel();
    }

    public IModelOutput Forward(string question) {
      var input = _tokenizer.Encode(SBertShape.SequenceLength, question).ToSBertInput();
      var predictions = _model.Predict(input);
      // Debug.Log("--------------------------");
      // Debug.Log("------- SBertInput -------");
      // Debug.Log("--------------------------");
      // Debug.Log("Input IDs " + string.Join(", ", input.InputIds));
      // Debug.Log("Attention Mask " + string.Join(", ", input.AttentionMask));
      // Debug.Log("Token Type IDs " + string.Join(", ", input.TokenTypeIds));

      return predictions;
    }

    public double[] Predict(string question) {
      var input = _tokenizer.Encode(SBertShape.SequenceLength, question).ToSBertInput();
      var predictions = _model.Predict(input);
      var outputs = predictions.LastHiddenState.GetValues().ToArray();
      
      // Debug.Log("--------------------------");
      // Debug.Log("------- SBertInput -------");
      // Debug.Log("--------------------------");
      // Debug.Log("Input IDs " + string.Join(", ", input.InputIds));
      // Debug.Log("Attention Mask " + string.Join(", ", input.AttentionMask));
      // Debug.Log("Token Type IDs " + string.Join(", ", input.TokenTypeIds));
      
      // DEBUG Happening in the following lines (or maybe inside CosSim extension method)
      var outputsPrime = MeanPooling(outputs, input.AttentionMask);

      double sum = 0;
      foreach (var output in outputsPrime) {
        sum += Math.Pow(output, 2);
      }

      double norm = Math.Sqrt(sum);
      double[] normalized = outputsPrime.Select(d => d / Math.Max(norm, 1)).ToArray();
      // Debug.Log(string.Join(", ", normalized));
      return normalized;
    }

    public double[][] Predict(string[] questions) {
      double[][] embeddings = new double[questions.Length][];
      foreach (var (question, i) in questions.Select((str, i) => (str, i))) {
        embeddings[i] = Predict(question);
      }
      return embeddings;
    }

    private double[] MeanPooling(float[] tokenEmbeddings, long[] attentionMask) {
      long[][] inputMaskExpanded = ExpandAttentionMask(attentionMask, SBertShape.LastLayerSize);
      double[] sum = new double[SBertShape.LastLayerSize];
      double[] clamp = new double[SBertShape.LastLayerSize];

      for (int i = 0; i < inputMaskExpanded.Length; i++) {
        for (int j = 0; j < SBertShape.LastLayerSize; j++) {
          int tindex = i * SBertShape.LastLayerSize + j;
          sum[j] += tokenEmbeddings[tindex] * inputMaskExpanded[i][j];
          clamp[j] += inputMaskExpanded[i][j];
        }
      }

      for (int i = 0; i < inputMaskExpanded.Length; i++) {
        sum[i] /= Math.Max(clamp[i], 1e-9);
      }

      return sum;
    }

    private static long[][] ExpandAttentionMask(long[] attentionMask, int length) {
      long[][] expandedTensor = new long[attentionMask.Length][];

      for (int i = 0; i < attentionMask.Length; i++) {
        expandedTensor[i] = new long[length];
        for (int j = 0; j < length; j++) {
          expandedTensor[i][j] = attentionMask[i];
        }
      }

      return expandedTensor;
    }
  }

  public static class TokenizerExtension {
    public static SBertInput ToSBertInput(this List<(long InputIds, long TokenTypeIds, long AttentionMask)> tokens) {
      return new SBertInput() {
        InputIds = tokens.Select(t => t.InputIds).ToArray(),
        AttentionMask = tokens.Select(t => t.AttentionMask).ToArray(),
        TokenTypeIds = tokens.Select(t => t.TokenTypeIds).ToArray()
      };
    }
  }
}