using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TaffyScript.MonoGame.Graphics;

namespace TaffyScript.MonoGame.Collisions
{
    [TaffyScriptObject]
    public class Polygon : Shape
    {
        #region Fields

        private float _rotation = 0;
        private RectangleF _boundingBox;
        private Vector2[] _edgeNormals;
        private Vector2[] _points;
        private Vector2 _center;

        protected bool _isDirty = true;
        protected Vector2[] _originalPoints;

        #endregion

        #region Properties

        public Vector2[] Points
        {
            get
            {
                if (_isDirty)
                    Clean();
                return _points;
            }
        }

        public override RectangleF BoundingBox
        {
            get
            {
                if (_isDirty)
                    Clean();
                return new RectangleF(_boundingBox.Position + Position, _boundingBox.Size);
            }
        }

        public Vector2[] EdgeNormals
        {
            get
            {
                if (_isDirty)
                    Clean();
                return _edgeNormals;
            }
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                if(value != _rotation)
                {
                    _rotation = value;
                    _isDirty = true;
                }
            }
        }

        public Vector2 Center
        {
            get => _center;
            set
            {
                if(value != _center)
                {
                    _center = value;
                    _isDirty = true;
                }
            }
        }

        public bool IsUnrotated { get => _rotation == 0; }

        public override string ObjectType => "TaffyScript.MonoGame.Collisions.Polygon";

        #endregion

        #region Ctor

        public Polygon(Vector2[] points)
            : this(points, Vector2.Zero)
        {
        }

        public Polygon(Vector2[] points, Vector2 center)
        {
            _originalPoints = points;
            _points = new Vector2[points.Length];
            _isDirty = true;
        }

        public Polygon(float width, float height)
            : this(BuildBox(width, height), Vector2.Zero)
        {
        }

        public Polygon(TsObject[] args)
            : this(FromTsObjects(args), Vector2.Zero)
        {
        }

        #endregion

        #region TaffyScript

        public override TsObject GetMember(string name)
        {
            switch(name)
            {
                case "rotation":
                    return _rotation;
                case "center":
                    return new TsObject[] { _center.X, _center.Y };
                case "center_x":
                    return _center.X;
                case "center_y":
                    return _center.Y;
                case "points":
                    //This should only be used while debugging.
                    var result = new TsObject[Points.Length];
                    for (var i = 0; i < result.Length; i++)
                        result[i] = new TsObject[] { _points[i].X, _points[i].Y };
                    return result;
                default:
                    return base.GetMember(name);
            }
        }

        public override void SetMember(string name, TsObject value)
        {
            switch(name)
            {
                case "rotation":
                    Rotation = (float)value;
                    break;
                case "center":
                    var array = value.GetArray();
                    Center = new Vector2((float)array[0], (float)array[1]);
                    break;
                case "center_x":
                    Center = new Vector2((float)value, Center.Y);
                    break;
                case "center_y":
                    Center = new Vector2(Center.X, (float)value);
                    break;
                default:
                    base.SetMember(name, value);
                    break;
            }
        }

        #endregion

        #region Instance Methods

