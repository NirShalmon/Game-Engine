using OpenTK;
using static System.Math;

namespace GameEngine
{
    /// <summary>
    /// A collider is the shape data the physics engine uses for collisions.
    /// </summary>
    public abstract class Collider
    {
        /// <summary>
        /// The physics object connected to this collider.
        /// </summary>
        public readonly Physics physics;

        /// <summary>
        /// The game object connected to this collider.
        /// </summary>
        public GameObject gameObject {
            get {
                return physics.gameObject;
            }
        }

        /// <summary>
        /// How bouncy will the object be? Recomended to keep between zero and one.
        /// </summary>
        public double bounciness { set; get; } = 0.5;

        /// <summary>
        /// The coefficiant of friction of the objet with objects it is not moving relative to.
        /// </summary>
        public double staticFriction { set; get; } = 0.12;

        /// <summary>
        /// The coefficiant of friction of the objet with objects it is moving relative to.
        /// </summary>
        public double dynamicFriction { set; get; } = 0.1;

        /// <summary>
        /// The center of the circle collider relative to the gameobject.
        /// </summary>
        public Vector2d center { set; get; }

        /// <summary>
        /// The center of the collider in global space.
        /// </summary>
        public Vector2d globalCenter {
            get {
                return gameObject.localPositionToGlobal(center);
            }
            set {
                center =gameObject.globalPositionToLocal(value);
            }
        }

        /// <summary>
        /// A simple constructor that doesn't fill in all of the properties.
        /// </summary>
        /// <param name="physics">The physics object attached with this collider.</param>
        public Collider(Physics physics) {
            this.physics = physics;
        }

        /// <summary>
        /// A constructor taking some of the collider properties.
        /// </summary>
        /// <param name="physics">The physics object attached with this collider.</param>
        /// <param name="center">The local center of the collider.</param>
        public Collider(Physics physics, Vector2d center) {
            this.physics = physics;
            this.center = center;
        }

        /// <summary>
        /// A constructor taking all of the collider properties.
        /// </summary>
        /// <param name="physics">The physics object attached with this collider.</param>
        /// <param name="center">The local center of the collider.</param>
        /// <param name="bounciness">The bounciness of the collider, recomended between 0 and one.</param>
        public Collider(Physics physics,Vector2d center, double bounciness) {
            this.physics = physics;
            this.center = center;
            this.bounciness = bounciness;
        }
    }
}