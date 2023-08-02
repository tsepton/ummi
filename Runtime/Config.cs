using Ummi.Runtime.Speech;
using Ummi.Runtime.Speech.SBert;

namespace Ummi.Runtime {
    // FIXME
    public static class Config {
        private static IModelOrganizer _organizer = new SBert();
        private static ISemanticEngine _semanticEngine = new SemanticEngine();
        private static IFusionEngine _fusionEngine = new MeaningFrameFusionEngine();
        
        public static IModelOrganizer Organizer => _organizer;
        public static ISemanticEngine SemanticEngine => _semanticEngine;
        public static IFusionEngine FusionEngine => _fusionEngine;
    }
}