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
using OpenTK.Graphics.OpenGL;

namespace MonoGame.GLSL
{
    public class GLEffect : IEffectMatrices
    {
        private GraphicsDevice Device;
        private List<GLShaderProgram> Shaders;

        public Matrix Projection { get; set; }

        public Matrix View { get; set; }

        public Matrix World { get; set; }

        private GLEffect (GraphicsDevice device, IEnumerable<GLShaderProgram> shaderPrograms)
        {
            Device = device;
            Shaders = shaderPrograms.ToList ();
        }

        public static GLEffect FromFiles (GraphicsDevice device, string pixelShaderFilename, string vertexShaderFilename)
        {
            GLShader pixelShader = new GLShader (ShaderStage.Pixel, File.ReadAllText (pixelShaderFilename));
            GLShader vertexShader = new GLShader (ShaderStage.Vertex, File.ReadAllText (vertexShaderFilename));
            GLShaderProgram shaderProgram = new GLShaderProgram (vertex: vertexShader, pixel: pixelShader);
            return new GLEffect (device: device, shaderPrograms: new GLShaderProgram[] { shaderProgram });
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
                    SetVertexBuffer (part.VertexBuffer);
                    Indices = part.IndexBuffer;
                    DrawIndexedPrimitives (PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                }
            }
        }

        private VertexBufferBinding[] vertexBufferBindings;
        private IndexBuffer Indices;

        /// <summary>
        /// Draw geometry by indexing into the vertex buffer.
        /// </summary>
        /// <param name="primitiveType">The type of primitives in the index buffer.</param>
        /// <param name="baseVertex">Used to offset the vertex range indexed from the vertex buffer.</param>
        /// <param name="minVertexIndex">A hint of the lowest vertex indexed relative to baseVertex.</param>
        /// <param name="numVertices">An hint of the maximum vertex indexed.</param>
        /// <param name="startIndex">The index within the index buffer to start drawing from.</param>
        /// <param name="primitiveCount">The number of primitives to render from the index buffer.</param>
        /// <remarks>Note that minVertexIndex and numVertices are unused in MonoGame and will be ignored.</remarks>
        public void DrawIndexedPrimitives (
            PrimitiveType primitiveType,
            int baseVertex,
            int minVertexIndex,
            int numVertices,
            int startIndex,
            int primitiveCount
        )
        {
            foreach (GLShaderProgram pass in Shaders) {
                pass.Apply ();

                // Unsigned short or unsigned int?
                bool shortIndices = Device.Indices.IndexElementSize == IndexElementSize.SixteenBits;

                // Set up the vertex buffers
                foreach (VertexBufferBinding vertBuffer in vertexBufferBindings) {
                    if (vertBuffer.VertexBuffer != null) {
                        BindVertexBuffer (vertBuffer.VertexBuffer.Handle);
                        vertBuffer.VertexBuffer.VertexDeclaration.Apply (
                            VertexShader,
                            (IntPtr)(vertBuffer.VertexBuffer.VertexDeclaration.VertexStride * (vertBuffer.VertexOffset + baseVertex))
                        );
                    }
                }

                // Enable the appropriate vertex attributes.
                OpenGLDevice.Instance.FlushGLVertexAttributes ();

                // Bind the index buffer
                OpenGLDevice.Instance.BindIndexBuffer (Indices.Handle);

                // Draw!
                GL.DrawRangeElements (
                    PrimitiveTypeGL (primitiveType),
                    minVertexIndex,
                    minVertexIndex + numVertices,
                    GetElementCountArray (primitiveType, primitiveCount),
                    shortIndices ? DrawElementsType.UnsignedShort : DrawElementsType.UnsignedInt,
                    (IntPtr)(startIndex * (shortIndices ? 2 : 4))
                );

                // Check for errors in the debug context
                GraphicsExtensions.CheckGLError ();
            }
        }

        public void SetVertexBuffer (VertexBuffer vertexBuffer)
        {
            if (!ReferenceEquals (vertexBufferBindings [0].VertexBuffer, vertexBuffer)) {
                vertexBufferBindings [0] = new VertexBufferBinding (vertexBuffer);
            }

            for (int vertexStreamSlot = 1; vertexStreamSlot < vertexBufferBindings.Length; ++vertexStreamSlot) {
                if (vertexBufferBindings [vertexStreamSlot].VertexBuffer != null) {
                    vertexBufferBindings [vertexStreamSlot] = new VertexBufferBinding (null);
                }
            }
        }

        private int currentVertexBuffer = 0;
        private int currentIndexBuffer = 0;

        public void BindVertexBuffer (int buffer)
        {
            if (buffer != currentVertexBuffer) {
                GL.BindBuffer (BufferTarget.ArrayBuffer, buffer);
                currentVertexBuffer = buffer;
            }
        }

        public void BindIndexBuffer (int buffer)
        {
            if (buffer != currentIndexBuffer) {
                GL.BindBuffer (BufferTarget.ElementArrayBuffer, buffer);
                currentIndexBuffer = buffer;
            }
        }
    }
}
