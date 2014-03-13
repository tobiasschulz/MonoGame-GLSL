/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Primitives
{
    public abstract class Primitive : IDisposable
    {
        List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
        List<ushort> indices = new List<ushort>();
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        protected void AddVertex (Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            vertices.Add (new VertexPositionNormalTexture (position, normal, texCoord));
        }

        protected void AddIndex (int index)
        {
            if (index > ushort.MaxValue) {
                throw new ArgumentOutOfRangeException ("index");
            }

            indices.Add ((ushort)index);
        }

        protected int CurrentVertex { get { return vertices.Count; } }

        protected void InitializePrimitive (GraphicsDevice device)
        {
            vertexBuffer = new VertexBuffer (device, typeof (VertexPositionNormalTexture), vertices.Count, BufferUsage.None);
            vertexBuffer.SetData (vertices.ToArray ());
            indexBuffer = new IndexBuffer (device, typeof (ushort), indices.Count, BufferUsage.None);
            indexBuffer.SetData (indices.ToArray ());
        }

        ~Primitive ()
        {
            Dispose (false);
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                if (vertexBuffer != null) {
                    vertexBuffer.Dispose ();
                }
                if (indexBuffer != null) {
                    indexBuffer.Dispose ();
                }
            }
        }

        public void Draw (Effect effect)
        {
            GraphicsDevice device = effect.GraphicsDevice;
            device.SetVertexBuffer (vertexBuffer);
            device.Indices = indexBuffer;

            foreach (EffectPass effectPass in effect.CurrentTechnique.Passes) {
                effectPass.Apply ();
                int primitiveCount = indices.Count / 3;
                device.DrawIndexedPrimitives (PrimitiveType.TriangleList, 0, 0, vertices.Count, 0, primitiveCount);
            }
        }
    }
}
