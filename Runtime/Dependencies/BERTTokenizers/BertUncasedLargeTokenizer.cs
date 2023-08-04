using Ummi.Runtime.Speech.Tokenizer.Base;

namespace Ummi.Runtime.Speech.Tokenizer {
  public class BertUncasedLargeTokenizer : UncasedTokenizer {
    public BertUncasedLargeTokenizer() : base("./Vocabularies/base_uncased_large.txt") { }
  }
}