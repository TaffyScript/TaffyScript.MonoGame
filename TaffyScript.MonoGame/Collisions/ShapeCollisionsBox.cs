using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame.Collisions
{
    public static partial class ShapeCollisions
    {
        public static bool BoxToBox(Box first, Box second, out CollisionResult result)
        {
            result = new CollisionResult();

            var diff = MinkowskiDifference(first, second);
            if (diff.Contains(0f, 0f))
            {
                result.MinimumTranslationVector = diff.GetClosestPointOnBoundsToOrigin();

                if (result.MinimumTranslationVector == Vector2.Zero)
                    return false;

                result.Normal = -result.MinimumTranslationVector;
                result.Normal.Normalize();

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static RectangleF MinkowskiDifference(Box first, Box second)
        {
            var topLeft = first.BoundingBox.Position - second.BoundingBox.Max;
            var fullSize = first.BoundingBox.Size + second.BoundingBox.Size;

            return new RectangleF(topLeft, fullSize);
        }
    }
}
