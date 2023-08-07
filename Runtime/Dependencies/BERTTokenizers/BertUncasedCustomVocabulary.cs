﻿using Ummi.Runtime.Speech.Tokenizer.Base;

namespace Ummi.Runtime.Speech.Tokenizer {
  public class BertUncasedCustomVocabulary : UncasedTokenizer {
    public BertUncasedCustomVocabulary(string vocabularyFilePath) : base(vocabularyFilePath) { }
  }
}