using System;
using System.Collections;

namespace GameEngine
{
    /// <summary>
    /// General utility functions to aid development.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// A modolu specialized to work for negative numbers, less efficiant than regular remainder(%).
        /// </summary>
        /// <example>
        /// positiveModulu(11,5) = 1
        /// positiveModulu(-1,7) = 6
        /// </example>
        /// <param name="i">The number to apply modolu on.</param>
        /// <param name="n">The base of the modulu operation.</param>
        /// <returns>The result of the modulu operation.</returns>
        public static double positiveModulo(double i,double n) {
            return (i % n + n) % n;
        }
    }
}
