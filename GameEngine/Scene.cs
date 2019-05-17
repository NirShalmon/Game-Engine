using OpenTK;
using System.Drawing;
using System.Collections.Generic;

namespace GameEngine
{
    /// <summary>
    /// A scene will contain all gameobjects and cameras in the scene, and functions to access them.
    /// </summary>
    public class Scene
    {
        static Dictionary<string,Scene> scenes = new Dictionary<string,Scene>();

        /// <summary>
        /// Get all objects in the scene, this includes gameobjects and cameras.
        /// </summary>
        /// <returns>An array of all objects in the scene.</returns>
        public GameObject[] getAllObjects() {
            GameObject[] objs = getAllGameObjects();
            Camera[] cams = getAllCameras();
            GameObject[] total = new GameObject[objs.Length + cams.Length];
            System.Array.Copy(objs,total,objs.Length);
            System.Array.Copy(cams,0,total,objs.Length,cams.Length);
            return total;
        }

        /// <summary>
        /// The name of the current scene.
        /// </summary>
        public static string currentSceneName { set; get; }

        /// <summary>
        /// The current scene.
        /// </summary>
        public static Scene current {
            get {
                try {
                    return scenes[currentSceneName];
                } catch(KeyNotFoundException) {
                    throw new System.Exception("No scene is currently active.");
                }
            }
            set {
                if(!scenes.ContainsKey(value.name)) {
                    addScene(value);
                }
                currentSceneName = value.name;
            }
        }

        /// <summary>
        /// Add a scene to the scene collection.
        /// </summary>
        /// <param name="scene">The scene to add.</param>
        public static void addScene(Scene scene) {
            scenes.Add(scene.name,scene);
        }

        /// <summary>
        /// The name identifier of the current scene.
        /// </summary>
        public readonly string name;

        Dictionary<string,GameObject> objects = new Dictionary<string,GameObject>();

        /// <summary>
        /// Returns all game obects in the scene.
        /// </summary>
        /// <returns>All game obects in the scene.</returns>
        public GameObject[] getAllGameObjects() {
            GameObject[] objectsArray = new GameObject[objects.Count];
            objects.Values.CopyTo(objectsArray,0);
            return objectsArray;
        }

        /// <summary>
        /// Find all objects in a certain layer.
        /// </summary>
        /// <param name="layers">The layers to search for.</param>
        /// <returns>An array of game objects on the given layers.</returns>
        public GameObject[] findObjectsOnLayers(ulong layers) {
            List<GameObject> objectsOnLayer = new List<GameObject>();
            foreach(GameObject o in objects.Values) {
                if((o.layers & layers) == layers) {
                    objectsOnLayer.Add(o);
                }
            }
            return objectsOnLayer.ToArray();
        }


        /// <summary>
        /// Get an object from the scene.
        /// </summary>
        /// <param name="name">The name of the game object to get.</param>
        /// <returns>The game object with the given name.</returns>
        public GameObject getObject(string name) {
            try {
                return objects[name];
            } catch(KeyNotFoundException) {
                throw new System.Exception("Game Object not found.");
            }
        }

        /// <summary>
        /// Add an object to the scene.
        /// </summary>
        /// <typeparam name="T">A type that inherits the GameObject Class</typeparam>
        /// <param name="gameObject">The game object to add to the scene.</param>
        public void addObject<T>(T gameObject) where T : GameObject {
            if(objects.ContainsKey(gameObject.name)) {
                throw new System.Exception("A Game Object with the same name already exists.");
            }
            objects.Add(gameObject.name,gameObject);
        }

        /// <summary>
        /// Remove a game object with a given name from the scene.
        /// </summary>
        /// <param name="name">The name of the object to remove.</param>
        public void removeObject(string name) {
            try {
                objects.Remove(name);
            } catch(KeyNotFoundException) {
                throw new System.Exception("Game Object not found.");
            }
        }

