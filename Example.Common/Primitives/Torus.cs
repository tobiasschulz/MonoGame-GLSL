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
    public class Torus : Primitive
    {
        public Torus (GraphicsDevice device)
        : this (device, 1, 0.25f, 32)
        {
        }

        public Torus (GraphicsDevice device, float diameter, float thickness, int tessellation, float circlePercent = 1f)
        {
            if (tessellation < 3) {
                throw new ArgumentOutOfRangeException ("cylinder tessellation");
            }
            
            for (int i = 0; i < tessellation*circlePercent; i++)
            {
                float outerAngle = i * MathHelper.TwoPi / tessellation;
                float textureU = (float)i / (float)tessellation;

                Matrix transform = Matrix.CreateTranslation (diameter / 2, 0, 0) *
                    Matrix.CreateRotationY (outerAngle);

                // Now we loop along the other axis, around the side of the tube.
                for (int j = 0; j < tessellation; j++)
                {
                    float innerAngle = j * MathHelper.TwoPi / tessellation;
                    float textureV = (float)j / (float)tessellation;

                    float dx = (float)Math.Cos (innerAngle);
                    float dy = (float)Math.Sin (innerAngle);

                    // Create a vertex.
                    Vector3 normal = new Vector3 (dx, dy, 0);
                    Vector3 position = normal * thickness / 2;

                    position = Vector3.Transform (position, transform);
                    normal = Vector3.TransformNormal (normal, transform);

                    AddVertex (position: position, normal: normal, texCoord: new Vector2 (textureU, textureV));

                    // And create indices for two triangles.
                    int nextI = (i + 1) % tessellation;
                    int nextJ = (j + 1) % tessellation;

                    if (nextI < tessellation*circlePercent)
                    {
                        AddIndex (i * tessellation + j);
                        AddIndex (i * tessellation + nextJ);
                        AddIndex (nextI * tessellation + j);

                        AddIndex (i * tessellation + nextJ);
                        AddIndex (nextI * tessellation + nextJ);
                        AddIndex (nextI * tessellation + j);
                    }
                }
            }

            InitializePrimitive (device);
        }
    }
}
