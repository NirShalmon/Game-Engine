using OpenTK;
using static System.Math;

namespace GameEngine
{
    /// <summary>
    /// A line collider simulates collisions of lines.
    /// </summary>
    public class LineCollider : Collider
    {
        /// <summary>
        /// The length of the line, it will be divided equaly to both sides of the center.
        /// </summary>
        public double length { set; get; }

        /// <summary>
        /// The angle of the line with the X axis in the local space.
        /// </summary>
        public Angle angle { set; get; }

        /// <summary>
        /// The angle of the line with the X axis in the global space.
        /// </summary>
        public double globalAngle {
            get {
                return angle + gameObject.angle;
            }
            set {
                angle = value - gameObject.angle;
            }
        }

        /// <summary>
        /// The angle perpendicular to the line with the X axis in the local space.
        /// </summary>
        public Angle normalAngle {
            get {
                return angle <= PI ? angle - MathHelper.PiOver2 : angle + MathHelper.PiOver2; 
            }
        }

        /// <summary>
        /// The angle perpendicular to the line with the X axis in the global space.
        /// </summary>
        public Angle normalGlobalAngle {
            get {
                return globalAngle <= PI ? globalAngle - MathHelper.PiOver2 : globalAngle + MathHelper.PiOver2;
            }
        }

        /// <summary>
        /// The gradient of the line in local space(dy/dx).
        /// </summary>
        public double gradient {
            get {
                return Tan(angle);
            }
            set {
                angle = Atan(value);
            }
        }

        /// <summary>
        /// The gradient of the line in global space(dy/dx).
        /// </summary>
        public double globalGradient {
            get {
                return Tan(globalAngle);
            }
            set {
                globalAngle = Atan(value);
            }
        }

        /// <summary>
        /// The gradient of the perpendicular to the line in local space(dy/dx).
        /// </summary>
        public double normalGradient {
            get {
                return Tan(normalAngle);
            }
        }

        /// <summary>
        /// The gradient of the perpendicular to the line in global space(dy/dx).
        /// </summary>
        public double normalGlobalGradient {
            get {
                return Tan(normalGlobalAngle);
            }
        }

        /// <summary>
        /// The vector perpendicular to the line in local space.
        /// </summary>
        public Vector2d normalVector {
            get {
                return ((double)normalAngle).toUnitVector();
            }
        }

        /// <summary>
        /// The vector perpendicular to the line in global space.
        /// </summary>
        public Vector2d globalNormalVector {
            get {
                return ((double)normalGlobalAngle).toUnitVector();
            }
        }

        /// <summary>
        /// An empty constructor for the line collider, sets all parameters to default.
        /// </summary>
        /// <param name="physics">The physics object linked to this collider.</param>
        public LineCollider(Physics physics) : base(physics) {
            
        }

        /// <summary>
        /// A constructor that includes most of the collider parameters.
        /// </summary>
        /// <param name="physics">The physics object linked to this collider.</param>
        /// <param name="center">The center of the line, in local space.</param>
        /// <param name="length">The length of the line, it will be divided equaly to both sides of the center.</param>
        /// <param name="angle">The angle of the line with the X axis in the local space.</param>
        /// <param name="bounciness">The bounciness coefficient of this collider.</param>
        public LineCollider(Physics physics,Vector2d center, double length, double angle, double bounciness): base(physics,center,bounciness) {
            this.length = length;
            this.angle = angle;
        }
    }
}
