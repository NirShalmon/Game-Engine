using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace GameEngine
{
    /// <summary>
    /// A struct for easy access to angles, it is in radians, and it will keep all the angles between zero to 2*pi automaticly. It could be implicitly treated as a double.
    /// </summary>
    public struct Angle
    {
        double angle;

        /// <summary>
        /// The constant pi(3.1414...).
        /// </summary>
        public const double pi = MathHelper.Pi;

        /// <summary>
        /// The constant pi devided by two(1.5707...).
        /// </summary>
        public const double piOver2 = MathHelper.PiOver2;

        /// <summary>
        /// The angle in degrees.
        /// </summary>
        public double degrees {
            get {
                return MathHelper.RadiansToDegrees(angle);
            }
            set {
                value = MathHelper.DegreesToRadians(value);
            }
        }

        /// <summary>
        /// An implicit operator to convert a double to an angle properly bounded between zero and two pi.
        /// </summary>
        /// <param name="d">The double to convert.</param>
        public static implicit operator Angle(double d) {
            return new Angle(Utility.positiveModulo(d,MathHelper.TwoPi));
        }

        /// <summary>
        /// An implicit operator to convert an angle to a double properly bounded between zero and two pi.
        /// </summary>
        /// <param name="a">The angle to convert.</param>
        public static implicit operator double(Angle a) {
            return a.angle;
        }

        Angle(double angle) {
            this.angle = angle;
        }
    }
}
