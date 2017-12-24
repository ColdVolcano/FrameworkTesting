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
using osu.Framework.Graphics.Shapes;
using osu.Framework.MathUtils;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;

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
        private BufferedContainer container;

        protected override void LoadComplete()
        {
            Host.Window.Title = "FrameworkTester";
            base.LoadComplete();

            Add(new Box
            {
                Colour = Color4.Gray,
                RelativeSizeAxes = Axes.Both,
            });
            Add(container = new BufferedContainer
            {
                CacheDrawnFrameBuffer = true,
                Child = path = new Path
                {
                    PathWidth = 50,
                    Name = "Path1",
                    Blending = BlendingMode.None,
                },
            });

            container.Attach(RenderbufferInternalFormat.DepthComponent16);

            populatePoints();

            int textureWidth = (int)path.PathWidth * 2;

            var texture = new Texture(textureWidth, 1);

            //initialise background
            var upload = new TextureUpload(textureWidth * 4);
            var bytes = upload.Data;

            const float aa_portion = .75f;
            const float border_portion = .75f;

            for (int i = 0; i < textureWidth; i++)
            {
                float progress = (float)i / (textureWidth - 1);

                if (progress <= border_portion)
                {
                    bytes[i * 4] = (byte)(255 - 255 * progress);
                    bytes[i * 4 + 1] = (byte)(255 - 255 * progress);
                    bytes[i * 4 + 2] = (byte)(255 - 255 * progress);    
                    bytes[i * 4 + 3] = (byte)(Math.Min(progress / aa_portion, 1) * 255);
                }
                else
                {

                    bytes[i * 4] = (byte)(255 - 255 * progress);
                    bytes[i * 4 + 1] = (byte)(255 - 255 * progress);
                    bytes[i * 4 + 2] = (byte)(255 - 255 * progress);
                    bytes[i * 4 + 3] = 255;
                }
            }

            texture.SetData(upload);
            path.Texture = texture;

            newPoints();
            container.Size = path.Size;
        }

        private List<Vector2> points;

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (args.Key == Key.F)
                newPoints();
            return base.OnKeyDown(state, args);
        }

        private void populatePoints()
        {
            points = new List<Vector2>();
            for (int i = 0; i < 10; i++)
                points.Add(new Vector2(RNG.NextSingle(0, 1850), RNG.NextSingle(0, 1000)));
        }

        private void newPoints()
        {
            path.ClearVertices();
            var ps = new osu.Game.Rulesets.Objects.BezierApproximator(points).CreateBezier();
            foreach (var point in ps)
                path.AddVertex(point);
            container.Size = path.Size;
            points.RemoveAt(0);
            container.ForceRedraw();
            if (points.Count == 1)
                populatePoints();
        }
    }
}
