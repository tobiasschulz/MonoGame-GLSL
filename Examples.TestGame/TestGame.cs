using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics.OpenGL;
using Platform;

namespace Examples.TestGame
{
    public class TestGame : Game
    {
        private GraphicsDeviceManager Graphics;

        public TestGame ()
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

        private Model model;

        protected override void LoadContent ()
        {
            model = Content.Load<Model> ("sphere");

            string pixelShaderFilename = SystemInfo.RelativeContentDirectory + "Shader" + SystemInfo.PathSeparator + "flat-color.frag";
            string vertexShaderFilename = SystemInfo.RelativeContentDirectory + "Shader" + SystemInfo.PathSeparator + "flat-color.vert";

            Vector3 position = new Vector3 (20, 10, 20);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;

            /*
            effect = GLEffect.FromFiles (
                pixelShaderFilename: pixelShaderFilename,
                vertexShaderFilename: vertexShaderFilename
            );
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt (position, target, up);
            ;
            effect.Projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (60), aspectRatio, nearPlane, farPlane);
            effect.Parameters.SetMatrix ("MVP", effect.Projection * effect.View * effect.World);

            Matrix mat = effect.Projection * effect.View * effect.World;
            Console.WriteLine ("mat4(");
            Console.WriteLine ("    vec4(" + mat.M11 + ", " + mat.M12 + ", " + mat.M13 + ", " + mat.M14 + "),");
            Console.WriteLine ("    vec4(" + mat.M21 + ", " + mat.M22 + ", " + mat.M23 + ", " + mat.M24 + "),");
            Console.WriteLine ("    vec4(" + mat.M31 + ", " + mat.M32 + ", " + mat.M33 + ", " + mat.M34 + "),");
            Console.WriteLine ("    vec4(" + mat.M41 + ", " + mat.M42 + ", " + mat.M43 + ", " + mat.M44 + ")");
            Console.WriteLine (")");

*/
        }

        protected override void Draw (GameTime time)
        {
            //GLEffect.GraphicsDevice = GraphicsDevice;
            //GraphicsDevice.Clear (Color.Green);
            //effect.Draw (model);

        }
    }
}

