﻿using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace GameEngine
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            new Game(200,false).Run();
        }

        public static void main() {
            Game.displayRenderTime = true;
            Game.width = 800;
            Scene scene = new Scene("test");
            Scene.current = scene;
            scene.addCamera(new Camera("camera",1,new Vector2d(0,0),new Vector2d(10,10),0,ulong.MaxValue,Color.Wheat));
            scene.setCurrentCamera("camera");
            Texture texture = new Texture(@"C:\Users\Nir Shalmon\Documents\Visual Studio 2015\Projects\GameEngine\GameEngine\sprites\circle.png");
            Texture other = new Texture(@"C:\Users\Nir Shalmon\Documents\Visual Studio 2015\Projects\GameEngine\GameEngine\sprites\arrow.png");
            scene.addObject(new Thing("box",texture,30,new Vector2d(0,0),new Vector2d(0,0),true));
            scene.addObject(new Thing("box2",texture,30,new Vector2d(5,0),new Vector2d(0,0),false));
            scene.addObject(new Thong("floor",other,2,-8 * Vector2d.UnitY,0));
            scene.addObject(new Thong("celing",other,4,9.5 * Vector2d.UnitY,Angle.pi));
            scene.addObject(new Thong("right",other,8,9.5 * Vector2d.UnitX,Angle.piOver2));
            scene.addObject(new Thong("left",other,16,-9.5 * Vector2d.UnitX,Angle.piOver2));
        }
    }


    internal class Thing : GameObject
    {
        bool a;
        public Thing(string name,Texture texture,ulong layers,Vector2d position,Vector2d velocity,bool a) : base(name,texture,position,Vector2d.One * 3,0,layers) {
            Game.update += update;
            physics.dragFunction = 2;
            physics.dragType = DragType.Omnidirectional;
            physics.coefficiantOfDrag = 0;
            physics.usePhysics = true;
            physics.mass = 1;
            physics.circleColliders.Add(new CircleCollider(1.5,Vector2d.Zero,physics));
           /* physics.lineColliders.Add(new LineCollider(physics,Vector2d.UnitY * 1.5,3,0,0));
            physics.lineColliders.Add(new LineCollider(physics,Vector2d.UnitY *- 1.5,3,0,0));
            physics.lineColliders.Add(new LineCollider(physics,Vector2d.UnitX * 1.5,3,Angle.piOver2,0));
            physics.lineColliders.Add(new LineCollider(physics,Vector2d.UnitX * -1.5,3,Angle.piOver2,0));*/
            physics.velocity = velocity;
            this.a = a;
        }

        void update(ulong frame) {
            //System.Windows.Forms.MessageBox.Show(position.Y.ToString());
            if(a) {
                if(Keyboard.GetState().IsKeyDown(Key.D)) {
                    physics.applyForce(Vector2d.UnitX / 6);
                }
                if(Keyboard.GetState().IsKeyDown(Key.A)) {
                    physics.applyForce(-Vector2d.UnitX / 6);
                }

                if(Keyboard.GetState().IsKeyDown(Key.W)) {
                    physics.applyForce(Vector2d.UnitY / 4);
                }
                if(Keyboard.GetState().IsKeyDown(Key.S)) {
                    physics.applyForce(-Vector2d.UnitY / 4);
                }
            } else {
                if(Keyboard.GetState().IsKeyDown(Key.Right)) {
                    physics.applyForce(Vector2d.UnitX / 6);
                }
                if(Keyboard.GetState().IsKeyDown(Key.Left)) {
                    physics.applyForce(-Vector2d.UnitX / 6);
                }

                if(Keyboard.GetState().IsKeyDown(Key.Up)) {
                    physics.applyForce(Vector2d.UnitY / 4);
                }
                if(Keyboard.GetState().IsKeyDown(Key.Down)) {
                    physics.applyForce(-Vector2d.UnitY / 4);
                }
            }

            if(Keyboard.GetState().IsKeyDown(Key.Space)) {
                System.Diagnostics.Debug.Close();
            }
            if(Keyboard.GetState().IsKeyDown(Key.Escape)) {
                Game.quit();
            }
            physics.applyForce(-0.2 * Vector2d.UnitY);
            //System.Diagnostics.Debug.WriteLine(globalVectorToLocal(Vector2d.UnitX));
        }
    }

    internal class Thong : GameObject
    {

        public Thong(string name,Texture texture,ulong layers,Vector2d position, Angle angle) : base(name,texture,position, new Vector2d(20,5),angle,layers) {
            Game.update += update;
            physics.dragFunction = 2;
            physics.dragType = DragType.Omnidirectional;
            physics.coefficiantOfDrag = 0.001;
            physics.usePhysics = true;
            physics.mass = double.PositiveInfinity;
            physics.lineColliders.Add(new LineCollider(physics,Vector2d.Zero,20,0,1));
            physics.momentOfInertia = double.PositiveInfinity;
        }

        void update(ulong frame) {
            //System.Windows.Forms.MessageBox.Show(position.Y.ToString());
            
        }
    }
}
