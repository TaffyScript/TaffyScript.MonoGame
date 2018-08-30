using System;
using Microsoft.Xna.Framework;

namespace TaffyScript.MonoGame.Collisions
{
    public struct CollisionResult
    {
        public Vector2 Normal;
        public Vector2 MinimumTranslationVector;
        public Vector2 Point;

        public void RemoveHorizontalTranslation(Vector2 deltaMovement)
        {
            if (System.Math.Sign(Normal.X) != System.Math.Sign(deltaMovement.X) || (deltaMovement.X == 0 && Normal.X != 0))
            {
                var responseDistance = MinimumTranslationVector.Length();
                var fix = responseDistance / Normal.Y;

                if (System.Math.Abs(Normal.X) != 1 && System.Math.Abs(fix) < System.Math.Abs(deltaMovement.Y * 3f))
                {
                    MinimumTranslationVector = new Vector2(0, -fix);
                }
            }
        }

        public void InvertResult()
        {
            Vector2.Negate(ref MinimumTranslationVector, out MinimumTranslationVector);
            Vector2.Negate(ref Normal, out Normal);
        }

        public override string ToString()
        {
            return $"[CollisionResult] Normal: {Normal} || TranslationVector: {MinimumTranslationVector}";
        }
    }
}
