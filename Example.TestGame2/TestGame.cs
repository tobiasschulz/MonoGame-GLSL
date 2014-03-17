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

        private Model model1;
        private Model model2;
        private Model model3;
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
            currentShader = shader4;

            string texturePath = SystemInfo.RelativeContentDirectory + "Textures/";
            FileStream stream = new FileStream (texturePath + "texture2.png", FileMode.Open);
            texture = Texture2D.FromStream (GraphicsDevice, stream);

            position = new Vector3 (15, 0, 15);
            target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;

            World = Matrix.Identity;
            View = Matrix.CreateLookAt (position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (60), aspectRatio, nearPlane, farPlane);
        }

        Vector3[] modelPositions = new Vector3 [3];
        Vector3[] modelDirections = new Vector3 [3];

        protected override void Draw (GameTime time)
        {
            GraphicsDevice.Clear (Color.Gray);

            MoveModel (0);

            Matrix modelWorld1 = Matrix.CreateScale (0.002f) * Matrix.CreateTranslation (modelPositions [0]);
            SetShaderParameters (modelWorld1);
            RemapModel (model1, currentShader);
            foreach (ModelMesh mesh in model1.Meshes) {
                mesh.Draw ();
            }

            MoveModel (1);

            Matrix modelWorld2 = Matrix.CreateTranslation (modelPositions [1]);
            SetShaderParameters (modelWorld2);
            RemapModel (model2, currentShader);
            foreach (ModelMesh mesh in model2.Meshes) {
                mesh.Draw ();
            }

            MoveModel (2);

            Matrix modelWorld3 = Matrix.CreateTranslation (modelPositions [2]);
            SetShaderParameters (modelWorld3);
            RemapModel (model3, currentShader);
            foreach (ModelMesh mesh in model3.Meshes) {
                mesh.Draw ();
            }
        }

        void MoveModel (int i)
        {
            if (random.Next () % 20 == 0) {
                modelDirections [i] = new Vector3 (random.Next () % 201 - 100, random.Next () % 201 - 100, random.Next () % 201 - 100) / 200f / 2f;
            }
            modelPositions [i] += modelDirections [i];
            if (modelPositions [i].Length () > 15) {
                modelDirections [i] = Vector3.Normalize (-modelPositions [i]) * modelDirections [i].Length ();
            }
        }

        private void SetShaderParameters (Matrix modelWorld)
        {
            try {
                currentShader.Parameters ["ModelTexture"].SetValue (texture);
            }
            catch (NullReferenceException) {}

            currentShader.Parameters ["World"].SetValue (modelWorld * World);
            currentShader.Parameters ["View"].SetValue (View);
            currentShader.Parameters ["Projection"].SetValue (Projection);
            Matrix worldInverseTransposeMatrix = Matrix.Transpose (Matrix.Invert (modelWorld * World));
            currentShader.Parameters ["WorldInverseTranspose"].SetValue (worldInverseTransposeMatrix);
        }

        Random random = new Random ();

        private void UpAndDown (ref float x, ref float sign, int maxDiff)
        {
            float diff = (random.Next () % maxDiff) / 1000f;
            if (x + sign * diff > 0.95f) {
                sign = -1;
            }
            if (x + sign * diff < 0.05f) {
                sign = 1;
            }
            x += sign * diff;
        }

        private void RemapModel (Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = effect;
                }
            }
        }
    }
}
