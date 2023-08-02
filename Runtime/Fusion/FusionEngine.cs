using System.Reflection;
using Ummi.Runtime.Parser;

namespace Ummi.Runtime {
    public interface IFusionEngine {
        /// <summary>
        /// Gets the best parameters for the referenced <paramref name="method"/> (if any) and invokes it.
        /// The selection of parameters relies on the implementation of this interface.
        /// </summary>
        /// <param name="method"></param>
        /// <returns>True if the method was called, false otherwise.</returns>
        public bool Call(AttributeParser.RegisteredMMIMethod method);
    }
}