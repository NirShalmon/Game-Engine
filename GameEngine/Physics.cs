using static System.Math;
using System.Collections.Generic;
using OpenTK;

namespace GameEngine
{
    /// <summary>
    /// Class of all physics data and calculation.
    /// </summary>
    public class Physics : Component {
        bool UsePhysics;

        /// <summary>
        /// Should physics act on this object?
        /// </summary>
        public bool usePhysics {
            get {
                return UsePhysics;
            }
            set {
                if(value) {
                    if(!UsePhysics) {
                        Game.physicsUpdate += update;
                        UsePhysics = true;
                    }
                } else {
                    if(UsePhysics) {
                        Game.physicsUpdate -= update;
                        UsePhysics = false;
                    }
                }
            }
        }

        /// <summary>
        /// The local rate of change in position of the object. Measured in units per second.
        /// </summary>
        public Vector2d velocity { set; get; }

        /// <summary>
        /// The rate of change in position of the object in global space, measured in units per second.
        /// </summary>
        public Vector2d localVelocity {
            get {
                return velocity - (gameObject.parent == null ? Vector2d.Zero : gameObject.parent.physics.velocity);
            }
            set {
                velocity = value + (gameObject.parent == null ? Vector2d.Zero : gameObject.parent.physics.velocity);
            }
        }

        /// <summary>
        /// The rate of change in angle, in local space. Measured in radians per second.
        /// </summary>
        public double localAngularSpeed { set; get; }

        /// <summary>
        /// The rate of change in angle, in radians per second.
        /// </summary>
        public double angularSpeed {
            get {
                return localAngularSpeed + (gameObject.parent == null ? 0 : gameObject.parent.physics.angularSpeed);
            }
            set {
                localAngularSpeed = value - (gameObject.parent == null ? 0 : gameObject.parent.physics.angularSpeed);
            }
        }

        /// <summary>
        /// The constant acceleration of the body, measured in unity/seconds^2.
        /// </summary>
        public Vector2d gravity { set; get; }

        /// <summary>
        /// The colliders of this object. Colliders are responsible for detecting and acting in result of collisions.
        /// </summary>
        public List<Collider> colliders { set; get; } = new List<Collider>();

        double Mass;

        double InverseMass;

        /// <summary>
        /// The mass of this object, this does not include the mass of children objects.
        /// </summary>
        public double mass
        {
            set
            {
                Mass = value;
                InverseMass = 1 / value;
            }
            get
            {
                return Mass;
            }
        }

        /// <summary>
        /// One divided by the mass of the object, useful to speed up some calculations.
        /// </summary>
        public double inverseMass
        {
            set
            {
                InverseMass = value;
                Mass = 1 / value;
            }
            get
            {
                return InverseMass;
            }
        }

        /// <summary>
        /// The moment of inertia of the object, higher values will require a stronger force to change angular speed.
        /// </summary>
        public double momentOfInertia { set; get; } = 5;

        /// <summary>
        /// A coefficient for the drag, higher values will result in more drag, negative values will result in opposite drag.
        /// </summary>
        public double coefficiantOfDrag { set; get; }

        /// <summary>
        /// The power the velocity would be raised to to calculate drag. Two is the realistic value.
        /// </summary>
        public double dragFunction { set; get; }

        /// <summary>
        /// The type of drag to be applied on the object.
        /// </summary>
        public DragType dragType { set; get; }

        /// <summary>
        /// The vertecies of the drag mesh. Only used with mesh drag mode.
        /// </summary>
        public Vector2d[] dragMeshVertecies { set; get; } = new Vector2d[0];

        /// <summary>
        /// The mass of this object and all of it's decendents.
        /// </summary>
        public double totalMass {
            get {
                double temp = usePhysics ? mass : 0;
                foreach(GameObject o in gameObject.children) {
                    temp += o.physics.totalMass;
                }
                return temp;
            }
        }

        /// <summary>
        /// Apply drag on the object based on the related properties.
        /// </summary>
        protected virtual void applyDrag() {
            Vector2d airodynamicCenter = Vector2d.Zero;
            double speed = velocity.Length;
            Vector2d vectorDragCoefficiant = -coefficiantOfDrag * (speed == 0 ? Vector2d.Zero : velocity / speed);
            switch(dragType) {
                case DragType.Mesh:
                    double width = 0;
                    for(int i = 0; i < dragMeshVertecies.Length; i++) {
                        Vector2d vect = dragMeshVertecies[i].rotate(gameObject.localAngle) - dragMeshVertecies[(i + 1) % dragMeshVertecies.Length].rotate(gameObject.localAngle);
                        double value = vect.Length * Abs(localVelocity.cosineOfAngleBetween(vect));
                        if(value > width) {
                            airodynamicCenter = (dragMeshVertecies[i].rotate(gameObject.localAngle) + dragMeshVertecies[(i + 1) % dragMeshVertecies.Length].rotate(gameObject.localAngle)) / 2;
                            width = value;
                        }
                    }
                    vectorDragCoefficiant *= width;
                    break;
            }
            //System.Diagnostics.Debug.Write(speed + " " + vectorDragCoefficiant.ToString() + " " + gameObject.position.ToString() + " " + gameObject.layers + " " + Camera.current.position.ToString() + "\n");
            if(speed > 0) {
                applyForceAtPosition(vectorDragCoefficiant * Pow(speed,dragFunction),airodynamicCenter,Space.global);
            }

        }

