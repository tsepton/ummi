using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Ummi.Runtime.Parser;
using Ummi.Runtime.Speech;
using Ummi.Runtime.Speech.SBert;
using Ummi.Runtime;
using Ummi.Runtime.Exceptions;
using ummi.Runtime.Processors;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ummi.Tests {
  public class TestSBert {
    private string[] _corpus = new[] {
      "Order this item",
      "Place the yellow coat inside my cart",
      "Rent a hotel room"
    };

    private string[] _utterances = new[] {
      "Order this item"
    };

    //   long[] input = new[] { 1L, 0L, 1L, 1L };
    //   long[][] expectedOutput = new[] {
    //     new[] { 1L, 1L, 1L, 1L },
    //     new[] { 0L, 0L, 0L, 0L },
    //     new[] { 1L, 1L, 1L, 1L },
    //     new[] { 1L, 1L, 1L, 1L }
    //   };

    [Test]
    public void TestModelTrainingNotNull() {
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      ModelOrganizer model = new SBert(new ModelPaths(vocabPath, modelPath));
      Assert.IsNotNull(model);
    }

    [Test]
    public void TestSBertUncasedOutputs() {
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      ModelOrganizer model = new SBert(new ModelPaths(vocabPath, modelPath));
      double[] tCased = model.Predict("Click this button");
      double[] tUncased = model.Predict("Click tHis buTTon");
      Assert.AreEqual(tCased, tUncased);
    }

    [Test]
    public void TestSBertOverall() {
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SBert sbert = new SBert(new ModelPaths(vocabPath, modelPath));

      double[][] corpusOutputs = _corpus.Select(c => sbert.Predict(c)).ToArray();
      double[][] utterancesOutputs = _utterances.Select(utter => sbert.Predict(utter)).ToArray();

      foreach (var (i, c) in corpusOutputs.Select((value, i) => (i, value))) {
        foreach (var (j, u) in utterancesOutputs.Select((value, i) => (i, value))) {
          Debug.Log($"{i} - {j}: {c.CosSim(u)}");
        }
      }

      // FIXME: how can we test this?
      // Assert.AreEqual(expectedOutput, output);
    }
  }

  public class TestParser {
    abstract class MmiApiRegistrationExample {
      public const string ID = "TestParser methods";

      [MultimodalInterface(ID)]
      public static void PublicStaticMethod() { }

      [MultimodalInterface(ID)]
      public void PublicConcreteAndNotStaticMethod() { }

      // Should not be registered
      [MultimodalInterface(ID)]
      public int MethodWithReturnType() {
        return 42;
      }

      // Should not be registered - but its implementation should (Future wrk)
      [MultimodalInterface(ID)]
      public abstract void AbstractMethod();

      // Should not be registered
      [MultimodalInterface(ID)]
      private static void PrivateMethod() { }
    }

    private ModelOrganizer _organizer = new SBert(new ModelPaths(
      Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath),
      Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath))
    );

    [Test]
    public void TestConcreteMethodRegistration() {
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, _organizer);
      Assert.AreEqual(1, attributeParser.ConcreteMethods // Note: static member is also concrete here
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestAbstractMethodRegistration() {
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, _organizer);
      Assert.AreEqual(0, attributeParser.AbstractMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestStaticMethodRegistration() {
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, _organizer);
      Assert.AreEqual(1, attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestPrivateMethodRegistration() {
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, _organizer);
      Assert.AreEqual(0, attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .Where(m => m.Info.IsPrivate)
        .ToArray().Length);
    }

    [Test]
    public void TestStaticMethodInvoke() {
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, _organizer);
      var method = attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray();
      Assert.AreEqual(1, method.Length);
      Assert.DoesNotThrow(method[0].Invoke);
      // Well, I guess if it did not raise an exception, everything's fine then
    }

    [Test]
    public void TestEnsureNoMethodsHasTheSameEmbeddings() {
      //TODO
    }
  }

  public class TestSemanticEngine {
    abstract class MmiApiRegistrationExample {
      [MultimodalInterface("Order this item")]
      public static void OrderThisItem() {
        Debug.Log("OrderThisItem has done its side effect magic");
      }

      [MultimodalInterface("Paint my car in a color")]
      public static void PaintThisCar() { }
    }

    [Test]
    public void TestInferFindTheMostLogicalMethod() {
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath);
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      AttributeParser.RegisteredMMIMethod method = se.Infer("Buy this stuff", threshold: 0.65f)[0].Method;
      if (method != null) Assert.AreEqual("OrderThisItem", method.Info.Name);
      else throw new NullReferenceException("No matching MMI registered method was found");
    }

    [Test]
    public void TestInferThresholdWorks() {
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath);
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      Assert.AreEqual(null, se.Infer("Buy this stuff", threshold: 1f));
      AttributeParser.RegisteredMMIMethod method = se.Infer("Order this item", threshold: 1f)[0].Method;
      if (method != null) Assert.AreEqual("OrderThisItem", method.Info.Name);
      else throw new NullReferenceException("No matching MMI registered method was found");
    }

    [Test]
    public void ComputeCosineSimilarity_ValidInput_ReturnsExpectedSimilarity() {
      double[] vector1 = { 1.0, 2.0, 3.0 };
      double[] vector2 = { 4.0, 5.0, 6.0 };
      double expectedSimilarity = 0.9746318461970762;
      double actualSimilarity = vector1.CosSim(vector2);
      Assert.AreEqual(expectedSimilarity, actualSimilarity, 1e-10);
    }

    [Test]
    public void ComputeCosineSimilarity_ZeroVectors_ThrowsArgumentException() {
      double[] vector1 = { 0.0, 0.0, 0.0 };
      double[] vector2 = { 0.0, 0.0, 0.0 };
      Assert.Throws<ArgumentException>(() => vector1.CosSim(vector2));
    }

    [Test]
    public void ComputeCosineSimilarity_DifferentVectorLength_ThrowsArgumentException() {
      double[] vector1 = { 1.0, 2.0, 3.0 };
      double[] vector2 = { 4.0, 5.0, 6.0, 7.0 };
      Assert.Throws<ArgumentException>(() => vector1.CosSim(vector2));
    }

    [Test]
    public void ComputeCosineSimilarity_OrthogonalVectors_ReturnsSimilarityZero() {
      double[] vector1 = { 1.0, 0.0 };
      double[] vector2 = { 0.0, 1.0 };
      double actualSimilarity = vector1.CosSim(vector2);
      Assert.AreEqual(0.0, actualSimilarity, 1e-10);
    }

    [Test]
    public void ComputeCosineSimilarity_OppositeDirectionVectors_ReturnsSimilarityMinusOne() {
      double[] vector1 = { 1.0, 2.0, 3.0 };
      double[] vector2 = { -1.0, -2.0, -3.0 };
      double actualSimilarity = vector1.CosSim(vector2);
      Assert.AreEqual(-1.0, actualSimilarity, 1e-10);
    }

    [Test]
    public void ThrowsErrorIfNoCorpusGiven() {
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath);
      Assert.That(() => se.Infer(""), Throws.TypeOf<NoCorpusException>());
    }
  }

  public class TestFusionFrame {
    abstract class MmiApiRegistrationExample {
      [MultimodalInterface("Order this item")]
      public static void OrderThisItem(GameObject item) {
        Debug.Log($"--------- {item} ---------");
      }

      public static ItemMockup Car = new ItemMockup("car", 20000, 1);
      public static ItemMockup Bus = new ItemMockup("bus", 50000, 2);

      [MultimodalInterface("Check Items Inferred Are Correct")]
      public static void CheckItemsInferredAreCorrect(ItemMockup item1, ItemMockup item2) {
        Assert.IsTrue(item1.id == Car.id);
        Assert.IsTrue(item2.id == Bus.id);
      }

      [MultimodalInterface("Update that item color")]
      public static void UpdateItemColor(ItemMockup item, Color color) {
        Assert.IsTrue(item.id == Car.id);
      }
    }

    class ItemMockup {
      public string Name;
      public int Price;
      public int id;

      public ItemMockup(string name, int price, int id) {
        this.Name = name;
        this.Price = price;
        this.id = id;
      }
    }

    [Test]
    public void TestBaseFusion() {
      FactBase.Instance.Clear();
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath );
      MeaningFrameFusionEngine frameFusionEngine = new MeaningFrameFusionEngine();
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      FactBase.Instance.Add(new GameObject(), new Source(ProcessorID.Voice, 0));
      AttributeParser.RegisteredMMIMethod method = se.Infer("Order this thing")[0].Method;
      Assert.IsNotNull(method);
      Assert.IsTrue(frameFusionEngine.Call(method));
    }

    [Test]
    public void TestMultipleTypesFusionAndTakesFirstAvailableArg() {
      FactBase.Instance.Clear();
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath );
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      MeaningFrameFusionEngine frameFusionEngine = new MeaningFrameFusionEngine();
      FactBase.Instance.Add(MmiApiRegistrationExample.Car, new Source(ProcessorID.Voice, 1));
      FactBase.Instance.Add(MmiApiRegistrationExample.Bus, new Source(ProcessorID.Voice, 2));
      FactBase.Instance.Add(MmiApiRegistrationExample.Bus, new Source(ProcessorID.Voice, 3));
      FactBase.Instance.Add(new Color(), new Source(ProcessorID.Voice, 0));
      AttributeParser.RegisteredMMIMethod method = se.Infer("Update that item color")[0].Method;
      Assert.IsNotNull(method);
      Assert.IsTrue(frameFusionEngine.Call(method));
    }

    [Test]
    public void TestNoFusionOccurs_NoFact() {
      FactBase.Instance.Clear();
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath );
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      MeaningFrameFusionEngine frameFusionEngine = new MeaningFrameFusionEngine();
      AttributeParser.RegisteredMMIMethod method = se.Infer("Order this thing")[0].Method;
      Assert.IsNotNull(method);
      Assert.IsFalse(frameFusionEngine.Call(method));
    }

    [Test]
    public void TestNoFusionOccurs_NoCorrectlyTypedFact() {
      FactBase.Instance.Clear();
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath );
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      MeaningFrameFusionEngine frameFusionEngine = new MeaningFrameFusionEngine();
      FactBase.Instance.Add(new Object(), new Source(ProcessorID.Voice, 0));
      AttributeParser.RegisteredMMIMethod method = se.Infer("Order this thing")[0].Method;
      Assert.IsNotNull(method);
      Assert.IsFalse(frameFusionEngine.Call(method));
    }

    [Test]
    public void TestMultipleArgs_CorrectlyInferredFusion() {
      FactBase.Instance.Clear();
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath );
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      MeaningFrameFusionEngine frameFusionEngine = new MeaningFrameFusionEngine();
      FactBase.Instance.Add(MmiApiRegistrationExample.Car, new Source(ProcessorID.Voice, 0));
      FactBase.Instance.Add(MmiApiRegistrationExample.Bus, new Source(ProcessorID.Voice, 1));
      AttributeParser.RegisteredMMIMethod method = se.Infer("Check Items Inferred Are Correct")[0].Method;
      Assert.IsNotNull(method);
      Assert.IsTrue(frameFusionEngine.Call(method)); // method has Assert in its body
    }

    [Test]
    public void TestLinkedFacts_NotReusedIfOneAlreadyInferred() {
      FactBase.Instance.Clear();
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath );
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      MeaningFrameFusionEngine frameFusionEngine = new MeaningFrameFusionEngine();
      DateTime beforeInfer = DateTime.UtcNow;
      AttributeParser.RegisteredMMIMethod method = se.Infer("Update that item color")[0].Method;
      FactBase.Instance.Add(MmiApiRegistrationExample.Car, new Source(ProcessorID.Voice, 0));
      FactBase.Instance.Add(new Color(), new Source(ProcessorID.Voice, 1));
      Assert.IsNotNull(method);
      Assert.IsTrue(frameFusionEngine.Call(method, beforeInfer, TimeSpan.FromSeconds(2)));
      FactBase.Instance.Add(MmiApiRegistrationExample.Car, new Source(ProcessorID.Voice, 0));
      FactBase.Instance.Add(new Color(), new Source(ProcessorID.Voice, 2));
      // method should not be called again, because no completion parameters should be found (same source for Car)
      Assert.IsFalse(frameFusionEngine.Call(method, beforeInfer, TimeSpan.FromSeconds(2)));
    }
    
    [Test]
    public void TestLinkedFacts_ReusedIfOneAlreadyInferred_IfWithDifferentProcessor() {
      FactBase.Instance.Clear();
      string vocabPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultVocabularyPath);
      string modelPath = Path.Combine(Application.streamingAssetsPath, Config.DefaultModelPath);
      SemanticEngine se = new SemanticEngine(modelPath, vocabPath );
      se.Register(new[] { typeof(MmiApiRegistrationExample) });
      MeaningFrameFusionEngine frameFusionEngine = new MeaningFrameFusionEngine();
      DateTime beforeInfer = DateTime.UtcNow;
      AttributeParser.RegisteredMMIMethod method = se.Infer("Update that item color")[0].Method;
      FactBase.Instance.Add(MmiApiRegistrationExample.Car, new Source(ProcessorID.Voice, 0));
      FactBase.Instance.Add(new Color(), new Source(ProcessorID.Voice, 1));
      Assert.IsNotNull(method);
      Assert.IsTrue(frameFusionEngine.Call(method, beforeInfer, TimeSpan.FromSeconds(2)));
      FactBase.Instance.Add(MmiApiRegistrationExample.Car, new Source(ProcessorID.DeicticGaze, 0));
      FactBase.Instance.Add(new Color(), new Source(ProcessorID.Voice, 2));
      // method should be called again, because Sources are not the same
      Assert.IsTrue(frameFusionEngine.Call(method, beforeInfer, TimeSpan.FromSeconds(2)));
    }
    
  }
}