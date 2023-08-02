using System;

namespace Ummi.Runtime.Exceptions {
  
  /// <summary>
  /// Base class for all Exception thrown by Ummi.
  /// </summary>
  public class UmmiException: Exception {}
  
  public class FusionEngineException: UmmiException {}
  
  public class SemanticEngineException: UmmiException {}
    
  public class NoCorpusException: SemanticEngineException {}
  
}