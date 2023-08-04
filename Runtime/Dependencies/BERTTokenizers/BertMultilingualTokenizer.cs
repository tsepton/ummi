using Ummi.Runtime.Speech.Tokenizer.Base;

namespace Ummi.Runtime.Speech.Tokenizer {
  public class BertMultilingualTokenizer : CasedTokenizer {
    public BertMultilingualTokenizer() : base("./Vocabularies/base_cased_multilingual.txt") { }
  }
}