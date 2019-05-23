using OpenTK;
using System.Collections.Generic;
using static System.Math;

namespace GameEngine
{
    /// <summary>
    /// A collision object includes data about the collision, and functions to invoke the collision. This is passed in collision related events.
    /// </summary>
    public class Collision
    {
        /// <summary>
        /// The first collider in the collision.
        /// </summary>
        public readonly Collider colliderA;

        /// <summary>
        /// The second collider in the collision.
        /// </summary>
        public readonly Collider colliderB;

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
        public readonly Vector2d contact;

        /// <summary>
        /// The bounce coefficient of the collision.
        /// </summary>
        public readonly double restetution;

        /// <summary>
        /// The depth of the overlap between the two colliders.
        /// </summary>
        public readonly double penetrationDepth;

        /// <summary>
        /// A collision constructor filling in all of the collision data.
        /// </summary>
        /// <param name="a">The first collider in the collision.</param>
        /// <param name="b">The second collider in the collision.</param>
        /// <param name="normal">The vector perpendicular to the collision.</param>
        /// <param name="contact">The point where the two colliders touched</param>
        /// <param name="penetrationDepth">How deep are the objects penetrating into each other</param>
        public Collision(Collider a, Collider b, Vector2d normal, Vector2d contact, double penetrationDepth) {
            colliderA = a;
            colliderB = b;
            relativeVelocity = b.physics.getVelocityAtPoint(contact) - a.physics.getVelocityAtPoint(contact);
            restetution = Min(a.bounciness,b.bounciness);
            this.normal = normal;
            tangent = normal.rotate90Deg();
            relativeSpeedAlongNormal = Vector2d.Dot(relativeVelocity, normal);
            relativeSpeedAlongTangent = Vector2d.Dot(relativeVelocity, tangent);
            this.contact = contact;
            this.penetrationDepth = penetrationDepth;
        }

        /// <summary>
        /// Invoke the collision, this will make the physics engine take the collision into effect.
        /// </summary>
        public void collide() {
            //Collision:
            double j = -relativeSpeedAlongNormal * (1 + restetution);
            double momentumBalancer;
            if(colliderA.physics.totalMass == double.PositiveInfinity) {
                momentumBalancer = colliderB.physics.totalMass;
            }else if(colliderB.physics.totalMass == double.PositiveInfinity) {
                momentumBalancer = colliderA.physics.totalMass;
            }else {
                momentumBalancer = (colliderA.physics.totalMass * colliderB.physics.totalMass) / (colliderA.physics.totalMass + colliderB.physics.totalMass);
            }
            double normalForce = -relativeSpeedAlongNormal * (1 + restetution) * momentumBalancer;
            Vector2d impulse = normalForce * normal;
            Vector2d frictionDirection = relativeSpeedAlongTangent > 0 ? -tangent : tangent;
            double maxStaticFrictionForce = normalForce * (colliderA.staticFriction + colliderB.staticFriction) / 2;
            //test application of max static friction to see if static friction can arrest tangent motion
            colliderA.physics.applyForceAtPosition(-frictionDirection * maxStaticFrictionForce, contact, Space.global);
            colliderB.physics.applyForceAtPosition(frictionDirection * maxStaticFrictionForce, contact, Space.global);
            double newRelativeSpeedAlongTangent = Vector2d.Dot(colliderA.physics.getVelocityAtPoint(contact) - colliderB.physics.getVelocityAtPoint(contact), tangent);
            //cancel out the tested force application
            colliderA.physics.applyForceAtPosition(frictionDirection * maxStaticFrictionForce, contact, Space.global);
            colliderB.physics.applyForceAtPosition(-frictionDirection * maxStaticFrictionForce, contact, Space.global);
            double tangentSpeedChange = Abs(newRelativeSpeedAlongTangent - relativeSpeedAlongTangent);
            if (tangentSpeedChange > Abs(relativeSpeedAlongTangent)) { //if we were able to change direction: use static friction to arrest tangent velocity completly
                double actualStaticFriction = maxStaticFrictionForce * Abs(relativeSpeedAlongTangent) / tangentSpeedChange; //use linearity of speed change(works even with rotation)
                impulse += frictionDirection * actualStaticFriction;
            } else {
                double dynamicFricitonForce = normalForce * (colliderA.dynamicFriction + colliderB.dynamicFriction) / 2;
                impulse += frictionDirection * dynamicFricitonForce;
            }
            colliderA.physics.applyForceAtPosition(-impulse, contact, Space.global);
            colliderB.physics.applyForceAtPosition(impulse, contact, Space.global);
            pushApart();
        }

        /// <summary>
        /// Teleports the colliding objects to remove collider ovelap. Will make negligable changes on reasonable velocities.
        /// </summary>
        void pushApart() {
            if (colliderB.physics.totalMass == double.PositiveInfinity) {
                colliderA.gameObject.position -= normal * penetrationDepth;
            } else {
                colliderA.gameObject.position -= normal * penetrationDepth * colliderB.physics.totalMass / (colliderA.physics.totalMass + colliderB.physics.totalMass);
            }
            if (colliderA.physics.totalMass == double.PositiveInfinity) {
                colliderB.gameObject.position += normal * penetrationDepth;
            } else {
                colliderB.gameObject.position += normal * penetrationDepth * colliderA.physics.totalMass / (colliderA.physics.totalMass + colliderB.physics.totalMass);
            }
        }
    }
}
