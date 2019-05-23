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
        public static Collision getContact(LineCollider a,LineCollider b) {
            double aGradient = a.globalGradient;
            double bGradient = b.globalGradient;
            if(aGradient == bGradient) {
                return null;
            }
            double xColl = aGradient * a.globalCenter.X - bGradient * b.globalCenter.X + b.globalCenter.Y - a.globalCenter.Y;
            xColl /= aGradient - bGradient;
            double dx = xColl - a.globalCenter.X;
            double tipA = a.length * a.length / 4 - dx * dx * (aGradient * aGradient + 1);
            if(tipA < 0) {
                return null;
            }
            dx = xColl - b.globalCenter.X;
            double tipB = b.length * b.length / 4 - dx * dx * (bGradient * bGradient + 1);
            if(tipB < 0) {
                return null;
            }
            Vector2d normal = tipA < tipB ? b.globalNormalVector : a.globalNormalVector;
            if(Vector2d.Dot(a.globalCenter,normal) < 0) {
                normal *= -1;
            }
            return new Collision(a,
                b,
                normal,
                b.globalCenter + new Vector2d(dx,bGradient * dx),
                Sqrt(Min(tipA,tipB)));
        }
    }
}
