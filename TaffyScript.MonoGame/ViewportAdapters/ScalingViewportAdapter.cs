using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.ViewportAdapters
{
    public class ScalingViewportAdapter : ViewportAdapter
    {

        public override int VirtualWidth { get; }
        public override int VirtualHeight { get; }
        public override int ViewportWidth => GraphicsDevice.Viewport.Width;
        public override int ViewportHeight => GraphicsDevice.Viewport.Height;

        public ScalingViewportAdapter(Game game, int virtualWidth, int virtualHeight)
            : base(game)
        {
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
        }

        public override Matrix GetScaleMatrix()
        {
            var scaleX = (float)ViewportWidth / VirtualWidth;
            var scaleY = (float)ViewportHeight / VirtualHeight;
            return Matrix.CreateScale(scaleX, scaleY, 1.0f);
        }
    }
}
