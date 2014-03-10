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
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MonoGame.GLSL
{
    internal class GLShader : IDisposable
    {
        public ShaderStage Stage { get; private set; }

        public string Code { get; private set; }

        public int HashKey { get; private set; }
        // The shader handle.
        private int _shaderHandle = -1;
        private GLAttribute[] Attributes;

        public GLSamplerInfo[] Samplers { get; private set; }

        public GLShader (ShaderStage stage, string code)
        {
            Stage = stage;
            Code = code;

            HashKey = Hash.ComputeHash (Code.ToCharArray ());
        }

        internal int GetShaderHandle ()
        {
            // If the shader has already been created then return it.
            if (_shaderHandle != -1)
                return _shaderHandle;

            //
            _shaderHandle = GL.CreateShader (Stage == ShaderStage.Vertex ? ShaderType.VertexShader : ShaderType.FragmentShader);
            GraphicsExtensions.CheckGLError ();
            GL.ShaderSource (_shaderHandle, Code);
            GraphicsExtensions.CheckGLError ();
            GL.CompileShader (_shaderHandle);
            GraphicsExtensions.CheckGLError ();

            var compiled = 0;
            GL.GetShader (_shaderHandle, ShaderParameter.CompileStatus, out compiled);
            GraphicsExtensions.CheckGLError ();
            if (compiled == (int)All.False) {
                var log = GL.GetShaderInfoLog (_shaderHandle);
                Console.WriteLine (log);

                if (GL.IsShader (_shaderHandle)) {
                    GL.DeleteShader (_shaderHandle);
                    GraphicsExtensions.CheckGLError ();
                }
                _shaderHandle = -1;

                throw new InvalidOperationException ("Shader Compilation Failed");
            }

            return _shaderHandle;
        }

        internal void GetVertexAttributeLocations (int program)
        {
            for (int i = 0; i < Attributes.Length; ++i) {
                Attributes [i].location = GL.GetAttribLocation (program, Attributes [i].name);
                GraphicsExtensions.CheckGLError ();
            }
        }

        internal int GetAttribLocation (VertexElementUsage usage, int index)
        {
            for (int i = 0; i < Attributes.Length; ++i) {
                if ((Attributes [i].usage == usage) && (Attributes [i].index == index))
                    return Attributes [i].location;
            }
            return -1;
        }

        internal void ApplySamplerTextureUnits (int program)
        {
            // Assign the texture unit index to the sampler uniforms.
            foreach (var sampler in Samplers) {
                var loc = GL.GetUniformLocation (program, sampler.name);
                GraphicsExtensions.CheckGLError ();
                if (loc != -1) {
                    GL.Uniform1 (loc, sampler.textureSlot);
                    GraphicsExtensions.CheckGLError ();
                }
            }
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected void Dispose (bool disposing)
        {
            if (_shaderHandle != -1) {
                if (GL.IsShader (_shaderHandle)) {
                    GL.DeleteShader (_shaderHandle);
                    GraphicsExtensions.CheckGLError ();
                }
                _shaderHandle = -1;
            }
        }
    }

    internal struct GLAttribute
    {
        public VertexElementUsage usage;
        public int index;
        public string name;
        public short format;
        public int location;
    }

    internal struct GLSamplerInfo
    {
        public GLSamplerType type;
        public int textureSlot;
        public int samplerSlot;
        public string name;
        public SamplerState state;
        // TODO: This should be moved to EffectPass.
        public int parameter;
    }

    internal enum GLSamplerType
    {
        Sampler2D = 0,
        SamplerCube = 1,
        SamplerVolume = 2,
        Sampler1D = 3,
    }
}
