using Ummi.Runtime.Speech.Tokenizer.Base;

namespace Ummi.Runtime.Speech.Tokenizer {
  public class BertBaseTokenizer : CasedTokenizer {
    public BertBaseTokenizer() : base("./Assets/LLM/Tokenizer/Vocabularies/base_cased.txt") { }
  }
}