using UnityEngine;

namespace Experiments.Features.TransformAnimator
{
    public static class EasingFunctions
    {
        public static float EaseOutElastic(float x)
        {
            float c4 = (2 * Mathf.PI) / 3;
            return x == 0 ? 0 : x == 1 ? 1 : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
        }
    
        public static float EaseOutBounce(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1 / d1)
                return n1 * x * x;
            else if (x < 2 / d1)
            {
                x -= 1.5f / d1;
                return n1 * x * x + 0.75f;
            }
            else if (x < 2.5f / d1)
            {
                x -= 2.25f / d1;
                return n1 * x * x + 0.9375f;
            }
            else
            {
                x -= 2.625f / d1;
                return n1 * x * x + 0.984375f;
            }
        }
    
        public static float EaseInOutCubic(float x)
        {
            return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
        }
    
        public static float EaseOutCubic(float x)
        {
            return 1 - Mathf.Pow(1 - x, 3);
        }
    }
}
