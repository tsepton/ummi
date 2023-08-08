using System;
using System.Reflection;
using Ummi.Runtime.Parser;

namespace Ummi.Runtime {
    public interface IFusionEngine {

        /// <summary>
        /// Gets the best parameters for the referenced <paramref name="method"/> (if any) and invokes it.
        /// The selection of parameters relies on the implementation of this interface.
        /// </summary>
        /// <param name="method">The method to complete and invoke</param>
        /// <returns>True if the method was called, false otherwise.</returns>
        public bool Call(AttributeParser.RegisteredMMIMethod method);

        /// <summary>
        /// Gets the best parameters for the referenced <paramref name="method"/> (if any) and invokes it.
        /// The selection of parameters relies on the implementation of this interface.
        /// Depending on the chosen algorithm, this method might provide better results since only a restrained subset
        /// of Facts will be evaluated.
        /// </summary>
        /// <param name="method">The method to complete and invoke</param>
        /// <param name="startedAt">The time when the command began to be emitted by the user</param>
        /// <param name="duration">The duration of the command</param>
        /// <returns>True if the method was called, false otherwise.</returns>
        public bool Call(AttributeParser.RegisteredMMIMethod method, DateTime startedAt, TimeSpan duration);
    }
}