using OpenTK;
using static System.Math;

namespace GameEngine
{
    /// <summary>
    /// A circle collider simulates collisions of circles.
    /// </summary>
    public class CircleCollider : Collider
    {
        /// <summary>
        /// The radius of the collision circle.
        /// </summary>
        public double radius { set; get; }

        /// <summary>
        /// The diamerter of the collision circle.
        /// </summary>
        public double diameter {
            get {
                return radius * 2;
            }
            set {
                radius = value / 2;
            }
        }

        /// <summary>
        /// Construct a circle collider based on some of the parameters.
        /// </summary>
        /// <param name="radius">The radius of the circle collider.</param>
        /// <param name="center">The center of the circle collider relatice to the gameobject.</param>
        /// <param name="physics">The physics object connected to this cllider.</param>
        public CircleCollider(double radius,Vector2d center,Physics physics):base(physics,center) {
            this.radius = radius;
        }

        /// <summary>
        /// Construct a circle collider based on all of the parameters.
        /// </summary>
        /// <param name="radius">The radius of the circle collider.</param>
        /// <param name="center">The center of the circle collider relatice to the gameobject.</param>
        /// <param name="bounciness">The bounciness of this collider.</param>
        /// <param name="physics">The physics object connected to this cllider.</param>
        public CircleCollider(double radius,Vector2d center, double bounciness, Physics physics): base(physics,center,bounciness) {
            this.radius = radius;
        }
    }
}
