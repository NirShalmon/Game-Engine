using OpenTK;
using System.Collections.Generic;
using static System.Math;

namespace GameEngine
{
    /// <summary>
    /// The base class for in game objects.
    /// </summary>
    public class GameObject
    {

        /// <summary>
        /// The name of the object.
        /// </summary>
        public readonly string name;

        /// <summary>
        /// The layers of this gameobject, each bit represents a layer, an object could be on multiple layers.
        /// </summary>
        public ulong layers { set; get; }

        /// <summary>
        /// The image to be drawn on the object.
        /// </summary>
        public Texture sprite { set; get; }

        /// <summary>
        /// The position vector of the object in local space.
        /// </summary>
        public Vector2d localPosition { set; get; }

        /// <summary>
        /// The local scale of the object.
        /// </summary>
        public Vector2d localScale { set; get; }

        /// <summary>
        /// The physics object of this object.
        /// </summary>
        public Physics physics { get; set; }

        List<GameObject> Children;

        /// <summary>
        /// The local angle of the object, in radians.
        /// </summary>
        public Angle localAngle { set; get; }

        GameObject Parent;

        /// <summary>
        /// The parent object of this object, set to null if non.
        /// </summary>
        public GameObject parent {
            get {
                return Parent;
            }
            set {
                if(parent != null) {
                    parent.Children.Remove(this);
                }
                localPosition = position - (value == null ? Vector2d.Zero : value.position);
                physics.localVelocity = physics.velocity - (value == null ? Vector2d.Zero : value.physics.velocity);
                localScale = scale - (value == null ? Vector2d.Zero : value.scale);
                localAngle = angle - (value == null ? 0 : value.angle);
                physics.localAngularSpeed = physics.angularSpeed - (value == null ? 0 : value.physics.localAngularSpeed);
                Parent = value;
                if(parent != null) {
                    parent.Children.Add(this);
                }
            }
        }

        /// <summary>
        /// The position of the object in global space.
        /// </summary>
        public Vector2d position {
            get {
                return localPosition.rotate(parent == null ? 0 : parent.angle) + (parent == null ? Vector2d.Zero : parent.position);
            }
            set {
                localPosition = value - (parent == null ? Vector2d.Zero : parent.position);
            }
        }

        /// <summary>
        /// The scale of the object in global space.
        /// </summary>
        public Vector2d scale {
            get {
                return localScale + (parent == null ? Vector2d.Zero : parent.scale);
            }
            set {
                localScale = value - (parent == null ? Vector2d.Zero : parent.scale);
            }
        }

        /// <summary>
        /// The angle of the object in global space.
        /// </summary>
        public Angle angle {
            get {
                return localAngle + (parent == null ? 0 : parent.angle);
            }
            set {
                localAngle = value - (parent == null ? 0 : parent.angle);
            }
        }

        /// <summary>
        /// An empty constructor for gameobjects.
        /// </summary>
        public GameObject() {
            Children = new List<GameObject>();
        }

        /// <summary>
        /// A full constructor to a game object.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="sprite">The image used to display the image.</param>
        /// <param name="position">The position of the object.</param>
        /// <param name="scale">The scale of the object.</param>
        /// <param name="angle">The angle of the object with the X axis.</param>
        /// <param name="layers">The layers of the object.</param>
        public GameObject(string name,Texture sprite,Vector2d position,Vector2d scale,double angle,ulong layers) {
            this.name = name;
            this.sprite = sprite;
            this.position = position;
            physics = new Physics(this);
            this.scale = scale;
            this.angle = angle;
            this.layers = layers;
            Children = new List<GameObject>();
        }

        /// <summary>
        /// The unit vector pointing right from the object
        /// </summary>
        public Vector2d right {
            get {
                return new Vector2d(Cos(angle),Sin(angle));
            }
            set {
                angle = Atan2(value.Y,value.X) - MathHelper.PiOver2;
            }
        }

        /// <summary>
        /// The unit vector pointing up from the object
        /// </summary>
        public Vector2d up {
            get {
                return new Vector2d(-Sin(angle),Cos(angle));
            }
            set {
                angle = Atan2(value.Y,value.X) - MathHelper.PiOver2;
            }
        }

        /// <summary>
        /// The unit vector pointing right from the object localy.
        /// </summary>
        public Vector2d localRight {
            get {
                return new Vector2d(Cos(localAngle),Sin(localAngle));
            }
            set {
                localAngle = Atan2(value.Y,value.X) - MathHelper.PiOver2;
            }
        }

