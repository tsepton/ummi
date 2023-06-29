using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Ummi.Runtime.Parser;
using Ummi.Runtime.Speech;
using Ummi.Runtime.Speech.SBert;
using Ummi.Runtime;
using UnityEngine;

namespace Ummi.Tests {
  public class TestSpeech {
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
      IModelOrganizer model = Config.GetModelOrganizer();
      Assert.IsNotNull(model);
    }

    [Test]
    public void TestSBertOverall() {
      SBert sbert = new SBert();

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

      // Should not be registered - but its implementation should
      [MultimodalInterface(ID)]
      public abstract void AbstractMethod();

      // Should not be registered
      [MultimodalInterface(ID)]
      private static void PrivateMethod() { }
    }

    [Test]
    public void TestConcreteMethodRegistration() {
      IModelOrganizer organizer = Config.GetModelOrganizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      Assert.AreEqual(1, attributeParser.ConcreteMethods // Note: static member is also concrete here
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestAbstractMethodRegistration() {
      IModelOrganizer organizer = Config.GetModelOrganizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      Assert.AreEqual(0, attributeParser.AbstractMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestStaticMethodRegistration() {
      IModelOrganizer organizer = Config.GetModelOrganizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      Assert.AreEqual(1, attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray().Length);
    }

    [Test]
    public void TestPrivateMethodRegistration() {
      IModelOrganizer organizer = Config.GetModelOrganizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      Assert.AreEqual(0, attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .Where(m => m.Info.IsPrivate)
        .ToArray().Length);
    }

    [Test]
    public void TestStaticMethodInvoke() {
      IModelOrganizer organizer = Config.GetModelOrganizer();
      AttributeParser attributeParser = new AttributeParser(new[] { typeof(MmiApiRegistrationExample) }, organizer);
      var method = attributeParser.StaticMethods
        .Where(m => m.Utters.Contains(MmiApiRegistrationExample.ID))
        .ToArray();
      Assert.AreEqual(1, method.Length);
      method[0].Invoke();
      // Well, I guess if it did not raise an exception, everything's fine then
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
      SemanticEngine se = new SemanticEngine(new[] { typeof(MmiApiRegistrationExample) });
      MethodInfo method = se.Infer("Buy this stuff", threshold: 0.7f);
      if (method != null) Assert.AreEqual("OrderThisItem", method.Name);
      else throw new NullReferenceException("No matching MMI registered method was found");
    }

    [Test]
    public void TestInferThresholdWorks() {
      SemanticEngine se = new SemanticEngine(new[] { typeof(MmiApiRegistrationExample) });
      MethodInfo method = se.Infer("Buy this stuff", threshold: 1f);
      Assert.AreEqual(null, method);
      method = se.Infer("Order this item", threshold: 1f);
      if (method != null) Assert.AreEqual("OrderThisItem", method.Name);
      else throw new NullReferenceException("No matching MMI registered method was found");
    }
  }
}