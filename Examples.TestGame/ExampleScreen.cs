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
using Knot3.Framework.Core;
using Microsoft.Xna.Framework;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using System.IO;
using Knot3.Framework.Effects;

namespace Examples.TestGame
{
    public class ExampleScreen : Screen
    {
        private World world;

        public ExampleScreen (GameCore game)
            : base(game)
        {
            //BackgroundColor = Color.CornflowerBlue;

            IRenderEffect glEffect = new ExampleRenderEffect (screen: this);
            //this.PostProcessingEffect = glEffect;

            world = new World (screen: this, drawOrder: DisplayLayer.GameWorld, effect: glEffect, bounds: Bounds);
            //world = new World (screen: this, drawOrder: DisplayLayer.GameWorld, bounds: Bounds);
            world.Camera.Position = new Vector3 (500, 200, 500);
            world.Camera.Target = new Vector3 (0, 0, 0);
            
            Log.Message ("Content Directory: ", Path.GetFullPath (SystemInfo.RelativeContentDirectory));
            ExampleModelInfo modelInfo = new ExampleModelInfo (modelname: "test") { Scale = Vector3.One / 10 };
            ExampleModel model = new ExampleModel (screen: this, info: modelInfo);
            world.Add (obj: model);
        }

        public override void Draw (GameTime time)
        {
        }

        public override void Update (GameTime time)
        {
        }

        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, world);
        }
    }
}
