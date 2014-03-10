using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.GLSL
{
    internal struct Attribute
    {
        public VertexElementUsage usage;
        public int index;
        public string name;
        public short format;
        public int location;
    }

    internal class GLShader
    {
        public byte[] ByteCode { get; private set; }

        public ShaderStage Stage { get; private set; }

        private List<Attribute> Attributes;

        public GLShader ()
        {
            Attributes = new List<Attribute> ();
        }

        public void WriteEffectCode (BinaryWriter writer)
        {
            bool isVertexShader = Stage == ShaderStage.Vertex ? true : false;
            writer.Write ((bool)isVertexShader);
            
            writer.Write ((Int32)ByteCode.Length);
            writer.Write ((byte[]) ByteCode);

            // TODO: Samplers (Shader.cs:126)
            int samplerCount = 0;
            writer.Write ((byte)samplerCount);

            // TODO: CBuffers (Shader.cs:154)
            int cBufferCount = 0;
            writer.Write ((byte)cBufferCount);

            // Attributes
            writer.Write ((byte)Attributes.Count);
            foreach (Attribute attr in Attributes) {
                writer.Write ((string)attr.name);
                writer.Write ((byte)attr.usage);
                writer.Write ((byte)attr.index);
                writer.Write ((Int16)attr.format);
            }
        }
    }
}

