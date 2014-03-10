using System;
using Knot3.Framework.Core;
using Knot3.Framework.Models;
using Microsoft.Xna.Framework;
using Knot3.Framework.Math;
using Knot3.Framework.Utilities;

namespace Examples.TestGame
{
    public class ExampleModel : GameModel
    {
        public ExampleModel (IScreen screen, ExampleModelInfo info)
            : base (screen,info)
        {
        }

        private Angles3 rotation = Angles3.Zero;

        public override void Update (GameTime gameTime)
        {
            rotation.Y += 0.01f;

            Info.Position = (Vector3.Backward * 200).RotateX (rotation.X).RotateY (rotation.Y).RotateZ (rotation.Z);

            base.Update (gameTime);
        }
    }

    public class ExampleModelInfo : GameModelInfo
    {
        public ExampleModelInfo (String modelname)
            : base (modelname)
        {
        }
    }
}

