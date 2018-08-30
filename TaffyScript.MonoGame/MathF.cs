using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace TaffyScript.MonoGame
{
    [TaffyScriptBaseType]
    public static class MathF
    {
        public const float Epsilon = 0.00001f;
        public const float Deg2Rad = 0.0174532924f;
        public const float Rad2Deg = 57.29578f;

        [TaffyScriptMethod]
        public static TsObject point_in_rectangle(TsObject[] args)
        {
            var x = (float)args[0];
            var y = (float)args[1];
            return x >= (float)args[2] && y >= (float)args[3] && x <= (float)args[4] && y <= (float)args[5];
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject length_dir(TsObject[] args)
        {
            var vec = LengthDir((float)args[0], (float)args[1]);
            return new TsObject[] { vec.X, vec.Y };
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject length_dir_x(TsObject[] args)
        {
            return LengthDirX((float)args[0], (float)args[1]);
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject length_dir_y(TsObject[] args)
        {
            return LengthDirY((float)args[0], (float)args[1]);
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject to_radians(TsObject[] args)
        {
            return (float)args[0] * Deg2Rad;
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject to_degrees(TsObject[] args)
        {
            return (float)args[0] * Rad2Deg;
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject floor_ext(TsObject[] args)
        {
            return Floor((float)args[0], (float)args[1]);
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject round_ext(TsObject[] args)
        {
            return Round((float)args[0], (float)args[1]);
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject ceil_ext(TsObject[] args)
        {
            return Ceil((float)args[0], (float)args[1]);
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject point_direction(TsObject[] args)
        {
            return Atan2((float)args[3] - (float)args[1], (float)args[2] - (float)args[0]);
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject point_distance(TsObject[] args)
        {
            return Sqrt(DistanceSquared((float)args[0], (float)args[1], (float)args[2], (float)args[3]));
        }

        [TaffyScriptMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TsObject point_distance_squared(TsObject[] args)
        {
            return DistanceSquared((float)args[0], (float)args[1], (float)args[2], (float)args[3]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 LengthDir(float length, float direction)
        {
            return new Vector2((float)(length * System.Math.Cos(direction)), (float)(length * System.Math.Sin(direction)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthDirX(float length, float direction)
        {
            return (float)(length * System.Math.Cos(direction));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthDirY(float length, float direction)
        {
            return (float)(-length * System.Math.Sin(direction));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(Vector2 p1, Vector2 p2)
        {
            return ((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(float x1, float y1, float x2, float y2)
        {
            return ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
        }

        public static float Direction(float x1, float y1, float x2, float y2)
        {
            var dir = MathHelper.ToDegrees((float)System.Math.Atan2(y1 - y2, x2 - x1));
            if (dir < 0f)
                dir = 360f + dir;
            return dir;
        }

        public static float Direction(Vector2 p1, Vector2 p2)
        {
            var dir = MathHelper.ToDegrees((float)System.Math.Atan2(p1.Y - p2.Y, p2.X - p1.X));
            if (dir < 0f)
                dir = 360f + dir;
            return dir;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float f)
        {
            return (float)System.Math.Floor(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float value, float n)
        {
            return (float)System.Math.Floor(value / n) * n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(float f)
        {
            return (int)System.Math.Floor(f);
        }

        /// <summary>
        /// floors the float to the nearest int value below x. note that this only works for values in the range of short
        /// </summary>
        /// <returns>The floor to int.</returns>
        /// <param name="x">The x coordinate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastFloorToInt(float x)
        {
            // we shift to guaranteed positive before casting then shift back after
            return (int)(x + 32768f) - 32768;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(float f)
        {
            return (float)System.Math.Ceiling(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(float value, float n)
        {
            return (float)System.Math.Ceiling(value / n) * n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(float f)
        {
            return (int)System.Math.Ceiling(f);
        }

        /// <summary>
        /// ceils the float to the nearest int value above y. note that this only works for values in the range of short
        /// </summary>
        /// <returns>The ceil to int.</returns>
        /// <param name="y">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastCeilToInt(float y)
        {
            return 32768 - (int)(32768f - y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float f)
        {
            return (float)System.Math.Round(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value, float n)
        {
            return (float)System.Math.Round(value / n) * n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(float f)
        {
            return (int)System.Math.Round(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;

            return value;
        }

        /// <summary>
        /// clamps value between 0 and 1
        /// </summary>
        /// <param name="value">Value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            if (value < 0f)
                return 0f;

            if (value > 1f)
                return 1f;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * MathF.Clamp01(t);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float from, float to, float t)
        {
            if (from < to)
            {
                if (t < from)
                    return 0.0f;
                else if (t > to)
                    return 1.0f;
            }
            else
            {
                if (t < to)
                    return 1.0f;
                else if (t > from)
                    return 0.0f;
            }

            return (t - from) / (to - from);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float UnclampedLerp(float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        /// <summary>
        /// moves start towards end by shift amount clamping the result. start can be less than or greater than end.
        /// example: start is 2, end is 10, shift is 4 results in 6
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="shift">Shift.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Approach(float start, float end, float shift)
        {
            if (start < end)
                return System.Math.Min(start + shift, end);
            return System.Math.Max(start - shift, end);
        }

        /// <summary>
        /// returns the minimum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinOf(float a, float b, float c)
        {
            return System.Math.Min(a, System.Math.Min(b, c));
        }


        /// <summary>
        /// returns the maximum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxOf(float a, float b, float c)
        {
            return System.Math.Max(a, System.Math.Max(b, c));
        }


        /// <summary>
        /// returns the minimum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="d">D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinOf(float a, float b, float c, float d)
        {
            return System.Math.Min(a, System.Math.Min(b, System.Math.Min(c, d)));
        }


        /// <summary>
        /// returns the minimum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="d">D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinOf(float a, float b, float c, float d, float e)
        {
            return System.Math.Min(a, System.Math.Min(b, System.Math.Min(c, System.Math.Min(d, e))));
        }


        /// <summary>
        /// returns the maximum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="d">D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxOf(float a, float b, float c, float d)
        {
            return System.Math.Max(a, System.Math.Max(b, System.Math.Max(c, d)));
        }


        /// <summary>
        /// returns the maximum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="d">D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxOf(float a, float b, float c, float d, float e)
        {
            return System.Math.Max(a, System.Math.Max(b, System.Math.Max(c, System.Math.Max(d, e))));
        }

        /// <summary>
        /// Returns the square root
        /// </summary>
        /// <param name="val">Value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float val)
        {
            return (float)System.Math.Sqrt(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float x, float y)
        {
            return (float)System.Math.Pow(x, y);
        }

        /// <summary>
        /// Returns the sine of angle in radians
        /// </summary>
        /// <param name="f">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float f)
        {
            return (float)System.Math.Sin(f);
        }

        /// <summary>
        /// Returns the cosine of angle in radians
        /// </summary>
        /// <param name="f">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float f)
        {
            return (float)System.Math.Cos(f);
        }

        /// <summary>
        /// Returns the arc-cosine of f: the angle in radians whose cosine is f
        /// </summary>
        /// <param name="f">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float f)
        {
            return (float)System.Math.Acos(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(float power)
        {
            return (float)System.Math.Exp(power);
        }

        /// <summary>
        /// returns the angle whose tangent is the quotient (y/x) of two specified numbers
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x)
        {
            return (float)System.Math.Atan2(y, x);
        }
    }
}
