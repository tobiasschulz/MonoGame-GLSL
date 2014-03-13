/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
 */

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Primitives
{
    public class Cylinder : Primitive
    {
        public Cylinder (GraphicsDevice device)
        : this (device, 1, 1, 32)
        {
        }

        public Cylinder (GraphicsDevice device, float height, float diameter, int tessellation)
        {
            if (tessellation < 3) {
                throw new ArgumentOutOfRangeException ("cylinder tessellation");
            }

            float halfHeight = height / 2;
            float radius = diameter / 2;

            for (int i = 0; i < tessellation; i++) {
                Vector3 normal = GetCircleVector (i, tessellation);
                float textureU = (float)i / (float)tessellation;

                AddVertex (position: normal * radius + Vector3.Up * halfHeight, normal: normal, texCoord: new Vector2 (textureU, 0));
                AddVertex (position: normal * radius + Vector3.Down * halfHeight, normal: normal, texCoord: new Vector2 (textureU, 1));

                AddIndex (i * 2);
                AddIndex (i * 2 + 1);
                AddIndex ((i * 2 + 2) % (tessellation * 2));

                AddIndex (i * 2 + 1);
                AddIndex ((i * 2 + 3) % (tessellation * 2));
                AddIndex ((i * 2 + 2) % (tessellation * 2));
            }

            CreateCap (tessellation, halfHeight, radius, Vector3.Up);
            CreateCap (tessellation, halfHeight, radius, Vector3.Down);

            InitializePrimitive (device);
        }

        void CreateCap (int tessellation, float height, float radius, Vector3 normal)
        {
            // create cap indices.
            for (int i = 0; i < tessellation - 2; i++) {
                if (normal.Y > 0) {
                    AddIndex (CurrentVertex);
                    AddIndex (CurrentVertex + (i + 1) % tessellation);
                    AddIndex (CurrentVertex + (i + 2) % tessellation);
                }
                else {
                    AddIndex (CurrentVertex);
                    AddIndex (CurrentVertex + (i + 2) % tessellation);
                    AddIndex (CurrentVertex + (i + 1) % tessellation);
                }
            }

            // create cap vertices.
            for (int i = 0; i < tessellation; i++) {
                Vector3 position = GetCircleVector (i, tessellation) * radius +
                                   normal * height;

                AddVertex (position, normal, Vector2.Zero);
            }
        }

        static Vector3 GetCircleVector (int i, int tessellation)
        {
            float angle = i * MathHelper.TwoPi / tessellation;
            float dx = (float)Math.Cos (angle);
            float dz = (float)Math.Sin (angle);
            return new Vector3 (dx, 0, dz);
        }
    }
}
