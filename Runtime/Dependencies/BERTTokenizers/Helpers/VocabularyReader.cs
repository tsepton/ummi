using System.Collections.Generic;
using System.IO;

namespace Ummi.Runtime.Speech.Tokenizer.Helpers {
  public class VocabularyReader {
    public static List<string> ReadFile(string filename) {
      var result = new List<string>();

      using (var reader = new StreamReader(filename)) {
        string line;

        while ((line = reader.ReadLine()) != null)
          if (!string.IsNullOrWhiteSpace(line))
            result.Add(line);
      }

      return result;
    }
  }
}