        /// <summary>
        /// The unit vector pointing up from the object localy.
        /// </summary>
        public Vector2d localUp {
            get {
                return new Vector2d(-Sin(localAngle),Cos(localAngle));
            }
            set {
                localAngle = Atan2(value.Y,value.X) - MathHelper.PiOver2;
            }
        }

        /// <summary>
        /// How many children does this object have?
        /// </summary>
        public int childCount {
            get {
                return Children.Count;
            }
        }

        /// <summary>
        /// How many descendants does this object have?
        /// </summary>
        public int descendantCount {
            get {
                int sum = 0;
                foreach(GameObject o in Children) {
                    sum += o.descendantCount;
                }
                return sum;
            }
        }

        /// <summary>
        /// An array of this object's children.
        /// </summary>
        public GameObject[] children {
            get {
                return Children.ToArray();
            }
        }

        /// <summary>
        /// Converts an angle from this local space to global space.
        /// </summary>
        /// <param name="angle">The angle to convert.</param>
        /// <returns>The converted angle</returns>
        public double localToGlobal(double angle) {
            return angle + this.angle;
        }

        /// <summary>
        /// Converts an angle from global space to this local space.
        /// </summary>
        /// <param name="angle">The angle to convert.</param>
        /// <returns>The converted angle</returns>
        public double globalToLocal(double angle) {
            return angle - this.angle;
        }

        /// <summary>
        /// Converts a vector from this local space to global space.
        /// </summary>
        /// <param name="vector">The vector to convert</param>
        /// <returns>The converted vector.</returns>
        public Vector2d localVectorToGlobal(Vector2d vector) {
            double length = vector.Length;
            double angle = localToGlobal(vector.angle());
            return new Vector2d(Cos(angle) * length,Sin(angle) * length);
        }

        /// <summary>
        /// Converts a vector from global space to this local space.
        /// </summary>
        /// <param name="vector">The vector to convert</param>
        /// <returns>The converted vector.</returns>
        public Vector2d globalVectorToLocal(Vector2d vector) {
            double length = vector.Length;
            double angle = globalToLocal(vector.angle());
            return new Vector2d(Cos(angle) * length,Sin(angle) * length);
        }

        /// <summary>
        /// Converts a position from this local space to global space.
        /// </summary>
        /// <param name="position">The position to convert.</param>
        /// <returns>The converted position.</returns>
        public Vector2d localPositionToGlobal(Vector2d position) {
            return  localVectorToGlobal(position) + this.position;
        }

        /// <summary>
        /// Converts a position from global space to this local space.
        /// </summary>
        /// <param name="position">The position to convert.</param>
        /// <returns>The converted position.</returns>
        public Vector2d globalPositionToLocal(Vector2d position) {
            return globalVectorToLocal(position - this.position);
        }

        /// <summary>
        /// Sets the right of the oject to look at a position.
        /// </summary>
        /// <param name="position">The position to look at.</param>
        public void lookAt(Vector2d position) {
            if(this.position != position) {
                right = position - this.position;
            }
        }

        /// <summary>
        /// Sets the right of an object to look at another object.
        /// </summary>
        /// <param name="other">The object to look at.</param>
        public void lookAt(GameObject other) {
            if(other.position != position) {
                right = position - other.position;
            }
        }

        /// <summary>
        /// Remove this object from the current scene.
        /// </summary>
        public virtual void remove() {
            if(parent != null) {
                parent.Children.Remove(this);
            }
            Scene.current.removeObject(name);
        }

        internal void removeChild(GameObject child) {
            Children.Remove(child);
        }

        /// <summary>
        /// Is an object a child of this object?
        /// </summary>
        /// <param name="other">The object to check </param>
        /// <returns>True if other is a child of this game object.</returns>
        public bool isChild(GameObject other) {
            try {
                return other.parent.name == name;
            } catch(System.NullReferenceException) {
                return false;
            }
        }

        /// <summary>
        /// Is an object Descendant of this object?
        /// </summary>
        /// <param name="other">The object to check </param>
        /// <returns>True if other is descendant of this game object.</returns>
        public bool isDescendant(GameObject other) {
            while(other.parent != null) {
                if(other.parent.name == name) {
                    return true;
                }
                other = other.parent;
            }
            return false;
        }

        //public virtual void remove(int milliseconds)

        /// <summary>
        /// Add this object to a scene.
        /// </summary>
        /// <param name="scene">The scene to add to.</param>
        public virtual void addToScene(Scene scene) {
            scene.addObject(this);
        }

        /// <summary>
        /// Generates a string containing data about the object.
        /// </summary>
        /// <returns>A string containing data about the object.</returns>
        public override string ToString() {
            return $"{name}, {position.ToString()}";
        }
    }
}