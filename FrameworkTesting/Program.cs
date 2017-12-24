using System;
using System.Collections.Generic;
using osu.Framework;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.OpenGL.Textures;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;
using osu.Framework.Platform;
using OpenTK;
using OpenTK.Input;
using osu.Framework.Graphics;

namespace FrameworkTesting
{
    class Program
    {
        static void Main()
        {
            using (DesktopGameHost host = Host.GetSuitableHost(@"Tester", true))
            {
                host.Run(new TestGame());
            }
        }
    }

    public class TestGame : Game
    {
        private Path path;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Add(new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = path = new Path
                {
                    Origin = Anchor.Centre,
                    PathWidth = 25,
                }
            });

            int textureWidth = (int)path.PathWidth * 2;

            var texture = new Texture(textureWidth, 1);

            //initialise background
            var upload = new TextureUpload(textureWidth * 4);
            var bytes = upload.Data;

            const float aa_portion = .5f;
            const float border_portion = 0.5f;

            for (int i = 0; i < textureWidth; i++)
            {
                float progress = (float)i / (textureWidth - 1);

                if (progress <= border_portion)
                {
                    bytes[i * 4] = 255;
                    bytes[i * 4 + 1] = 255;
                    bytes[i * 4 + 2] = 255;
                    bytes[i * 4 + 3] = (byte)(Math.Min(progress / aa_portion, 1) * 255);
                }
                else
                {

                    bytes[i * 4] = 255;
                    bytes[i * 4 + 1] = 255;
                    bytes[i * 4 + 2] = 255;
                    bytes[i * 4 + 3] = 255;
                }
            }

            texture.SetData(upload);
            path.Texture = texture;
        }

        private readonly List<Vector2> points = new List<Vector2>
        {
            new Vector2(0, 120),
            new Vector2(200, 300),
            new Vector2(500, 350)
        };

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (!args.Repeat && args.Key == Key.F)
            {
                path.ClearVertices();
                /*if (points.Count >= 3)
                {
                    var ps = new osu.Game.Rulesets.Objects.BezierApproximator(points).CreateBezier();
                    foreach (var point in ps)
                        path.AddVertex(point);
                }
                else
                {*/
                foreach (var point in points)
                    path.AddVertex(point);
                //}
                points.RemoveAt(0);
            }
            return base.OnKeyDown(state, args);
        }
    }
}
