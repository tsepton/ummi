using Ummi.Runtime.Speech.Tokenizer.Base;

namespace Ummi.Runtime.Speech.Tokenizer {
  public class BertCasedCustomVocabulary : CasedTokenizer {
    public BertCasedCustomVocabulary(string vocabularyFilePath) : base(vocabularyFilePath) { }
  }
}