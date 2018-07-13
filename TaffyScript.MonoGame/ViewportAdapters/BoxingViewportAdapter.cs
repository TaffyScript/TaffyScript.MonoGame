using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.ViewportAdapters
{
    public class BoxingViewportAdapter : ScalingViewportAdapter
    {
        private readonly GameWindow _window;

        /// <summary>
        ///     Size of horizontal bleed areas (from left and right edges) which can be safely cut off
        /// </summary>
        public int HorizontalBleed { get; }

        /// <summary>
        ///     Size of vertical bleed areas (from top and bottom edges) which can be safely cut off
        /// </summary>
        public int VerticalBleed { get; }

        public BoxingMode BoxingMode { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoxingViewportAdapter" />.
        ///     Note: If you're using DirectX please use the other constructor due to a bug in MonoGame.
        ///     https://github.com/mono/MonoGame/issues/4018
        /// </summary>
        public BoxingViewportAdapter(Game game, int virtualWidth,
            int virtualHeight, int horizontalBleed, int verticalBleed)
            : base(game, virtualWidth, virtualHeight)
        {
            _window = game.Window;
            _window.ClientSizeChanged += OnClientSizeChanged;
            HorizontalBleed = horizontalBleed;
            VerticalBleed = verticalBleed;
            OnClientSizeChanged(this, null);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoxingViewportAdapter" />.
        ///     Note: If you're using DirectX please use the other constructor due to a bug in MonoGame.
        ///     https://github.com/mono/MonoGame/issues/4018
        /// </summary>
        public BoxingViewportAdapter(Game game, int virtualWidth,
            int virtualHeight)
            : this(game, virtualWidth, virtualHeight, 0, 0)
        {
        }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var viewport = GraphicsDevice.Viewport;

            var worldScaleX = (float)viewport.Width / VirtualWidth;
            var worldScaleY = (float)viewport.Height / VirtualHeight;

            var safeScaleX = (float)viewport.Width / (VirtualWidth - HorizontalBleed);
            var safeScaleY = (float)viewport.Height / (VirtualHeight - HorizontalBleed);

            var worldScale = MathHelper.Max(worldScaleX, worldScaleY);
            var safeScale = MathHelper.Min(safeScaleX, safeScaleY);
            var scale = MathHelper.Min(worldScale, safeScale);

            var width = (int)(scale * VirtualWidth + 0.5f);
            var height = (int)(scale * VirtualHeight + 0.5f);

            if (height >= viewport.Height && width < viewport.Width)
                BoxingMode = BoxingMode.Pillarbox;
            else if (width >= viewport.Height && height < viewport.Height)
                BoxingMode = BoxingMode.Letterbox;
            else
                BoxingMode = BoxingMode.None;

            var x = (int)((viewport.Width - width) * .5f);
            var y = (int)((viewport.Height - height) * .5f);
            GraphicsDevice.Viewport = new Viewport(x, y, width, height);
        }

        public override void Reset()
        {
            base.Reset();
            OnClientSizeChanged(this, EventArgs.Empty);
        }

        public override Point PointToScreen(int x, int y)
        {
            var viewport = GraphicsDevice.Viewport;
            return base.PointToScreen(x - viewport.X, y - viewport.Y);
        }
    }
}