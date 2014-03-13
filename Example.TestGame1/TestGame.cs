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

        private Model model;
        private Matrix World;
        private Matrix View;
        private Matrix Projection;
        private Effect shader1;
        private Effect shader1_gl;
        private Effect shader2;
        private Effect currentShader;

        protected override void LoadContent ()
        {
            model = Content.Load<Model>("Models/sphere");

            string shaderPath = SystemInfo.RelativeContentDirectory + "Shader/";
            shader1 = new Effect (
                graphicsDevice: GraphicsDevice,
                effectCode: File.ReadAllBytes (shaderPath + "shader1.mgfx"),
                effectName: "shader1"
            );

            // Write human-readable effect code to file
            File.WriteAllText (shaderPath + "shader1.glfx_gen", shader1.EffectCode);

            // Construct a new shader by loading the human-readable effect code
            shader1_gl = new Effect (
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText (shaderPath + "shader1.glfx_gen"),
                effectName: "shader1_gl"
            );

            // Construct a new shader by loading the human-readable effect code
            shader2 = new Effect (
                graphicsDevice: GraphicsDevice,
                effectCode: System.IO.File.ReadAllText (shaderPath + "shader2.glfx"),
                effectName: "shader2"
            );

            shader1_gl = shader1 = null;
            currentShader = shader1_gl;
            currentShader = shader2;

            Vector3 position = new Vector3 (15, 15, 15);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;
            float aspectRatio = Graphics.GraphicsDevice.Viewport.AspectRatio;
            float nearPlane = 0.5f;
            float farPlane = 1000.0f;

            World = Matrix.Identity;
            View = Matrix.CreateLookAt (position, target, up);
            Projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (60), aspectRatio, nearPlane, farPlane);
        }

        Vector4 color = Color.Blue.ToVector4 ();
        Vector4 colorSigns = Vector4.One;
        Vector3 modelPosition = Vector3.Zero;
        Vector3 modelDirection = Vector3.Zero;

        protected override void Draw (GameTime time)
        {
            GraphicsDevice.Clear (Color.Gray);

            UpAndDown (ref color.X, ref colorSigns.X, 5);
            UpAndDown (ref color.Y, ref colorSigns.Y, 10);
            UpAndDown (ref color.Z, ref colorSigns.Z, 15);
            color.W = 1;

            currentShader.Parameters ["color1"].SetValue (Vector4.Normalize (color));
            currentShader.Parameters ["color2"].SetValue (Vector4.Normalize (color));

            if (random.Next () % 20 == 0) {
                modelDirection = new Vector3 (random.Next () % 201 - 100, random.Next () % 201 - 100, random.Next () % 201 - 100) / 200f / 2f;
            }
            modelPosition += modelDirection;
            if (modelPosition.Length () > 15) {
                modelDirection = Vector3.Normalize (-modelPosition)* modelDirection.Length ();
            }

            Matrix modelWorld = Matrix.CreateTranslation (modelPosition);
            currentShader.Parameters ["World"].SetValue (modelWorld * World);
            currentShader.Parameters ["View"].SetValue (View);
            currentShader.Parameters ["Projection"].SetValue (Projection);

            RemapModel (model, currentShader);
            foreach (ModelMesh mesh in model.Meshes) {
                mesh.Draw ();
            }
        }

        Random random = new Random ();

        private void UpAndDown (ref float x, ref float sign, int maxDiff)
        {
            float diff = (random.Next () % maxDiff) / 1000f;
            if (x + sign*diff > 0.95f) {
                sign = -1;
            }
            if (x + sign*diff < 0.05f) {
                sign = 1;
            }
            x += sign*diff;
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
