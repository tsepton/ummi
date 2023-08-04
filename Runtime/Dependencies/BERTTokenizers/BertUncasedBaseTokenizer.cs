using Ummi.Runtime.Speech.Tokenizer.Base;

namespace Ummi.Runtime.Speech.Tokenizer {
  public class BertUncasedBaseTokenizer : UncasedTokenizer {
    public BertUncasedBaseTokenizer() : base("./Vocabularies/base_uncased.txt") { }
  }
}