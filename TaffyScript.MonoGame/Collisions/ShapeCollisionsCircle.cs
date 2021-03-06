﻿using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame.Collisions
{
    public static partial class ShapeCollisions
    {
        public static bool CircleToCircle(Circle first, Circle second)
        {
            var distanceSquared = Vector2.DistanceSquared(first.Position, second.Position);
            var sumOfRadii = first.Radius + second.Radius;
            return distanceSquared < sumOfRadii * sumOfRadii;
        }

        public static bool CircleToCircle(Circle first, Circle second, out CollisionResult result)
        {
            result = new CollisionResult();

            // avoid the square root until we actually need it
            var distanceSquared = Vector2.DistanceSquared(first.Position, second.Position);
            var sumOfRadii = first.Radius + second.Radius;
            var collided = distanceSquared < sumOfRadii * sumOfRadii;
            if (collided)
            {
                result.Normal = Vector2.Normalize(first.Position - second.Position);
                var depth = sumOfRadii - MathF.Sqrt(distanceSquared);
                result.MinimumTranslationVector = -depth * result.Normal;
                result.Point = second.Position + result.Normal * second.Radius;

                return true;
            }

            return false;
        }

        public static bool CircleToBox(Circle circle, Box box)
        {
            if (box.ContainsPoint(circle.Position))
                return true;

            var closestPointOnBounds = box.BoundingBox.GetClosestPointOnBorderToPoint(circle.Position);

            float sqrDistance = Vector2.DistanceSquared(closestPointOnBounds, circle.Position);

            // see if the point on the box is less than radius from the circle
            if (sqrDistance <= circle.Radius * circle.Radius)
                return true;

            return false;
        }

        public static bool CircleToBox(Circle circle, Box box, out CollisionResult result)
        {
            result = new CollisionResult();

            var closestPointOnBounds = box.BoundingBox.GetClosestPointOnBorderToPoint(circle.Position, out result.Normal);

            // deal with circles whos center is in the box first since its cheaper to see if we are contained
            if (box.ContainsPoint(circle.Position))
            {
                result.Point = closestPointOnBounds;

                // calculate mtv. Find the safe, non-collided position and get the mtv from that.
                var safePlace = closestPointOnBounds + result.Normal * circle.Radius;
                result.MinimumTranslationVector = circle.Position - safePlace;

                return true;
            }

            float sqrDistance = Vector2.DistanceSquared(closestPointOnBounds, circle.Position);

            // see if the point on the box is less than radius from the circle
            if (sqrDistance == 0)
            {
                result.MinimumTranslationVector = result.Normal * circle.Radius;
            }
            else if (sqrDistance <= circle.Radius * circle.Radius)
            {
                result.Normal = circle.Position - closestPointOnBounds;
                var depth = result.Normal.Length() - circle.Radius;

                result.Point = closestPointOnBounds;
                Vector2Ext.Normalize(ref result.Normal);
                result.MinimumTranslationVector = depth * result.Normal;

                return true;
            }

            return false;
        }

        public static bool CircleToPolygon(Circle circle, Polygon polygon)
        {

            // circle TruePosition in the polygons coordinates
            var poly2Circle = circle.Position - polygon.Position;

            // first, we need to find the closest distance from the circle to the polygon
            var closestPoint = Polygon.GetClosestPointOnPolygonToPoint(polygon.Points, poly2Circle, out float distanceSquared);

            var circleCenterInsidePoly = polygon.ContainsPoint(circle.Position);
            if (distanceSquared > circle.Radius * circle.Radius && !circleCenterInsidePoly)
                return false;

            return true;
        }

        public static bool CircleToPolygon(Circle circle, Polygon polygon, out CollisionResult result)
        {
            result = new CollisionResult();

            // circle TruePosition in the polygons coordinates
            var poly2Circle = circle.Position - polygon.Position;

            // first, we need to find the closest distance from the circle to the polygon
            var closestPoint = Polygon.GetClosestPointOnPolygonToPoint(polygon.Points, poly2Circle, out float distanceSquared, out result.Normal);

            // make sure the squared distance is less than our radius squared else we are not colliding. Note that if the Circle is fully
            // contained in the Polygon the distance could be larger than the radius. Because of that we also  make sure the circle TruePosition
            // is not inside the poly.
            var circleCenterInsidePoly = polygon.ContainsPoint(circle.Position);
            if (distanceSquared > circle.Radius * circle.Radius && !circleCenterInsidePoly)
                return false;

            // figure out the mtv. We have to be careful to deal with circles fully contained in the polygon or with their center contained.
            Vector2 mtv;
            if (circleCenterInsidePoly)
            {
                mtv = result.Normal * (MathF.Sqrt(distanceSquared) - circle.Radius);
            }
            else
            {
                // if we have no distance that means the circle center is on the polygon edge. Move it only by its radius
                if (distanceSquared == 0)
                    mtv = result.Normal * circle.Radius;
                else
                {
                    var distance = MathF.Sqrt(distanceSquared);
                    mtv = -(poly2Circle - closestPoint) * ((circle.Radius - distance) / distance);
                }
            }

            result.MinimumTranslationVector = mtv;
            result.Point = closestPoint + polygon.Position;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
        {
            var v = lineB - lineA;
            var w = closestTo - lineA;
            var t = Vector2.Dot(w, v) / Vector2.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }
    }
}
