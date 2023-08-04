using Ummi.Runtime.Speech.Tokenizer.Base;

namespace Ummi.Runtime.Speech.Tokenizer {
  public class BertGermanTokenizer : CasedTokenizer {
    public BertGermanTokenizer() : base("./Vocabularies/base_cased_german.txt") { }
  }
}