using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame.Collisions
{
    public static partial class ShapeCollisions
    {
        public static bool PointToCircle(Vector2 point, Circle circle)
        {
            var distanceSquared = Vector2.DistanceSquared(point, circle.Position);
            var sumOfRadii = 1 + circle.Radius;
            return distanceSquared < sumOfRadii * sumOfRadii;
        }

        public static bool PointToCircle(Vector2 point, Circle circle, out CollisionResult result)
        {
            result = new CollisionResult();

            // avoid the square root until we actually need it
            var distanceSquared = Vector2.DistanceSquared(point, circle.Position);
            var sumOfRadii = 1 + circle.Radius;
            var collided = distanceSquared < sumOfRadii * sumOfRadii;
            if (collided)
            {
                result.Normal = Vector2.Normalize(point - circle.Position);
                var depth = sumOfRadii - MathF.Sqrt(distanceSquared);
                result.MinimumTranslationVector = -depth * result.Normal;
                result.Point = circle.Position + result.Normal * circle.Radius;

                return true;
            }

            return false;
        }

        public static bool PointToBox(Vector2 point, Box box, out CollisionResult result)
        {
            result = new CollisionResult();

            if (box.ContainsPoint(point))
            {
                result.Point = box.BoundingBox.GetClosestPointOnBorderToPoint(point, out result.Normal);
                result.MinimumTranslationVector = point - result.Point;

                return true;
            }

            return false;
        }

        public static bool PointToPoly(Vector2 point, Polygon poly)
        {
            return poly.ContainsPoint(point);
        }

        public static bool PointToPoly(Vector2 point, Polygon poly, out CollisionResult result)
        {
            result = new CollisionResult();

            if (poly.ContainsPoint(point))
            {
                var closestPoint = Polygon.GetClosestPointOnPolygonToPoint(poly.Points, point - poly.Position, out float distanceSquared, out result.Normal);

                result.MinimumTranslationVector = result.Normal * MathF.Sqrt(distanceSquared);
                result.Point = closestPoint + poly.Position;

                return true;
            }

            return false;
        }
    }
}
