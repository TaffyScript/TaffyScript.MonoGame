using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame.Collisions
{
    [TaffyScriptObject]
    public class Box : Polygon
    {
        private float _width;
        private float _height;

        public float Width
        {
            get => _width;
            set
            {
                if (value == _width)
                    return;
                _width = value;
                _originalPoints = BuildBox(value, _height);
                _isDirty = true;
            }
        }

        public float Height
        {
            get => _height;
            set
            {
                if (value == _height)
                    return;
                _height = value;
                _originalPoints = BuildBox(_width, value);
                _isDirty = true;
            }
        }

        public Vector2 Size
        {
            get => new Vector2(_width, _height);
            set
            {
                if (value.X == _width && value.Y == _height)
                    return;
                _width = value.X;
                _height = value.Y;
                _originalPoints = BuildBox(_width, _height);
                _isDirty = true;
            }
        }

        public Box(float width, float height) : base(width, height)
        {
            _width = width;
            _height = height;
        }

        public Box(TsObject[] args)
            : this((float)args[0], (float)args[1])
        {
        }

        public override bool Overlaps(Shape other)
        {
            if (IsUnrotated)
            {
                if (other is Box box && box.IsUnrotated)
                    return BoundingBox.Intersects(other.BoundingBox);
                if (other is Circle circle)
                    return ShapeCollisions.CircleToBox(circle, this);
            }

            return base.Overlaps(other);
        }

        public override bool CollidesWithShape(Shape other, out CollisionResult result)
        {
            if (IsUnrotated && other is Box box && box.IsUnrotated)
                return ShapeCollisions.BoxToBox(this, box, out result);

            return base.CollidesWithShape(other, out result);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            if (IsUnrotated)
                return BoundingBox.Contains(point);

            return base.ContainsPoint(point);
        }

        public override bool CollidesWithPoint(Vector2 point, out CollisionResult result)
        {
            if (IsUnrotated)
                return ShapeCollisions.PointToBox(point, this, out result);

            return base.CollidesWithPoint(point, out result);
        }
    }
}
