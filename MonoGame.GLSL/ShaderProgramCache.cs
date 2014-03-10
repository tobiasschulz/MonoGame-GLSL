using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.GLSL
{
    internal struct ShaderProgramInfo
    {
        public int program;
        public int posFixupLoc;
    }

    /// <summary>
    /// This class is used to Cache the links between Vertex/Pixel Shaders and Constant Buffers.
    /// It will be responsible for linking the programs under OpenGL if they have not been linked
    /// before. If an existing link exists it will be resused.
    /// </summary>
    internal class ShaderProgramCache : IDisposable
    {
        private readonly Dictionary<int, ShaderProgramInfo> _programCache = new Dictionary<int, ShaderProgramInfo> ();
        bool disposed;

        ~ShaderProgramCache ()
        {
            Dispose (false);
        }

        /// <summary>
        /// Clear the program cache releasing all shader programs.
        /// </summary>
        public void Clear ()
        {
            foreach (var pair in _programCache) {
                if (GL.IsProgram (pair.Value.program)) {
                    GL.DeleteProgram (pair.Value.program);
                    GraphicsExtensions.CheckGLError ();
                }
            }
            _programCache.Clear ();
        }

        public ShaderProgramInfo GetProgramInfo (GLShader vertexShader, GLShader pixelShader)
        {
            // TODO: We should be hashing in the mix of constant 
            // buffers here as well.  This would allow us to optimize
            // setting uniforms to only when a constant buffer changes.

            var key = vertexShader.HashKey | pixelShader.HashKey;
            if (!_programCache.ContainsKey (key)) {
                // the key does not exist so we need to link the programs
                Link (vertexShader, pixelShader);    
            }

            return _programCache [key];
        }

        private void Link (GLShader vertexShader, GLShader pixelShader)
        {
            // NOTE: No need to worry about background threads here
            // as this is only called at draw time when we're in the
            // main drawing thread.
            var program = GL.CreateProgram ();
            GraphicsExtensions.CheckGLError ();

            GL.AttachShader (program, vertexShader.GetShaderHandle ());
            GraphicsExtensions.CheckGLError ();

            GL.AttachShader (program, pixelShader.GetShaderHandle ());
            GraphicsExtensions.CheckGLError ();

            //vertexShader.BindVertexAttributes(program);

            GL.LinkProgram (program);
            GraphicsExtensions.CheckGLError ();

            GL.UseProgram (program);
            GraphicsExtensions.CheckGLError ();

            vertexShader.GetVertexAttributeLocations (program);

            pixelShader.ApplySamplerTextureUnits (program);

            var linked = 0;

            GL.GetProgram (program, ProgramParameter.LinkStatus, out linked);
            GraphicsExtensions.LogGLError ("VertexShaderCache.Link(), GL.GetProgram");
            if (linked == 0) {
                var log = GL.GetProgramInfoLog (program);
                Console.WriteLine (log);
                GL.DetachShader (program, vertexShader.GetShaderHandle ());
                GL.DetachShader (program, pixelShader.GetShaderHandle ());
                GL.DeleteProgram (program);
                throw new InvalidOperationException ("Unable to link effect program");
            }

            ShaderProgramInfo info;
            info.program = program;
            info.posFixupLoc = GL.GetUniformLocation (program, "posFixup");

            _programCache.Add (vertexShader.HashKey | pixelShader.HashKey, info);
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!disposed) {
                if (disposing)
                    Clear ();
                disposed = true;
            }
        }
    }
}
