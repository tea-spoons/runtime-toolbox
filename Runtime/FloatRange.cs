
namespace TeaSpoons.RuntimeToolbox
{
    using UnityEngine;

    /// <summary>
    /// Serializable struct for a min-max range of floats.
    /// </summary>
    /// <remarks>
    /// Accepts negative ranges (min > max).
    /// </remarks>
    [System.Serializable]
    public struct FloatRange
    {
        /// <summary>
        /// Creates a positive <see cref="FloatRange"/>, meaning that <see cref="Max"/> is greater than or equal to <see cref="Min"/>.
        /// </summary>
        public static FloatRange PositiveRange(float a, float b)
        {
            if (a <= b)
            {
                return new FloatRange(a, b);
            }
            else
            {
                return new FloatRange(b, a);
            }
        }

        public float Min;
        public float Max;

        public float Range => Max - Min;
        public float AbsoluteRange => Mathf.Abs(Range);

        public FloatRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Lerp(float t)
        {
            return Mathf.Lerp(Min, Max, t);
        }

        public float Clamp(float f)
        {
            return Mathf.Clamp(f, Min, Max);
        }

        public float GetRandom()
        {
            return Random.Range(Min, Max);
        }

        public bool Contains(float f)
        {
            return f >= Min && f <= Max;
        }

        public static FloatRange operator *(FloatRange range, float m)
        {
            return new FloatRange(range.Min * m, range.Max * m);
        }

        public static FloatRange operator /(FloatRange range, float d)
        {
            return new FloatRange(range.Min / d, range.Max / d);
        }

        public override string ToString()
        {
            return ($"FloatRange[Min={Min}, Max={Max}]");
        }
    }
}
