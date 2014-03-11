/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * This source code file is part of Knot3. Copying, redistribution and
 * use of the source code in this file in source and binary forms,
 * with or without modification, are permitted provided that the conditions
 * of the MIT license are met:
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 *
 * See the LICENSE file for full license details of the Knot3 project.
 */
using System;
using System.Diagnostics.CodeAnalysis;
using Knot3.Framework.Core;
using Microsoft.Xna.Framework;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using System.IO;
using Knot3.Framework.Effects;
using MonoGame.GLSL;
using Microsoft.Xna.Framework.Graphics;

namespace Examples.TestGame
{
    public class ExampleRenderEffect : RenderEffect
    {
        private GLEffect effect;
        Effect pascalEffect;


        public ExampleRenderEffect (IScreen screen)
            : base (screen)
        {
            pascalEffect = screen.LoadEffect ("OpaqueShader");


            string pixelShaderFilename = SystemInfo.RelativeContentDirectory + "Shader" + SystemInfo.PathSeparator + "pixel1.glfx";
            string vertexShaderFilename = SystemInfo.RelativeContentDirectory + "Shader" + SystemInfo.PathSeparator + "vertex1.glfx";

            effect = GLEffect.FromFiles (
                pixelShaderFilename: pixelShaderFilename,
                vertexShaderFilename: vertexShaderFilename
            );
        }

        public override void DrawModel (GameModel model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            //Viewport original = screen.Viewport;
            //screen.Viewport = model.World.Viewport;
            Console.WriteLine ("fuck: "+model.World);

            // die aktuellen Matrizen setzen
            Camera camera = model.World.Camera;
            effect.World = camera.WorldMatrix;
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;
            effect.Parameters.SetMatrix ("WorldViewProjection", camera.ProjectionMatrix * camera.ViewMatrix * camera.WorldMatrix);

            // das Modell zeichnen
            foreach (ModelMesh mesh in model.Model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    if (part.Effect is BasicEffect) {
                        ModifyBasicEffect (part.Effect as BasicEffect, model);
                    }
                }
            }
            
            effect.Draw (model.Model);

            // Setze den Viewport wieder auf den ganzen Screen
            //screen.Viewport = original;
        }
    }
}
