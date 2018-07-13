using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TaffyScript.MonoGame.Graphics;

namespace TaffyScript.MonoGame.Collisions
{
    /// <summary>
    /// Essentially represents a transform rather than a shape.
    /// </summary>
    public class NullCollider : Shape
    {
        public override RectangleF BoundingBox => RectangleF.Empty;

        public override string ObjectType => "TaffyScript.MonoGame.Collisions.NullCollider";

        public override bool CollidesWithLine(Vector2 start, Vector2 end)
        {
            return false;
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end, out RaycastHit hit)
        {
            hit = default(RaycastHit);
            return false;
        }

        public override bool CollidesWithPoint(Vector2 point, out CollisionResult result)
        {
            result = default(CollisionResult);
            return false;
        }

        public override bool CollidesWithShape(Shape other, out CollisionResult result)
        {
            result = default(CollisionResult);
            return false;
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return false;
        }

        public override bool Overlaps(Shape other)
        {
            return false;
        }

        public override void DebugDraw(SpriteBatch spriteBatch, Color color)
        {
            Primitives2D.PutPixel(spriteBatch, Position, color);
        }
    }
}
