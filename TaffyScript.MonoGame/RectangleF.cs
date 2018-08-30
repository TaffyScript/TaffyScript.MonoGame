using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        #region Fields

        public float X;
        public float Y;
        public float Width;
        public float Height;

        #endregion

        #region Properties

        public static RectangleF Empty
        {
            get { return new RectangleF(); }
        }

        public float Top
        {
            get { return Y; }
        }

        public float Left
        {
            get { return X; }
        }

        public float Bottom
        {
            get { return Y + Height; }
        }

        public float Right
        {
            get { return X + Width; }
        }

        public Vector2 Max
        {
            get { return new Vector2(Right, Bottom); }
        }

        public bool IsEmpty
        {
            get
            {
                return (X == 0 && Y == 0 && Width == 0 && Height == 0);
            }
        }

        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public Vector2 Center
        {
            get { return new Vector2(X + (Width * .5f), Y + (Height * .5f)); }
        }

        #endregion

        #region Initialize

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public static RectangleF FromMinMax(Vector2 min, Vector2 max)
        {
            return new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
        }

        public static RectangleF FromMinMax(float minX, float minY, float maxX, float maxY)
        {
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public static RectangleF RectEncompassingPoints(Vector2[] points)
        {
            var minX = points[0].X;
            var minY = points[0].Y;
            var maxX = points[0].X;
            var maxY = points[0].Y;

            for (int i = 1; i < points.Length; i++)
            {
                var point = points[i];
                if (point.X < minX)
                    minX = point.X;
                if (maxX < point.X)
                    maxX = point.X;
                if (point.Y < minY)
                    minY = point.Y;
                if (maxY < point.Y)
                    maxY = point.Y;
            }

            return FromMinMax(minX, minY, maxX, maxY);
        }

        #endregion

        #region Member Methods

        public bool Contains(int x, int y)
        {
            return (Left < x && x < Right && Top < y && y < Bottom);
        }

        public bool Contains(float x, float y)
        {
            return (Left < x && x < Right && Top < y && y < Bottom);
        }

        public bool Contains(Point value)
        {
            return (Left < value.X && value.X < Right && Top < value.Y && value.Y < Bottom);
        }

        public bool Contains(Vector2 value)
        {
            return (Left < value.X && value.X < Right && Top < value.Y && value.Y < Bottom);
        }

        public bool Contains(RectangleF value)
        {
            return (Left <= value.Left && value.Right <= Right && Top <= value.Top && value.Bottom <= Bottom);
        }

        public void Inflate(int xAmount, int yAmount)
        {
            X -= xAmount;
            Y -= yAmount;
            Width += xAmount * 2;
            Height += yAmount * 2;
        }

        public void Inflate(float xAmount, float yAmount)
        {
            X -= xAmount;
            Y -= yAmount;
            Width += xAmount * 2;
            Height += yAmount * 2;
        }

        public bool Intersects(RectangleF value)
        {
            return (value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom);
        }

        public Vector2 GetClosestPointOnBoundsToOrigin()
        {
            var max = Max;
            var minDist = System.Math.Abs(Position.X);
            var boundsPoint = new Vector2(Position.X, 0);

            if (System.Math.Abs(max.X) < minDist)
            {
                minDist = System.Math.Abs(max.X);
                boundsPoint.X = max.X;
                boundsPoint.Y = 0f;
            }

            if (System.Math.Abs(max.Y) < minDist)
            {
                minDist = System.Math.Abs(max.Y);
                boundsPoint.X = 0f;
                boundsPoint.Y = max.Y;
            }

            if (System.Math.Abs(Position.Y) < minDist)
            {
                minDist = System.Math.Abs(Position.Y);
                boundsPoint.X = 0;
                boundsPoint.Y = Position.Y;
            }

            return boundsPoint;
        }

        public Vector2 GetClosestPointOnRectToPoint(Vector2 point)
        {
            var result = new Vector2()
            {
                X = MathHelper.Clamp(point.X, Left, Right),
                Y = MathHelper.Clamp(point.Y, Top, Bottom)
            };
            return result;
        }

        public Vector2 GetClosestPointOnBorderToPoint(Vector2 point)
        {
            var result = new Vector2()
            {
                X = MathHelper.Clamp(point.X, Left, Right),
                Y = MathHelper.Clamp(point.Y, Top, Bottom)
            };

            if (Contains(result))
            {
                var dl = result.X - Left;
                var dr = Right - result.X;
                var dt = result.Y - Top;
                var db = Bottom - result.Y;

                var min = MathF.MinOf(dl, dr, dt, db);
                if (min == dt)
                    result.Y = Top;
                else if (min == db)
                    result.Y = Bottom;
                else if (min == dl)
                    result.X = Left;
                else
                    result.X = Right;
            }

            return result;
        }

        public Vector2 GetClosestPointOnBorderToPoint(Vector2 point, out Vector2 edgeNormal)
        {
            edgeNormal = Vector2.Zero;

            var result = new Vector2()
            {
                X = MathHelper.Clamp(point.X, Left, Right),
                Y = MathHelper.Clamp(point.Y, Top, Bottom)
            };

            if (Contains(result))
            {
                var dl = result.X - Left;
                var dr = Right - result.X;
                var dt = result.Y - Top;
                var db = Bottom - result.Y;

                var min = MathF.MinOf(dl, dr, dt, db);
                if (min == dt)
                {
                    result.Y = Top;
                    edgeNormal.Y = -1;
                }
                else if (min == db)
                {
                    result.Y = Bottom;
                    edgeNormal.Y = 1;
                }
                else if (min == dl)
                {

                    result.X = Left;
                    edgeNormal.X = -1;
                }
                else
                {
                    result.X = Right;
                    edgeNormal.X = 1;
                }
            }
            else
            {
                if (result.X == Left)
                    edgeNormal.X = -1;
                if (result.X == Right)
                    edgeNormal.X = 1;
                if (result.Y == Top)
                    edgeNormal.Y = -1;
                if (result.Y == Bottom)
                    edgeNormal.Y = 1;
            }

            return result;
        }

        public void Offset(int xOffset, int yOffset)
        {
            X += xOffset;
            Y += yOffset;
        }

        public void Offset(float xOffset, float yOffset)
        {
            X += xOffset;
            Y += yOffset;
        }

        public void Offset(Point amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        public void Offset(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        #endregion

        #region Static Methods

        public static bool IntersectArea(RectangleF value1, RectangleF value2, out RectangleF intersect)
        {
            if (value1.Intersects(value2))
            {
                var top = System.Math.Max(value1.Top, value2.Top);
                var left = System.Math.Max(value1.Left, value2.Left);
                var bottom = System.Math.Min(value1.Bottom, value2.Bottom);
                var right = System.Math.Min(value1.Right, value2.Right);
                intersect = new RectangleF(left, top, right - left, bottom - top);
                return true;
            }
            intersect = Empty;
            return false;
        }

        public static RectangleF Union(RectangleF value1, RectangleF value2)
        {
            var x = System.Math.Min(value1.X, value2.X);
            var y = System.Math.Min(value1.Y, value2.Y);
            var width = System.Math.Max(value1.Right, value2.Right) - x;
            var height = System.Math.Max(value1.Bottom, value2.Bottom) - y;
            return new RectangleF(x, y, width, height);
        }

        public static Vector2 GetIntersectionDepth(ref RectangleF rectA, ref RectangleF rectB)
        {
            // calculate half sizes
            var halfWidthA = rectA.Width / 2.0f;
            var halfHeightA = rectA.Height / 2.0f;
            var halfWidthB = rectB.Width / 2.0f;
            var halfHeightB = rectB.Height / 2.0f;

            // calculate centers
            var centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            var centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // calculate current and minimum-non-intersecting distances between centers
            var distanceX = centerA.X - centerB.X;
            var distanceY = centerA.Y - centerB.Y;
            var minDistanceX = halfWidthA + halfWidthB;
            var minDistanceY = halfHeightA + halfHeightB;

            // if we are not intersecting at all, return (0, 0)
            if (System.Math.Abs(distanceX) >= minDistanceX || System.Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // calculate and return intersection depths
            var depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            var depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;

            return new Vector2(depthX, depthY);
        }

        #endregion

        #region Operators

        public override bool Equals(object obj)
        {
            return (obj is RectangleF) && this == (RectangleF)obj;
        }

        public bool Equals(RectangleF other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y ^ (int)Width ^ (int)Height;
        }

        public override string ToString()
        {
            return $"X: {X} || Y: {Y} || Width: {Width} || Height: {Height}";
        }

        public static bool operator ==(RectangleF left, RectangleF right)
        {
            return (left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height);
        }

        public static bool operator !=(RectangleF left, RectangleF right)
        {
            return !(left == right);
        }

        public static implicit operator Rectangle(RectangleF value)
        {
            return new Rectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
        }

        public static implicit operator RectangleF(Rectangle value)
        {
            return new RectangleF(value.X, value.Y, value.Width, value.Height);
        }

        #endregion
    }
}
