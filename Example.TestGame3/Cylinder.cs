using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Examples.TestGame3
{
    public class CylinderPrimitive : GeometricPrimitive
    {
        public CylinderPrimitive(GraphicsDevice device)
            : this(device, 1, 1, 32)
        {
        }

        public CylinderPrimitive(GraphicsDevice device, float _height, float diameter, int tessellation)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException("cylinder tessellation");

            float halfHeight = _height / 2;
            float radius = diameter / 2;

            for (int i = 0; i < tessellation; i++)
            {
                Vector3 normal = GetCircleVector(i, tessellation);

                AddVertex(normal * radius + Vector3.Up * halfHeight, normal);
                AddVertex(normal * radius + Vector3.Down * halfHeight, normal);

                AddIndex(i * 2);
                AddIndex(i * 2 + 1);
                AddIndex((i * 2 + 2) % (tessellation * 2));

                AddIndex(i * 2 + 1);
                AddIndex((i * 2 + 3) % (tessellation * 2));
                AddIndex((i * 2 + 2) % (tessellation * 2));
            }

            CreateCap(tessellation, halfHeight, radius, Vector3.Up);
            CreateCap(tessellation, halfHeight, radius, Vector3.Down);

            InitializePrimitive(device);
        }

        void CreateCap(int tessellation, float height, float radius, Vector3 normal)
        {
            // create cap indices.
            for (int i = 0; i < tessellation - 2; i++)
            {
                if (normal.Y > 0)
                {
                    AddIndex(CurrentVertex);
                    AddIndex(CurrentVertex + (i + 1) % tessellation);
                    AddIndex(CurrentVertex + (i + 2) % tessellation);
                }
                else
                {
                    AddIndex(CurrentVertex);
                    AddIndex(CurrentVertex + (i + 2) % tessellation);
                    AddIndex(CurrentVertex + (i + 1) % tessellation);
                }
            }

            // create cap vertices.
            for (int i = 0; i < tessellation; i++)
            {
                Vector3 position = GetCircleVector(i, tessellation) * radius +
                    normal * height;

                AddVertex(position, normal, Vector2.Zero);
            }
        }

        static Vector3 GetCircleVector(int i, int tessellation)
        {
            float angle = i * MathHelper.TwoPi / tessellation;
            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);
            return new Vector3(dx, 0, dz);
        }
    }
}