        public override bool Overlaps(Shape other)
        {
            if (other is Polygon poly)
                return ShapeCollisions.PolygonToPolygon(this, poly);

            if (other is Circle circle)
                return ShapeCollisions.CircleToPolygon(circle, this);

            if (other is NullCollider)
                return false;

            throw new NotImplementedException($"Overlaps of Polygon to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithShape(Shape other, out CollisionResult result)
        {
            if (other is Polygon poly)
                return ShapeCollisions.PolygonToPolygon(this, poly, out result);

            if (other is Circle circle)
            {
                if (ShapeCollisions.CircleToPolygon(circle, this, out result))
                {
                    result.InvertResult();
                    return true;
                }
            }

            if(other is NullCollider)
            {
                result = default(CollisionResult);
                return false;
            }

            throw new NotImplementedException($"Collisions of Polygon to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end)
        {
            return ShapeCollisions.LineToPoly(start, end, this);
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end, out RaycastHit hit)
        {
            return ShapeCollisions.LineToPoly(start, end, this, out hit);
        }

        public override bool CollidesWithPoint(Vector2 point, out CollisionResult result)
        {
            return ShapeCollisions.PointToPoly(point, this, out result);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            point -= Position;
            bool isInside = false;
            for (int i = 0, j = _points.Length - 1; i < _points.Length; j = i++)
            {
                if (((_points[i].Y > point.Y) != (_points[j].Y > point.Y)) &&
                     (point.X < (_points[j].X - _points[i].X) * (point.Y - _points[i].Y) / (_points[j].Y - _points[i].Y) + _points[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        private void Clean()
        {
            if (_rotation != 0)
            {
                var cos = MathF.Cos(_rotation);
                var sin = MathF.Sin(_rotation);
                for (var i = 0; i < _originalPoints.Length; i++)
                {
                    var p = _originalPoints[i] - _center;
                    p = new Vector2(cos * p.X - sin * p.Y, sin * p.X + cos * p.Y);

                    _points[i] = p;
                }
            }
            else
            {
                for (var i = 0; i < _originalPoints.Length; i++)
                    _points[i] = _originalPoints[i] - _center;
            }

            BuildEdgeNormals();
            _boundingBox = RectangleF.RectEncompassingPoints(_points);
            _isDirty = false;
        }

        private void BuildEdgeNormals()
        {
            var totalEdges = _points.Length;
            if (_edgeNormals == null || _edgeNormals.Length != totalEdges)
                _edgeNormals = new Vector2[totalEdges];

            Vector2 p2;
            Vector2 p1;
            for (var i = 0; i < totalEdges; i++)
            {
                p1 = _points[i];
                if (i + 1 >= _points.Length)
                    p2 = _points[0];
                else
                    p2 = _points[i + 1];

                var perp = Vector2Ext.Perpendicular(ref p1, ref p2);
                Vector2Ext.Normalize(ref perp);
                _edgeNormals[i] = perp;
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch, Color color)
        {
            Primitives2D.DrawLine(spriteBatch, Points[_points.Length - 1] + Position, _points[0] + Position, color);

            for (var i = 1; i < _points.Length; i++)
                Primitives2D.DrawLine(spriteBatch, _points[i - 1] + Position, _points[i] + Position, color);
        }

        #endregion

        #region Static Methods

        private static Vector2[] FromTsObjects(TsObject[] args)
        {
            Vector2[] points = new Vector2[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var array = args[i].GetArray();
                points[i] = new Vector2((float)array[0], (float)array[1]);
            }
            return points;
        }

        protected static Vector2[] BuildBox(float width, float height)
        {
            return new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(width, 0),
                new Vector2(width, height),
                new Vector2(0, height)
            };
        }

        public static Vector2 FindPolygonCenter(Vector2[] points)
        {
            float x = 0, y = 0;

            for (var i = 0; i < points.Length; i++)
            {
                x += points[i].X;
                y += points[i].Y;
            }

            return new Vector2(x / points.Length, y / points.Length);
        }

        public static Vector2 GetFarthestPointInDirection(Polygon poly, Vector2 direction)
        {
            var points = poly.Points;
            int index = 0;
            Vector2.Dot(ref points[index], ref direction, out float maxDot);

            for (int i = 0; i < points.Length; i++)
            {
                Vector2.Dot(ref points[i], ref direction, out float dot);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    index = i;
                }
            }

            return points[index];
        }

        public static Vector2 GetClosestPointOnPolygonToPoint(Vector2[] points, Vector2 point, out float distanceSquared)
        {
            distanceSquared = float.MaxValue;
            var closestPoint = Vector2.Zero;

            for (var i = 0; i < points.Length; i++)
            {
                var j = i + 1;
                if (j == points.Length)
                    j = 0;

                var closest = ShapeCollisions.ClosestPointOnLine(points[i], points[j], point);
                Vector2.DistanceSquared(ref point, ref closest, out float tempDistanceSquared);

                if (tempDistanceSquared < distanceSquared)
                {
                    distanceSquared = tempDistanceSquared;
                    closestPoint = closest;
                }
            }

            return closestPoint;
        }

        public static Vector2 GetClosestPointOnPolygonToPoint(Vector2[] points, Vector2 point, out float distanceSquared, out Vector2 edgeNormal)
        {
            distanceSquared = float.MaxValue;
            edgeNormal = Vector2.Zero;
            var closestPoint = Vector2.Zero;

            for (var i = 0; i < points.Length; i++)
            {
                var j = i + 1;
                if (j == points.Length)
                    j = 0;

                var closest = ShapeCollisions.ClosestPointOnLine(points[i], points[j], point);
                Vector2.DistanceSquared(ref point, ref closest, out float tempDistanceSquared);

                if (tempDistanceSquared < distanceSquared)
                {
                    distanceSquared = tempDistanceSquared;
                    closestPoint = closest;

                    // get the normal of the line
                    var line = points[j] - points[i];
                    edgeNormal.X = -line.Y;
                    edgeNormal.Y = line.X;
                }
            }

            Vector2Ext.Normalize(ref edgeNormal);

            return closestPoint;
        }

        public static Polygon CreateConvexFromPoints(Vector2[] points)
        {
            Vector2 yMin = points[0];
            int location = 0;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].Y < yMin.Y || (points[i].Y == yMin.Y && points[i].X < yMin.X))
                {
                    yMin = points[i];
                    location = i;
                }
            }

            var temp = points[0];
            points[0] = yMin;
            points[location] = temp;

            List<KeyValuePair<Vector2, float>> order = new List<KeyValuePair<Vector2, float>>();

            for (int i = 1; i < points.Length; i++)
            {
                float angle = (MathF.Atan2(points[i].Y, points[i].X) - MathF.Atan2(yMin.Y, yMin.X)) * MathF.Rad2Deg;
                order.Add(new KeyValuePair<Vector2, float>(points[i], angle));
            }

            order = (from kvp in order
                     orderby kvp.Value
                     select kvp).ToList();

            int m = 1;
            for (int i = 0; i < order.Count; i++)
            {
                int cur = i;
                float curDis = CalculateDistance(yMin, order[i].Key);
                while (i < order.Count - 1 && order[i].Value == order[i + 1].Value)
                {
                    i++;
                    var d = CalculateDistance(yMin, order[i].Key);
                    if (d > curDis)
                    {
                        curDis = d;
                        cur = i;
                    }
                }
                points[m] = order[cur].Key;
                m++;
            }

            if (m < 3)
            {
                return null;
            }

            Stack<Vector2> make = new Stack<Vector2>();

            make.Push(points[0]);
            make.Push(points[1]);
            make.Push(points[2]);

            for (int i = 3; i < m; i++)
            {
                while (Orientation(NextToTop(make), make.Peek(), points[i]) != 2)
                {
                    Console.WriteLine(make.Pop());
                }
                make.Push(points[i]);
            }

            return new Polygon(make.ToArray());

            float CalculateDistance(Vector2 p1, Vector2 p2)
            {
                return (((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y)));
            }

            int Orientation(Vector2 p, Vector2 q, Vector2 r)
            {
                float val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
                if (val == 0)
                    return 0;
                return (val > 0) ? 1 : 2;

            }

            Vector2 NextToTop(Stack<Vector2> s)
            {
                Vector2 p = s.Peek();
                s.Pop();
                Vector2 ret = s.Peek();
                s.Push(p);
                return ret;
            }
        }

        #endregion
    }
}