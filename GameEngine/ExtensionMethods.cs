using OpenTK;
using static System.Math;

namespace GameEngine
{
    /// <summary>
    /// General extension methods to aid development.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The angle a vector forms with the x axis.
        /// </summary>
        /// <param name="vector">The vector to check for an angle.</param>
        /// <returns>The angle of the vector with the x axis</returns>
        public static double angle(this Vector2d vector) {
            return Atan2(vector.Y,vector.X);
        }

        /// <summary>
        /// How many Y units will move every X unit of this vector.
        /// </summary>
        /// <param name="vector">The vector to check for gradient.</param>
        /// <returns>The gradient of this vector.</returns>
        public static double gradient(this Vector2d vector) {
            return vector.X == 0 ? (vector.Y >= 0 ? double.PositiveInfinity : double.NegativeInfinity ): vector.X / vector.Y;
        }

        /// <summary>
        /// Converts an angle with the X axis to the corsponding unit vector.
        /// </summary>
        /// <param name="angle">An angle with the X axis</param>
        /// <returns>A unit vector with an angle with the X axis equel to angle.</returns>
        public static Vector2d toUnitVector(this double angle) {
            return new Vector2d(Cos(angle),Sin(angle));
        }

        /// <summary>
        /// Return a vector rotated by a certain amount of degrees.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="angle">The amount of degrees to rotate it.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2d rotate(this Vector2d vector, double angle) {
            angle += vector.angle();
            double length = vector.Length;
            return new Vector2d(Cos(angle) * length,Sin(angle) * length);
        }

        /// <summary>
        /// Return a vector 90 degrees counterclockwise from the given vector
        /// </summary>
        public static Vector2d rotate90Deg(this Vector2d vector) {
            return new Vector2d(-vector.Y, vector.X);
        }

        /// <summary>
        /// Returns the distance between two positions
        /// </summary>
        /// <param name="position">The first position.</param>
        /// <param name="other">The second position.</param>
        /// <returns>The distance between the positions.</returns>
        public static double distance(this Vector2d position, Vector2d other) {
            return (position - other).Length;
        }

        /// <summary>
        /// Returns the squere distance between two positions0
        /// </summary>
        /// <param name="position">The first position.</param>
        /// <param name="other">The second position.</param>
        /// <returns>The squere distance between the positions.</returns>
        public static double sqrDistance(this Vector2d position,Vector2d other) {
            return (position - other).LengthSquared;
        }

        /// <summary>
        /// Calculate the cosine of an angle between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cosine of the angle between the two vectors.</returns>
        public static double cosineOfAngleBetween(this Vector2d a, Vector2d b) {
            return ((a.LengthSquared + b.LengthSquared) == 0) ? 0 : (a.X * b.X + a.Y * b.Y) / (a.Length * b.Length);
        }

        /// <summary>
        /// Get the length of a vector in a certain direction. The operation is a dot product.
        /// </summary>
        /// <param name="a">The vector to check it's length.</param>
        /// <param name="direction">The normalized direcion vector.</param>
        /// <returns>The length of a vector on a given unit vector's dirction.</returns>
        public static double getLengthInDirection(this Vector2d a,Vector2d direction) {
            return Vector2d.Dot(a,direction);
        }

        /// <summary>
        /// Sets the length of a vector in a certain direction. The operation is opposite of a dot product.
        /// </summary>
        /// <param name="a">The vector to set it's length.</param>
        /// <param name="direction">The normalized direcion vector.</param>
        /// <param name="length">The length to set to the vector.</param>
        public static void setLengthInDirection(this Vector2d a,Vector2d direction,double length) {
             if(a.getLengthInDirection(direction) != 0) {
                a += direction * length / a.getLengthInDirection(direction);
            }
        }

        /// <summary>
        /// The vector cross multiplication operation.
        /// </summary>
        /// <param name="a">Any vector.</param>
        /// <param name="b">Any other vector.</param>
        /// <returns>The cross product of the two vectors(aXb).</returns>
        public static double cross(this Vector2d  a, Vector2d b) {
            return a.X * b.Y - a.Y * b.X;
        }

        /// <summary>
        /// The vector cross multiplication operation.
        /// </summary>
        /// <param name="a">Any vector.</param>
        /// <param name="s">Any scalar.</param>
        /// <returns>The cross product of the vector cross the scalar(aXs).</returns>
        public static Vector2d cross(this Vector2d a, double s) {
            return new Vector2d(s * a.Y,-s * a.X);
        }

        /// <summary>
        /// The vector cross multiplication operation.
        /// </summary>
        /// <param name="a">Any vector.</param>
        /// <param name="s">Any scalar.</param>
        /// <returns>The cross product of the vector cross the scalar(sXa).</returns>
        public static Vector2d cross(this double s,Vector2d a) {
            return new Vector2d(-s * a.Y,s * a.X);
        }

        /// <summary>
        /// Computes the angle between two vectors, in radians.
        /// </summary>
        public static Angle angleBetween(this Vector2d a, Vector2d b) {
            return Acos(Vector2d.Dot(a,b) / (a.Length * b.Length));
        }

        /// <summary>
        /// Computes the square of a double
        /// </summary>
        public static double sqr(this double a) => a * a;

        /// <summary>
        /// returns min &lt; a &lt; max
        /// </summary>
        public static bool isInRange(this double a, double min, double max) =>  a<max && a> min;

        /// <summary>
        /// return -abs &lt; a &lt; abs
        /// </summary>
        public static bool inSymetricRange(this double a,double abs) => a < abs && a > -abs;
    }
}
