using System.Reflection;
using Ummi.Runtime.Parser;

namespace Ummi.Runtime {
    public interface IFusionEngine {
        /// <summary>
        /// Gets the best parameters for the referenced <paramref name="method"/>.
        /// The selection of parameters relies on the implementation of this interface.
        /// </summary>
        /// <param name="method"></param>
        public void Complete(AttributeParser.RegisteredMMIMethod method);
    }
}