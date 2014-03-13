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
        private Effect shader3;
        private Effect shader3_gl;
        private Effect shader4;
        private Effect currentShader;
        private Texture2D texture;
        private Vector3 position;
        private Vector3 target;

        protected override void LoadContent()
        {
            model = Content.Load<Model>("Models/test");

            string shaderPath = SystemInfo.RelativeContentDirectory + "Shader/";
            shader3 = new Effect(
                graphicsDevice: GraphicsDevice,
                effectCode: File.ReadAllBytes(shaderPath + "shader3.mgfx"),
                effectName: "shader3"
            );

            // Write human-readable effect code to file
            File.WriteAllText(shaderPath + "shader3.glfx_gen", shader3.EffectCode);

            // Construct a new shader by loading the human-readable effect code
            shader3_gl = new Effect(
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText(shaderPath + "shader3.glfx_gen"),
                effectName: "shader3_gl"
            );

            // Construct a new shader by loading the human-readable effect code
            shader4 = new Effect(
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText(shaderPath + "shader2.glfx"),
                effectName: "shader2"
            );

            //shader3_gl = shader3 = null;
            currentShader = shader3_gl;
            currentShader = shader3;
            //currentShader = shader2;

            string texturePath = SystemInfo.RelativeContentDirectory + "Textures/";
            FileStream stream = new FileStream(texturePath + "texture1.png", FileMode.Open);
            texture = Texture2D.FromStream(GraphicsDevice, stream);


            position = new Vector3(15, 15, 15);
            target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;
            
            World = Matrix.Identity;
            View = Matrix.CreateLookAt(position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), aspectRatio, nearPlane, farPlane);
        }

        Vector4 color = Color.Blue.ToVector4();
        Vector4 colorSigns = Vector4.One;
        Vector3 modelPosition = Vector3.Zero;
        Vector3 modelDirection = Vector3.Zero;

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.Gray);
            
            UpAndDown(ref color.X, ref colorSigns.X, 5);
            UpAndDown(ref color.Y, ref colorSigns.Y, 10);
            UpAndDown(ref color.Z, ref colorSigns.Z, 15);
            color.W = 1;

            //currentShader.Parameters["color1"].SetValue(Vector4.Normalize(color));
            //currentShader.Parameters["color2"].SetValue(Vector4.Normalize(color));
            try {
                currentShader.Parameters["ModelTexture"].SetValue(texture);
            } catch (NullReferenceException) {}
            //currentShader.Parameters["ViewVector"].SetValue(Vector3.Normalize(target-position));

            if (random.Next() % 20 == 0)
                modelDirection = new Vector3(random.Next() % 201 - 100, random.Next() % 201 - 100, random.Next() % 201 - 100) / 200f / 2f;
            modelPosition += modelDirection;
            if (modelPosition.Length() > 15)
                modelDirection = Vector3.Normalize(-modelPosition) * modelDirection.Length();

            Matrix modelWorld = Matrix.CreateScale (0.002f) * Matrix.CreateTranslation(modelPosition);
            currentShader.Parameters["World"].SetValue(modelWorld * World);
            currentShader.Parameters["View"].SetValue(View);
            currentShader.Parameters["Projection"].SetValue(Projection);
            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(modelWorld * World));
            currentShader.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);

            RemapModel(model, currentShader);
            foreach (ModelMesh mesh in model.Meshes)
            {
                mesh.Draw();
            }
        }

        Random random = new Random();

        private void UpAndDown(ref float x, ref float sign, int maxDiff)
        {
            float diff = (random.Next() % maxDiff) / 1000f;
            if (x + sign * diff > 0.95f)
            {
                sign = -1;
            }
            if (x + sign * diff < 0.05f)
            {
                sign = 1;
            }
            x += sign * diff;
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