        /// <summary>
        /// The velocity of the object at a (possibly offcenter) point. This takes rotation into account
        /// </summary>
        public Vector2d getVelocityAtPoint(Vector2d point, Space space = Space.global) {
            if(space == Space.global) {
                Vector2d pointRelative = point - gameObject.position;
                Vector2d tangent = pointRelative.rotate90Deg().Normalized();
                return velocity + angularSpeed * tangent * pointRelative.Length;
            }else {
                Vector2d tangent = point.rotate90Deg();
                return localVelocity + localAngularSpeed * tangent * point.Length;
            }
        } 

        /// <summary>
        /// Applies a force to the object, this will change it's velocity.
        /// </summary>
        /// <param name="force">The force to apply on the object.</param>
        /// <param name="space">Are we working in global or local coordinates</param>
        public virtual void applyForce(Vector2d force, Space space = Space.global) {
            if(space == Space.local) {
                localVelocity += force / totalMass;
            } else {
                velocity += force / totalMass;
            }
        }

        /// <summary>
        /// Apply a force of a certain magnitude in a normalized direction vector.
        /// </summary>
        /// <param name="force">The magnitude of the force to be applied.</param>
        /// <param name="direction">A normalized direction to apply the force at.</param>
        /// <param name="space">Are we working in global or local coordinates</param>
        public virtual void applyForceInDirection(double force,Vector2d direction, Space space = Space.global) {
            applyForce(direction * force,space);
        }

        /// <summary>
        /// Apply a torque to an object, this will change it's angular speed.
        /// </summary>
        /// <param name="torque"></param>
        public virtual void applyTorque(double torque) {
             localAngularSpeed += torque / momentOfInertia;
        }

        /// <summary>
        /// Apply a force on the object at a local position, this affects it's velocity and anglular speed.
        /// </summary>
        /// <param name="force">The force vector to apply.</param>
        /// <param name="position">The local position to apply the force at.</param>
        /// <param name="space">Are we working in global or local coordinates</param>
        public virtual void applyForceAtPosition(Vector2d force,Vector2d position, Space space = Space.local) {
            applyForce(force,space);
            if(space == Space.global) {
                position -= gameObject.position;
            }
            applyTorque(position.cross(force));
        }

        /// <summary>
        /// Was physics calculated for this object this physics frame?
        /// </summary>
        public bool calculatedThisFrame { set; get; }

        internal Physics(GameObject gameObject) : base(gameObject) {
            Game.physicsUpdate += update;
            mass = 1;
        }

        /// <summary>
        /// An physics update function to be called on the physics event every physics frame.
        /// </summary>
        /// <param name="frame">The serial number of the frame.</param>
        protected virtual void update(ulong frame) {
            if(!usePhysics) {
                return;
            }
            calculatedThisFrame = true;
            applyForce(gravity / 60);
            gameObject.localPosition += localVelocity / 60;
            gameObject.localAngle += localAngularSpeed / 60;
            applyDrag();
        }

        internal static void collisions() {
            GameObject[] objects = Scene.current.getAllObjects();
            for(int i = 0; i < objects.Length; i++) {
                if(objects[i].physics.usePhysics) {
                    for(int j = i + 1; j < objects.Length; j++) {
                        if(objects[j].physics.usePhysics && (objects[j].layers & objects[i].layers) != 0) {
                            foreach(dynamic a in objects[i].physics.colliders) {
                                foreach(dynamic b in objects[j].physics.colliders) {
                                    Collision collision = CollisionDetectors.getCollision(a, b);
                                    if (collision != null) {
                                        collision.collide();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// How should the drag be modeled?.
    /// </summary>
    public enum DragType
    {
        /// <summary>
        /// Equal drag in all directions, this won't affect rotation, and should be used for spheres or when realistic drag isn't needed.
        /// </summary>
        Omnidirectional,
        /// <summary>
        /// Drag wil be calculated based on a mesh, use when realistic drag is required. Often a triangle will be sufficiant.
        /// </summary>
        Mesh
    }

    /// <summary>
    /// Used to identify the coordinate system used.
    /// </summary>
    public enum Space
    {
        /// <summary>
        /// The global coordinate system - Screen x and y axies.
        /// </summary>
        global,
        /// <summary>
        /// The local coordinate system for some object - Axies direction is derived from the up and right attributes of the object.
        /// The object is located at the (0,0) coordinate.
        /// </summary>
        local
    }
}