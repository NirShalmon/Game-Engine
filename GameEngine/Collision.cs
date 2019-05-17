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
        public readonly Physics a;

        /// <summary>
        /// The second collider in the collision.
        /// </summary>
        public readonly Physics b;

        public List<Contact> contacts;

        /// <summary>
        /// The relative velocity vector of the collision.
        /// </summary>
        public readonly Vector2d relativeVelocity;

        /// <summary>
        /// The bounce coefficient of the collision.
        /// </summary>
        public readonly double restetution;

        /// <summary>
        /// The bounce coefficient of the collision.
        /// </summary>
        public readonly double staticFriction;

        /// <summary>
        /// The bounce coefficient of the collision.
        /// </summary>
        public readonly double dynamicFriction;
        
        public Vector2d penetration { private set; get; }

        /// <summary>
        /// A collision constructor filling in all of the collision data.
        /// </summary>
        /// <param name="a">The first collider in the collision.</param>
        /// <param name="b">The second collider in the collision.</param>
        /// <param name="normal">The vector perpendicular to the collision.</param>
        /// <param name="contact">The point where the two colliders touched</param>
        /// <param name="restetution">The bounce coefficient of the collision</param>
        /// <param name="penetrationDepth">How deep are the objects penetrating into each other</param>
        public Collision(Physics a,Physics b) {
            this.a = a;
            this.b = b;
            relativeVelocity = b.physics.velocity- a.physics.velocity;
            restetution = Min(a.bounciness,b.bounciness);
            staticFriction = (a.staticFriction + b.staticFriction) / 2;
            dynamicFriction = (a.dynamicFriction + b.dynamicFriction) / 2;
        }

        public void addContact(Vector2d point, Vector2d normal, double penetrationDepth) {
            penetration += normal * penetrationDepth;
            contacts.Add(new Contact(point,normal,relativeVelocity + b.physics.angularSpeed.cross(point - b.gameObject.position) - a.physics.angularSpeed.cross(point - a.gameObject.position)));
        }

        /// <summary>
        /// Invoke the collision, this will make the physics engine take the collision into effect.
        /// </summary>
        public void collide() {
            foreach(Contact contact in contacts) {
                if(contact.relativeSpeedAlongNormal > 0) {
                    return;
                    //Trigger.
                }
                //Collision:
                double j = -contact.relativeSpeedAlongNormal * (1 + restetution);
                double devisor = 1 / a.physics.totalMass + 1 / b.physics.totalMass + ((contact.point - a.gameObject.position).cross(contact.normal)).sqr() / b.physics.momentOfInertia + ((contact.point - b.gameObject.position).cross(contact.normal)).sqr() / b.physics.momentOfInertia;
                j /= devisor;
                Vector2d impulse = j * contact.normal / 2;

                //friction:
                double jt = -contact.relativeSpeedAlongTangent / devisor / 2;

                if(contact.relativeSpeedAlongTangent.inSymetricRange(Abs(contact.relativeSpeedAlongNormal * (a.staticFriction + b.staticFriction)))) {
                    impulse += ((jt / a.physics.totalMass) + (jt / b.physics.totalMass)) * contact.tangent;
                    //System.Diagnostics.Debug.Write("static" + jt * tangent + "\n");
                } else {
                    impulse += -j * (a.dynamicFriction + b.dynamicFriction) / 2 * contact.tangent;
                    //System.Diagnostics.Debug.Write("dynamic" + a.gameObject.position.Y + "\n");
                }
                a.physics.applyForceAtPosition(-impulse,contact.point,Space.global);
                b.physics.applyForceAtPosition(impulse,contact.point,Space.global);
                System.Diagnostics.Debug.Write(contact);
                pushApart();
            }
        }

        void pushApart() {
            const double precent = 0.05;
            const double slop = 0.04;
            Vector2d correction = penetration * precent / (1 / a.physics.totalMass + 1 / b.physics.totalMass);
            a.gameObject.position -= correction / a.physics.totalMass;
            b.gameObject.position += correction / b.physics.totalMass;
          //  Physics.getConatct((a.GetType().GetMethod(,).Invoke(a,b);
        }
    }
}
