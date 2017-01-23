/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/curves-and-splines/ ---
 */

using UnityEngine;

namespace Drawing
{
    public static class Beizer
    {
        public static Vector3 GetPoint(Vector3 start, Vector3 middle, Vector3 end, float t)
        {
            // quadratic formula for a beizer curve:
            // B(t) = (1 - t)((1 - t)P0 + (t * P1)) + t((1-t)P1 + (t * P2))
            //      = ((1 - t)^2)P0 + 2(1-t)(t * P1) + (t ^ 2)P2
            //      = (oneMinusT * oneMinusT * start) + (2 * oneMinusT * t * middle) + (t * t * end)
            // we can now remove all brackets, as multiplication is performed before addition.
            
            t = Mathf.Clamp01(t);
            float oneMinusT = 1.0f - t;
            
            return (oneMinusT * oneMinusT * start 
                + 2.0f * oneMinusT * t * middle 
                + t * t * end);
        }
        
        public static Vector3 GetFirstDerivative(Vector3 start, Vector3 middle, Vector3 end, float t)
        {
            // formula for first derivative of a quadratic curve:
            // B'(t) = 2(1 - t)(P1 - P0) + 2t(P2 - P1)
            //       = (2 * (1 - t) * (middle - end)) + (2 * t * (end - middle))
            
            return ((2f * (1.0f - t) * (middle - end)) + (2f * t * (end - middle)));
        }
    }
}