using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using static System.Math;

namespace GameEngine
{
    /// <summary>
    /// This class should contain a getCollision function for each possible pair of colliders.
    /// Add functions to this class when you create new colliders.
    /// </summary>
    public static partial class CollisionDetectors
    {
        /// <summary>
        /// Get collision data between two collliders. NOTE, this will not cause a physical collision, if you want to cause a collision use the function "collide()" in the object returned.
        /// </summary>
        /// <param name="a">Any circle collider.</param>
        /// <param name="b">Any Circle collider.</param>
        /// <returns>The collision data between the colliders, or null if no collision occured.</returns>
        public static Collision getCollision(CircleCollider a, CircleCollider b) {
            if ((a.globalCenter - b.globalCenter).LengthSquared <= (a.radius + b.radius) * (a.radius + b.radius)) {
                return new Collision(a,
                            b,
                            -(a.globalCenter - b.globalCenter).Normalized(), //normal
                            (a.globalCenter * a.radius + b.globalCenter * b.radius) / (a.radius + b.radius), //point(weighted average)
                            a.radius + b.radius - a.globalCenter.distance(b.globalCenter));
            }
            return null;
        }

        /// <summary>
        /// Get collision data between two collliders. NOTE, this will not cause a physical collision, if you want to cause a collision use the function "collide()" in the object returned.
        /// </summary>
        /// <param name="a">Any circle collider.</param>
        /// <param name="b">Any line collider.</param>
        /// <returns>The collision data between the colliders, or null if no collision occured.</returns>
        public static Collision getCollision(CircleCollider a,LineCollider b) {
            //precalculate gradient for efficiancy.
            double gradient = b.globalGradient;
            //squere distance between circle center to possible contact point
            double sqrDistance = a.globalCenter.Y - b.globalCenter.Y + gradient * (b.globalCenter.X - a.globalCenter.X);
            sqrDistance *= sqrDistance / (gradient * gradient + 1);
            if(sqrDistance > a.radius * a.radius) {
                return null;
            }
            if(a.globalCenter.sqrDistance(b.globalCenter) - sqrDistance > b.length * b.length / 4) {
                return null;
            }
            Vector2d normal = b.globalNormalVector;
            //look here for 1 directional colliders.
            if((a.globalCenter - b.globalCenter).cosineOfAngleBetween(normal) > 0) {
                normal *= -1;
            }
            return new Collision(a,
                b,
                normal,
                a.globalCenter + Sqrt(sqrDistance) * normal,
                a.radius - Sqrt(sqrDistance));
        }

        /// <summary>
        /// Get collision data between two collliders. NOTE, this will not cause a physical collision, if you want to cause a collision use the function "collide()" in the object returned.
        /// </summary>
        /// <param name="a">Any line collider.</param>
        /// <param name="b">Any Circle collider.</param>
        /// <returns>The collision data between the colliders, or null if no collision occured.</returns>
        public static Collision getCollision(LineCollider a, CircleCollider b) {
            return getCollision(b, a);
        }

        /// <summary>
        /// Get collision data between two collliders. NOTE, this will not cause a physical collision, if you want to cause a collision use the function "collide()" in the object returned.
        /// </summary>
        /// <param name="a">Any line collider.</param>
        /// <param name="b">Any line collider.</param>
        /// <returns>The collision data between the colliders, or null if no collision occured.</returns>
        public static Collision getCollision(LineCollider a,LineCollider b) {
            if(Abs(a.globalAngle -b.globalAngle) < 1) {
                return null;
            }
            if((a.length + b.length).sqr() < a.globalCenter.sqrDistance(b.globalCenter)) { //quick check to exclude far away objects
                return null;
            }
            Vector2d aTangent = a.globalTangentVector;
            Vector2d bTangent = b.globalTangentVector;
            double a2 = (a.globalCenter.Y - b.globalCenter.Y + (b.globalCenter.X-a.globalCenter.X) * aTangent.Y / aTangent.X) / (bTangent.Y-bTangent.X*aTangent.Y/aTangent.X);
            Vector2d contact = b.globalCenter + a2 * bTangent;
            if(contact.sqrDistance(a.globalCenter) > a.length.sqr()/4 || contact.sqrDistance(b.globalCenter) > b.length.sqr()/4) {
                return null; //contact is outside of line segment
            }
            double aDistanceFromEndpoint = a.length/2 - contact.distance(a.globalCenter); //The distance of the contact point from a's closest endpoint
            double bDistanceFromEndpoint = b.length/2 - contact.distance(b.globalCenter); //The distance of the contact point from b's closest endpoint
            Vector2d normal = aDistanceFromEndpoint > bDistanceFromEndpoint ? a.globalNormalVector : b.globalNormalVector;
            if(aDistanceFromEndpoint > bDistanceFromEndpoint) {
                normal = a.globalNormalVector;
                if(Vector2d.Dot(normal,contact - b.globalCenter) > 0) {
                    normal *= -1;
                }
            }else {
                normal = b.globalNormalVector;
                if (Vector2d.Dot(normal, contact - a.globalCenter) < 0) {
                    normal *= -1;
                }
            }
            return new Collision(a,
                b,
                normal,
                contact,
                Min(aDistanceFromEndpoint,bDistanceFromEndpoint));
        }
    }
}
