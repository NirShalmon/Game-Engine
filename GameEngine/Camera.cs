using System.Collections.Generic;
using System.Drawing;
using OpenTK;

namespace GameEngine
{

    /// <summary>
    /// A camera object, used as a renderer for the scene.
    /// </summary>
    public class Camera : GameObject
    {

        /// <summary>
        /// The currently active camera.
        /// </summary>
        public static Camera current {
            get {
                return Scene.current.currentCamera;
            }
            set {
                Scene.current.currentCamera = value;
            }
        }

        /// <summary>
        /// This color wil be shown where no sprite is rendered.
        /// </summary>
        public Color backgroundColor { set; get; }

        /// <summary>
        /// The width / height of the camera.
        /// </summary>
        public double aspectRatio { set; get; }

        /// <summary>
        /// A constructor for a camera.
        /// </summary>
        /// <param name="name">The name of the camera.</param>
        /// <param name="aspectRatio">The aspect ratio of the camera.</param>
        /// <param name="position">The position of the camera.</param>
        /// <param name="scale">The scale of the camera's FOV.</param>
        /// <param name="angle">The angle of this camera with the X axis.</param>
        /// <param name="layers">The layers that this camera can render.</param>
        /// <param name="backgroundColor">The color displayed when no other object is rendered.</param>
        public Camera(string name,double aspectRatio,Vector2d position,Vector2d scale,double angle,ulong layers,Color backgroundColor) : base(name,new Texture(),position,scale,angle,layers) {
            this.aspectRatio = aspectRatio;
            this.position = position;
            this.scale = scale;
            this.angle = angle;
            this.layers = layers;
            this.backgroundColor = backgroundColor;
        }

        /// <summary>
        /// Remove this camera from the current scene.
        /// </summary>
        public override void remove() {
            if(parent != null) {
                parent.removeChild(this);
            }
            Scene.current.removeCamera(name);
        }

        //public void remove(int milliseconds)

        /// <summary>
        /// Add this camera to a scene.
        /// </summary>
        /// <param name="scene">The scene to add to.</param>
        public override void addToScene(Scene scene) {
            scene.addCamera(this);
        }

        /// <summary>
        /// Returns a string containing inforation about the camera.
        /// </summary>
        /// <returns>A string containing inforation about the camera.</returns>
        public override string ToString() {
            return $"{base.ToString()}, {aspectRatio}";
        }
    }
}