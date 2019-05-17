using System;
using OpenTK;

namespace GameEngine
{
    public class Contact
    {
        /// <summary>
        /// The relative velocity vector of the collision at the contact point.
        /// </summary>
        public readonly Vector2d relativeVelocity;

        /// <summary>
        /// The relative speed in the direction perpendicular to the colision.
        /// </summary>
        public readonly double relativeSpeedAlongNormal;

        /// <summary>
        /// The relative speed in the direction tangent to the colision.
        /// </summary>
        public readonly double relativeSpeedAlongTangent;

        /// <summary>
        /// The vector perpendicular to the collision.
        /// </summary>
        public readonly Vector2d normal;

        /// <summary>
        /// The vector tangent to the collision.
        /// </summary>
        public readonly Vector2d tangent;

        /// <summary>
        /// The point where the two colliders touched.
        /// </summary>
        public readonly Vector2d point;

        public Contact(Vector2d point, Vector2d normal, Vector2d relativeVelocity) {
            this.point = point;
            this.normal = normal;
            this.relativeVelocity = relativeVelocity;
            relativeSpeedAlongNormal = Vector2d.Dot(relativeVelocity,normal);
            tangent = (relativeVelocity - relativeSpeedAlongNormal * normal).Normalized();   //Remove the normal velocity from the relative velocity.
            relativeSpeedAlongTangent = Vector2d.Dot(relativeVelocity,tangent);
        }
    }
}
