using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TaffyScript.MonoGame.ViewportAdapters
{
    public abstract class ViewportAdapter
    {
        public abstract int VirtualWidth { get; }
        public abstract int VirtualHeight { get; }
        public abstract int ViewportWidth { get; }
        public abstract int ViewportHeight { get; }

        public Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);
        public Point Center => BoundingRectangle.Center;
        public GraphicsDevice GraphicsDevice { get; }
        public Viewport Viewport => GraphicsDevice.Viewport;

        protected ViewportAdapter(Game game)
        {
            GraphicsDevice = game.GraphicsDevice;
        }

        public abstract Matrix GetScaleMatrix();

        public virtual void Reset()
        {
        }

        public Point PointToScreen(Point point)
        {
            return PointToScreen(point.X, point.Y);
        }

        public virtual Point PointToScreen(int x, int y)
        {
            var scale = GetScaleMatrix();
            var inverted = Matrix.Invert(scale);
            return Vector2.Transform(new Vector2(x, y), inverted).ToPoint();
        }
    }
}
