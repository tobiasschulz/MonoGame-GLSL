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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.GLSL
{
    public class GLEffect : IEffectMatrices
    {
        private GraphicsDevice Device;
        private List<GLShaderProgram> Shaders;

        public Matrix Projection { get; set; }

        public Matrix View { get; set; }

        public Matrix World { get; set; }

        public GLEffect (GraphicsDevice device)
        {
            Device = device;
            Shaders = new List<GLShaderProgram> ();
        }

        public void Draw (Model model)
        {
            int boneCount = model.Bones.Count;

            // Look up combined bone matrices for the entire model.
            Matrix[] sharedDrawBoneMatrices = new Matrix [boneCount];
            model.CopyAbsoluteBoneTransformsTo (sharedDrawBoneMatrices);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes) {
                Matrix world = sharedDrawBoneMatrices [mesh.ParentBone.Index] * World;
                Draw (mesh, world);
            }
        }

        public void Draw (ModelMesh mesh)
        {
            Draw (mesh, World);
        }

        public void Draw (ModelMesh mesh, Matrix world)
        {
            foreach (ModelMeshPart part in mesh.MeshParts) {
                if (part.PrimitiveCount > 0) {
                    Device.SetVertexBuffer (part.VertexBuffer);
                    Device.Indices = part.IndexBuffer;

                    foreach (GLShaderProgram pass in Shaders) {
                        pass.Apply ();
                        Device.DrawIndexedPrimitives (PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }
    }
}
