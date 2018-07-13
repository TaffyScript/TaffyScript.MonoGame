﻿using System;
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
            if (Math.Sign(Normal.X) != Math.Sign(deltaMovement.X) || (deltaMovement.X == 0 && Normal.X != 0))
            {
                var responseDistance = MinimumTranslationVector.Length();
                var fix = responseDistance / Normal.Y;

                if (Math.Abs(Normal.X) != 1 && Math.Abs(fix) < Math.Abs(deltaMovement.Y * 3f))
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
