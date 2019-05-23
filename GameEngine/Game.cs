using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GameEngine
{
    /// <summary>
    /// Base class of the game, includes static fields and methods to controll the game.
    /// </summary>
    public class Game : GameWindow
    {
        static ulong frameCount = 0;

        static Game game;
        static long lastFrameTime;
        static Stopwatch watch;

        /// <summary>
        /// Is the game paused?
        /// </summary>
        public static bool isPaused { set; get; } = false;

        /// <summary>
        /// A delegate for a metod containing a ulong parameter.
        /// </summary>
        /// <param name="num"></param>
        public delegate void number(ulong num);

        /// <summary>
        /// An event that calls every frame.
        /// </summary>
        public static event number update;

        /// <summary>
        /// An event that calls every physics update.
        /// </summary>
        public static event number physicsUpdate;

        /// <summary>
        /// Should the game display rendering time.
        /// </summary>
        public static bool displayRenderTime { set; get; }

        /// <summary>
        /// The width of the game rendering area.
        /// </summary>
        public static int width {
            get {
                return game.ClientSize.Width;
            }
            set {
                game.ClientSize = new Size(value,value * game.Height / game.Width);
                GL.Viewport(new Size(value,value * game.Height / game.Width));
            }
        }

        /// <summary>
        /// The height of the game rendering area.
        /// </summary>
        public static int height {
            get {
                return game.ClientSize.Height;
            }
            set {
                game.ClientSize = new Size(value * game.Width / game.Height,value);
                GL.Viewport(game.ClientSize);
            }
        }

        /// <summary>
        /// Gets and sets the title of the game, this text will be showed at the top border.
        /// </summary>
        public static string title {
            get {
                return game.Title;
            }
            set {
                game.Title = value;
            }
        }

        /// <summary>
        /// Gets and sets the icon of the game, this text will be showed at the top border.
        /// </summary>
        public static Icon icon {
            get {
                return game.Icon;
            }
            set {
                game.Icon = value;
            }
        }

        /// <summary>
        /// Gets the size of the game window.
        /// </summary>
        public static Size size {
            get {
                return game.ClientSize;
            }
            internal set {
                game.ClientSize = value;
                GL.Viewport(game.ClientSize);
            }
        }

        /// <summary>
        /// Converts a position in global space to a point on the screen in pixels.
        /// </summary>
        /// <param name="position">The position to convert.</param>
        /// <returns>A position in pixels.</returns>
        public static Point worldToScreenPoint(Vector2d position) {
            Vector2d temp = (position - Camera.current.position);
            temp.X /= Camera.current.scale.X * 2;
            temp.Y /= Camera.current.scale.Y * 2;
            return new Point((int)Math.Round( width / 2 + temp.X * width),(int)Math.Round(height / 2 + temp.Y * height));
        }

        /// <summary>
        /// Stop the game time and physics.              TOFIX
        /// </summary>
        public static void pause() {
            
            isPaused = true;
            //onPause event.                               TODO
        }

        /*public static void pause(int milliseconds) {      TODO
            game.TargetUpdateFrequency = 0;
            game.TargetRenderFrequency = 0;
            isPaused = true;
        }*/

        /// <summary>
        /// Resumes the game time and physics.               TODO
        /// </summary>
        public static void unPause() {
            isPaused = false;
            //onUnpaused event.
        }

        /// <summary>
        /// Close the application.
        /// </summary>
        public static void quit() {
            //onQuit event.                             TODO
            game.Close();
        }

        /*public static void quit(int milliseconds) {   TODO
            //onQuit event.
            game.Close();
        }*/

        /// <summary>
        /// Create a new game.
        /// </summary>
        /// <param name="width">The initial width of the game, this can be changed later.</param>
        /// <param name="isFullscreen">Will the game be fullscreen, this can be changed later.</param>
        public Game(int width,bool isFullscreen) : base(width,width,new GraphicsMode(32,24,0,16),"Game Engine",isFullscreen ? GameWindowFlags.Fullscreen : GameWindowFlags.FixedWindow) {
            game = this;
           
            UpdateFrame += OnUpdateFrame;
            Load += OnLoad;
            RenderFrame += OnRenderFrame;
        }

        internal void OnLoad(object sender,EventArgs e) {
            game.VSync = VSyncMode.Adaptive;
            watch = new Stopwatch();
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
            Program.main();
        }

        internal void OnUpdateFrame(object sender, FrameEventArgs e) {
            if(isPaused) {
                return;
            }
            if(physicsUpdate != null) {
                GameObject[] obj = Scene.current.getAllObjects();
                foreach(GameObject o in obj) {
                    o.physics.calculatedThisFrame = false;
                }
                physicsUpdate(frameCount);
                Physics.collisions();
            }
        }

        internal void OnRenderFrame(object sendr,FrameEventArgs e) {
            update?.Invoke(frameCount);
            if (displayRenderTime) {
                watch.Start();
            }
            if(Camera.current != null) {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.ClearColor(Camera.current.backgroundColor);
                if(displayRenderTime) {
                   // Graphics g = Graphics.fro   TODO
                }
                for(int l = 63; l >= 0; l--) {
                    if((Camera.current.layers & (1ul << l)) != 0) {
                        GameObject[] onLayer = Scene.current.findObjectsOnLayers(1ul << l);
                        foreach(GameObject o in onLayer) {
                            if((o.layers & Camera.current.layers) << (63 - l) == 1ul << 63) {
                                Vector2d up = o.up;
                                Vector2d right = o.right;
                                Vector2d relativePosition = Camera.current.position - o.position;

                                GL.BindTexture(TextureTarget.Texture2D,o.sprite.id);
                                GL.Begin(PrimitiveType.Quads);

                                GL.TexCoord2(new Vector2(0,1));
                                GL.Vertex2(- new Vector2d((relativePosition.X + up.X * o.scale.Y / 2 - right.X * o.scale.X / 2) / Camera.current.scale.X,(relativePosition.Y + up.Y * o.scale.Y / 2 - right.Y * o.scale.X / 2) / Camera.current.scale.Y));

                                GL.TexCoord2(new Vector2(1,1));
                                GL.Vertex2(- new Vector2d((relativePosition.X + up.X * o.scale.Y / 2 + right.X * o.scale.X / 2) / Camera.current.scale.X,(relativePosition.Y + up.Y * o.scale.Y / 2 + right.Y * o.scale.X / 2) / Camera.current.scale.Y));

                                GL.TexCoord2(new Vector2(1,0));
                                GL.Vertex2(- new Vector2d((relativePosition.X - up.X * o.scale.Y / 2 + right.X * o.scale.X / 2)  / Camera.current.scale.X,(relativePosition.Y - up.Y * o.scale.Y / 2 + right.Y * o.scale.X / 2) / Camera.current.scale.Y));

                                GL.TexCoord2(new Vector2(0,0));
                                GL.Vertex2(- new Vector2d((relativePosition.X - up.X * o.scale.Y / 2 - right.X * o.scale.X / 2) / Camera.current.scale.X,(relativePosition.Y - up.Y * o.scale.Y / 2 - right.Y * o.scale.X / 2) / Camera.current.scale.Y));

                                GL.End();
                            }
                        }
                    }
                }
            } else {
                GL.Clear(ClearBufferMask.ColorBufferBit);
            }
            SwapBuffers();
            if(displayRenderTime) {
                lastFrameTime = watch.ElapsedMilliseconds;
                watch.Reset();
            }
            ++frameCount;
        }
    }
}
