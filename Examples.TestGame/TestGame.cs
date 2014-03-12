using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics.OpenGL;
using Platform;
using System.IO;

namespace Examples.TestGame
{
    public class TestGame : Game
    {
        private GraphicsDeviceManager Graphics;

        public TestGame()
        {
            
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = 600;
            Graphics.PreferredBackBufferHeight = 480;

            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();

            IsMouseVisible = true;

            Content.RootDirectory = SystemInfo.RelativeContentDirectory;
            Window.Title = "Xna Test";
        }

        private Model model;
        private Matrix World;
        private Matrix View;
        private Matrix Projection;
        private Effect shader1;
        private Effect shader2;

        protected override void LoadContent()
        {
            model = Content.Load<Model>("Models/sphere");

            string shader1Path = SystemInfo.RelativeContentDirectory + "Shader/hlsl";
            shader1 = new Effect(
                graphicsDevice: GraphicsDevice,
                effectCode: File.ReadAllBytes(shader1Path + ".mgfx"),
                effectName: "shader1"
            );

            Vector3 position = new Vector3(10, 10, 10);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;
            
            World = Matrix.Identity;
            View = Matrix.CreateLookAt(position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), aspectRatio, nearPlane, farPlane);

            shader1.Parameters["World"].SetValue(World);
            shader1.Parameters["View"].SetValue(View);
            shader1.Parameters["Projection"].SetValue(Projection);

            shader1.Parameters["color1"].SetValue(Color.Yellow.ToVector4());
            shader1.Parameters["color2"].SetValue(Color.Red.ToVector4());

            /*
            foreach (var pair in EffectUtilities.ReadableEffectCode)
            {
                Log.Message("ReadableEffectCode(", pair.Key, "):");
                Log.Message(pair.Value);
            }*/

            // Write human-readable effect code to file
            File.WriteAllText(shader1Path + ".glfx", shader1.EffectCode);

            // Construct a new shader by loading the human-readable effect code
            /*shader2 = new Effect(
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText(shader1Path + ".glfx"),
                effectName: "shader2"
            );*/
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.BlanchedAlmond);

            RemapModel(model, shader1);
            foreach (ModelMesh mesh in model.Meshes)
            {
                mesh.Draw();
            }
        }

        private void RemapModel(Model model, Effect effect)
        {
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }
    }
}

