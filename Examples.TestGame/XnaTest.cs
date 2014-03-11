using System;
using Microsoft.Xna.Framework;
using Knot3.Framework.Platform;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.GLSL;

namespace Examples.TestGame
{
    public class XnaTest : Game
    {
        private GraphicsDeviceManager Graphics;

        public XnaTest ()
        {
            
            Graphics = new GraphicsDeviceManager (this);

            Graphics.PreferredBackBufferWidth = 600;
            Graphics.PreferredBackBufferHeight = 480;

            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges ();

            IsMouseVisible = true;

            Content.RootDirectory = SystemInfo.RelativeContentDirectory;
            Window.Title = "Xna Test";
        }
        
        private GLEffect effect;
        private Model model;

        protected override void LoadContent ()
        {
            model = Content.Load<Model> ("sphere");

            string pixelShaderFilename = SystemInfo.RelativeContentDirectory + "Shader" + SystemInfo.PathSeparator + "flat-color.frag";
            string vertexShaderFilename = SystemInfo.RelativeContentDirectory + "Shader" + SystemInfo.PathSeparator + "flat-color.vert";

            Vector3 position = new Vector3 (20,20,20);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;

            effect = GLEffect.FromFiles (
                pixelShaderFilename: pixelShaderFilename,
                vertexShaderFilename: vertexShaderFilename
                );
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt (position, target, up);;
            effect.Projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (60), aspectRatio, nearPlane, farPlane);
            effect.Parameters.SetMatrix ("MVP", effect.Projection * effect.View * effect.World);
        }

        protected override void Draw (GameTime time)
        {
            GraphicsDevice.Clear (Color.Green);
            effect.Draw (model);

        }
    }
}

