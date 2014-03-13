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
        private Effect shader1_gl;
        private Effect shader2;
        private Effect currentShader;

        protected override void LoadContent()
        {
            model = Content.Load<Model>("Models/sphere");

            string shaderPath = SystemInfo.RelativeContentDirectory + "Shader/";
            shader1 = new Effect(
                graphicsDevice: GraphicsDevice,
                effectCode: File.ReadAllBytes(shaderPath + "shader1.mgfx"),
                effectName: "shader1"
                );

            // Write human-readable effect code to file
            File.WriteAllText(shaderPath + "shader1.glfx_gen", shader1.EffectCode);

            // Construct a new shader by loading the human-readable effect code
            shader1_gl = new Effect(
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText(shaderPath + "shader1.glfx_gen"),
                effectName: "shader1_gl"
                );

            // Construct a new shader by loading the human-readable effect code
            shader2 = new Effect(
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText(shaderPath + "shader2.glfx"),
                effectName: "shader2"
                );
            
            currentShader = shader1_gl;
            currentShader = shader2;

            Vector3 position = new Vector3(15, 15, 15);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;
            
            World = Matrix.Identity;
            View = Matrix.CreateLookAt(position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), aspectRatio, nearPlane, farPlane);
        }
        
        Vector4 color = Color.Blue.ToVector4();
        Vector3 modelPosition = Vector3.Zero;
        Random random = new Random();

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.Gray);
            
            color.X = ((color.X*1000 + random.Next()%10)%1000) / 1000f;
            color.Y = ((color.Y*1000 + random.Next()%20)%1000) / 1000f;
            color.Z = ((color.Z*1000 + random.Next()%30)%1000) / 1000f;
            color.W = 1;
            //Console.WriteLine(color);

            shader1.Parameters["color1"].SetValue(color);
            shader1.Parameters["color2"].SetValue(color);

            modelPosition += new Vector3(random.Next()%3-1, random.Next()%3-1, random.Next()%3-1);
            if (modelPosition.Length() > 20)
                modelPosition = Vector3.Zero;

            Matrix modelWorld = Matrix.CreateTranslation(modelPosition);
            shader1.Parameters["World"].SetValue(modelWorld * World);
            shader1.Parameters["View"].SetValue(View);
            shader1.Parameters["Projection"].SetValue(Projection);

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

