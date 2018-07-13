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
    [WeakObject]
    public class Circle : Shape
    {
        private float _radius;
        private bool _isDirty = true;
        private RectangleF _boundingBox;
        private Vector2 _position;

        public float Radius
        {
            get { return _radius; }
            set
            {
                if (value != _radius)
                {
                    _radius = value;
                    _isDirty = true;
                }
            }
        }

        public override Vector2 Position
        {
            get => _position;
            set
            {
                if(value != _position)
                {
                    _position = value;
                    _isDirty = true;
                }
            }
        }

        public override RectangleF BoundingBox
        {
            get
            {
                if (_isDirty)
                    UpdateBoundingBox();
                return _boundingBox;
            }
        }

        public override string ObjectType => "TaffyScript.MonoGame.Collisions.Circle";

        public Circle(float radius)
        {
            Radius = radius;
        }

        public Circle(TsObject[] args)
            : this((float)args[0])
        {
        }

        private void UpdateBoundingBox()
        {
            _boundingBox = new RectangleF(Position.X - _radius, Position.Y - _radius, _radius * 2f, _radius * 2f);
        }

        public override bool Overlaps(Shape other)
        {
            if (other is Box box && box.IsUnrotated)
                return ShapeCollisions.CircleToBox(this, box);
            else if (other is Circle circle)
                return ShapeCollisions.CircleToCircle(this, circle);
            else if (other is Polygon poly)
                return ShapeCollisions.CircleToPolygon(this, poly);
            else if (other is NullCollider)
                return false;

            throw new NotImplementedException($"Overlaps of Circle to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithShape(Shape other, out CollisionResult result)
        {
            if (other is Box box && box.IsUnrotated)
                return ShapeCollisions.CircleToBox(this, box, out result);
            else if (other is Circle circle)
                return ShapeCollisions.CircleToCircle(this, circle, out result);
            else if (other is Polygon poly)
                return ShapeCollisions.CircleToPolygon(this, poly, out result);
            else if(other is NullCollider)
            {
                result = default(CollisionResult);
                return false;
            }

            throw new NotImplementedException($"Overlaps of Circle to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end)
        {
            return ShapeCollisions.LineToCircle(start, end, this);
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end, out RaycastHit hit)
        {
            return ShapeCollisions.LineToCircle(start, end, this, out hit);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return (point - Position).LengthSquared() <= Radius * Radius;
        }

        public override bool CollidesWithPoint(Vector2 point, out CollisionResult result)
        {
            return ShapeCollisions.PointToCircle(point, this, out result);
        }

        public override void DebugDraw(SpriteBatch spriteBatch, Color color)
        {
            Primitives2D.DrawCircle(spriteBatch, Position, Radius, 12, color);
        }
    }
}
