using Ummi.Runtime.Speech.Tokenizer.Base;

namespace Ummi.Runtime.Speech.Tokenizer {
  public class BertLargeTokenizer : CasedTokenizer {
    public BertLargeTokenizer() : base("./Vocabularies/base_cased_large.txt") { }
  }
}