        /// <summary>
        /// Moves all objects a certain amount, this will not have any visable affect.
        /// </summary>
        /// <param name="amount">The amount to move all objects.</param>
        public void moveAll(Vector2d amount) {
            foreach(GameObject o in objects.Values) {
                if(o.parent == null) {
                    o.position += amount;
                }
            }
        }

        Dictionary<string,Camera> cameras = new Dictionary<string,Camera>();
        string CurrentCamera = "";

        /// <summary>
        /// Get and set the current camera, setting a camera that is not in the scene will add it to the scene.
        /// </summary>
        public Camera currentCamera {
            get {
                try {
                    return cameras[CurrentCamera];
                } catch(KeyNotFoundException) {
                    return null;
                }
            }
            set {
                CurrentCamera = value.name;
                if(!cameras.ContainsKey(value.name)) {
                    addCamera(value);
                }
                if(currentCamera.aspectRatio != Game.width / Game.height) {
                    Game.size = new Size(Game.size.Width,(int)(Game.size.Width / cameras[CurrentCamera].aspectRatio));
                }
            }
        }

        /// <summary>
        /// Sets the current camera, setting a camera that is not in the scene will add it to the scene.
        /// </summary>
        /// <param name="camera">The camera to set.</param>
        public void setCurrentCamera(Camera camera) {
            currentCamera = camera;
        }

        /// <summary>
        /// Sets the current camera.
        /// </summary>
        /// <param name="name">The name of the camera to set.</param>
        public void setCurrentCamera(string name) {
            if(cameras.ContainsKey(name)) {
                CurrentCamera = name;
                if(currentCamera.aspectRatio != Game.width / Game.height) {
                    Game.size = new Size(Game.size.Width,(int)(Game.size.Width / cameras[CurrentCamera].aspectRatio));
                }
            } else {
                throw new System.Exception("Camera was not found");
            }
        }

        /// <summary>
        /// Returns all cameras in the scene.
        /// </summary>
        /// <returns>All cameras in the scene.</returns>
        public Camera[] getAllCameras() {
            Camera[] camerasArray = new Camera[cameras.Count];
            cameras.Values.CopyTo(camerasArray,0);
            return camerasArray;
        }

        /// <summary>
        /// Find all cameras in a certain layer.
        /// </summary>
        /// <param name="layers">The layers to search for.</param>
        /// <returns>An array of cameras on the given layers.</returns>
        public Camera[] findCamerasOnLayers(ulong layers) {
            List<Camera> camerasOnLayer = new List<Camera>();
            foreach(Camera o in cameras.Values) {
                if((o.layers & layers) == layers) {
                    camerasOnLayer.Add(o);
                }
            }
            return camerasOnLayer.ToArray();
        }


        /// <summary>
        /// Get a camera from the scene.
        /// </summary>
        /// <param name="name">The name of the camera to get.</param>
        /// <returns>The camera with the given name.</returns>
        public Camera getCamera(string name) {
            try {
                return cameras[name];
            } catch(KeyNotFoundException) {
                throw new System.Exception("Camera not found.");
            }
        }

        /// <summary>
        /// Add a camera to the scene.
        /// </summary>
        /// <param name="camera">The camera to add to the scene.</param>
        public void addCamera(Camera camera) {
            if(cameras.ContainsKey(camera.name)) {
                throw new System.Exception("A camera with the same name already exists.");
            }
            cameras.Add(camera.name,camera);
        }

        /// <summary>
        /// Remove a camera with a given name from the scene.
        /// </summary>
        /// <param name="name">The name of the camera to remove.</param>
        public void removeCamera(string name) {
            try {
                if(CurrentCamera == name) {
                    CurrentCamera = "";
                }
                cameras.Remove(name);
            } catch(KeyNotFoundException) {
                throw new System.Exception("Camera not found.");
            }
        }

        /// <summary>
        /// An empty scene constructor.
        /// </summary>
        public Scene(string name) {
            this.name = name;
        }
    }
}