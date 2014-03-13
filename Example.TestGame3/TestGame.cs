/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using OpenTK.Graphics.OpenGL;
using Platform;

namespace Examples.TestGame3
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

        private Model model1;
        private Model model2;
        private Model model3;
        private Cylinder cylinder1;
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

        protected override void LoadContent ()
        {
            model1 = Content.Load<Model>("Models/test");
            model2 = Content.Load<Model>("Models/sphere");
            model3 = Content.Load<Model>("Models/pipe-straight");
            cylinder1 = new Cylinder (device: GraphicsDevice, height: 2f, diameter: 0.5f, tessellation: 64);

            string shaderPath = SystemInfo.RelativeContentDirectory + "Shader/";
            shader3 = new Effect (
                graphicsDevice: GraphicsDevice,
                effectCode: File.ReadAllBytes (shaderPath + "shader3.mgfx"),
                effectName: "shader3"
            );

            // Write human-readable effect code to file
            File.WriteAllText (shaderPath + "shader3.glfx_gen", shader3.EffectCode);

            // Construct a new shader by loading the human-readable effect code
            shader3_gl = new Effect (
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText (shaderPath + "shader3.glfx_gen"),
                effectName: "shader3_gl"
            );

            // Construct a new shader by loading the human-readable effect code
            shader4 = new Effect (
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText (shaderPath + "shader4.glfx"),
                effectName: "shader4"
            );

            //shader3_gl = shader3 = null;
            currentShader = shader3;
            currentShader = shader3_gl;
            //currentShader = shader4;

            string texturePath = SystemInfo.RelativeContentDirectory + "Textures/";
            FileStream stream = new FileStream (texturePath + "texture1.png", FileMode.Open);
            texture = Texture2D.FromStream (GraphicsDevice, stream);

            position = (Vector3.Right + Vector3.Backward) * 15f;
            target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;

            World = Matrix.Identity;
            View = Matrix.CreateLookAt (position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (60), aspectRatio, nearPlane, farPlane);

            modelScale [0] = Vector3.One * 0.002f;
            modelScale [1] = Vector3.One * 1f;
            modelScale [2] = Vector3.One * 2f;
            modelScale [3] = Vector3.One * 2f;

            modelPositions [0] = (Vector3.Left + Vector3.Up) * 10f;
            modelPositions [1] = (Vector3.Left + Vector3.Down) * 10f;
            modelPositions [2] = (Vector3.Forward + Vector3.Up) * 10f;
            modelPositions [3] = (Vector3.Forward + Vector3.Down) * 10f;
        }

        Vector3[] modelScale = new Vector3 [4];
        Vector3[] modelPositions = new Vector3 [4];
        Vector3[] modelRotations = new Vector3 [4];
        Vector3[] modelDirections = new Vector3 [4];

        protected override void Draw (GameTime time)
        {
            GraphicsDevice.Clear (Color.Gray);
            
            RotateModel (0);
            modelRotations [3] = modelRotations [2] = modelRotations [1] = modelRotations [0];

            int index = 0;
            SetShaderParameters (index);
            RemapModel (model1, currentShader);
            foreach (ModelMesh mesh in model1.Meshes)
            {
                mesh.Draw ();
            }

            ++index;
            SetShaderParameters (index);
            RemapModel (model2, currentShader);
            foreach (ModelMesh mesh in model2.Meshes)
            {
                mesh.Draw ();
            }
            
            ++index;
            SetShaderParameters (index);
            RemapModel (model3, currentShader);
            foreach (ModelMesh mesh in model3.Meshes)
            {
                mesh.Draw ();
            }
            
            ++index;
            SetShaderParameters (index);
            cylinder1.Draw (currentShader);
        }

        void RotateModel (int i)
        {
            if (random.Next () % 100 == 0)
            {
                modelDirections [i] = new Vector3 (random.Next () % 201 - 100, random.Next () % 201 - 100, random.Next () % 201 - 100) / 200f / 15f;
            }
            modelRotations [i] += modelDirections [i];
            if (modelRotations [i].Length () > 15)
            {
                modelDirections [i] = Vector3.Normalize (-modelRotations [i]) * modelDirections [i].Length ();
            }
        }

        private void SetShaderParameters (int index)
        {
            Matrix modelWorld = Matrix.CreateScale (modelScale [index])
                * Matrix.CreateFromYawPitchRoll (modelRotations [index].Y, modelRotations [index].X, modelRotations [index].Z)
                * Matrix.CreateTranslation (modelPositions [index]);

            try
            {
                currentShader.Parameters ["ModelTexture"].SetValue (texture);
            }
            catch (NullReferenceException)
            {
            }

            currentShader.Parameters ["World"].SetValue (modelWorld * World);
            currentShader.Parameters ["View"].SetValue (View);
            currentShader.Parameters ["Projection"].SetValue (Projection);
            Matrix worldInverseTransposeMatrix = Matrix.Transpose (Matrix.Invert (modelWorld * World));
            currentShader.Parameters ["WorldInverseTranspose"].SetValue (worldInverseTransposeMatrix);
        }

        Random random = new Random ();

        private void RemapModel (Model model, Effect effect)
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
