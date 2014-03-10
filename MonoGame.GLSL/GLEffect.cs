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
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.GLSL
{
    public class GLEffect
    {
        private GraphicsDevice Device;
        private List<GLShader> Shaders;
        private List<EffectParameter> Parameters;
        private List<EffectTechnique> Techniques;

        public GLEffect (GraphicsDevice device)
        {
            Device = device;
            Shaders = new List<GLShader> ();
            Parameters = new List<EffectParameter> ();
            Techniques = new List<EffectTechnique> ();
        }

        public Effect ToEffect ()
        {
            return new Effect (graphicsDevice: Device, effectCode: EffectCode);
        }

        public static implicit operator Effect (GLEffect glEffect)
        {
            return glEffect.ToEffect ();
        }

        public byte[] EffectCode {
            get {
                using (MemoryStream stream = new MemoryStream()) {
                    using (BinaryWriter writer = new BinaryWriter(stream)) {
                        WriteEffectCode (writer);
                    }
                    return stream.ToArray ();
                }
            }
        }

        /// <summary>
        /// The MonoGame Effect file format header identifier.
        /// </summary>
        private const string MGFXHeader = "MGFX";
        /// <summary>
        /// The current MonoGame Effect file format versions
        /// used to detect old packaged content.
        /// </summary>
        private const byte MGFXVersion = 5;
        /// <summary>
        /// The MGFX profile. OpenGL is 0, DirectX is 1.
        /// </summary>
        private const byte MGFXProfile = 0;

        private void WriteEffectCode (BinaryWriter writer)
        {
            writer.Write (MGFXHeader.ToCharArray ());
            writer.Write ((byte)MGFXVersion);
            writer.Write ((byte)MGFXProfile);
            
            // We don't have any constant buffers.
            int constantBufferCount = 0;
            writer.Write ((byte)constantBufferCount);

            // Write all the shader objects
            int shaderCount = Shaders.Count;
            writer.Write ((byte)shaderCount);
            foreach (GLShader shader in Shaders) {
                shader.WriteEffectCode (writer);
            }

            // Write the parameters.
            WriteParameters (writer, Parameters);

            // Write the techniques.
            int techniqueCount = Techniques.Count;
            writer.Write ((byte)techniqueCount);
            foreach (EffectTechnique technique in Techniques) {
                writer.Write (technique.Name);
                WriteAnnotations (writer);
                WritePasses (writer);
            }
        }

        private void WriteAnnotations (BinaryWriter writer)
        {
            writer.Write ((byte)0);
        }

        private void WriteParameters (BinaryWriter writer, EffectParameterCollection parameters)
        {
            WriteParameters (writer, parameters.ToList ());
        }

        private void WriteParameters (BinaryWriter writer, List<EffectParameter> parameters)
        {
            writer.Write ((byte)parameters.Count);
            if (parameters.Count == 0)
                return;

            foreach (EffectParameter param in parameters) {
                writer.Write ((byte)param.ParameterClass);
                writer.Write ((byte)param.ParameterType);
                writer.Write ((string)param.Name);
                writer.Write ((string)param.Semantic);
                WriteAnnotations (writer);
                writer.Write ((byte)param.RowCount);
                writer.Write ((byte)param.ColumnCount);
                
                WriteParameters (writer, param.Elements);
                WriteParameters (writer, param.StructureMembers);

                

                if (param.Elements.Count == 0 && param.StructureMembers.Count == 0) {
                    if (param.GetType () == EffectParameterType.Bool || param.GetType () == EffectParameterType.Int32
                        || param.GetType () == EffectParameterType.Single) {

                        var buffer = new float[rowCount * columnCount];
                        for (var j = 0; j < buffer.Length; j++)
                            buffer [j] = reader.ReadSingle ();
                        data = buffer;
                    }
                }
            }
        }

        private void WritePasses (BinaryWriter writer)
        {
            throw new NotImplementedException ();
        }
    }
}
