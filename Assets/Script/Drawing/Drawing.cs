/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/curves-and-splines/ ---
 */

using UnityEngine;

namespace Drawing
{
    /// <summary>Provides general Bezier curve functionality.</summary>
    public static class BezierUtility
    {
        /// <summary>Finds a specific point on a quadratic Beizer curve.</summary>
        /// <returns>The coordinates of the point.</returns>
        /// <param name="pointOne">The start point in the quadratic Beizer curve.</param>
        /// <param name="pointTwo">The middle point in the quadratic Beizer curve.</param>
        /// <param name="pointThree">The end point in the quadratic Beizer curve.</param>
        /// <param name="t">The specific point, defined as a normalised interpolant.</param>
        /// <remarks>When we want to find a specific point on a quadratic Beizer curve, we can 
        /// find the point using the following equation:
        /// B(t) = (1 - t)((1 - t)P0 + (t * P1)) + t((1-t)P1 + (t * P2))
        ///      = ((1 - t)^2)P0 + 2(1-t)(t * P1) + (t ^ 2)P2</remarks>
        public static Vector3 GetPoint(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree, 
            float t)
        {
            // Clamp t to 0 - 1 to make sure we have a normalised distance
            t = Mathf.Clamp01(t);
            
            // Create a cached reference to (1 - t), as this equation uses it several times.
            float oneMinusT = 1.0f - t;
            
            // oneMinusT^2 * pointOne + 2 * oneMinusT * t * pointTwo + t^2 * pointThree
            return (oneMinusT * oneMinusT * pointOne 
                + 2.0f * oneMinusT * t * pointTwo 
                + t * t * pointThree);
        }
        
        /// <summary>Finds a specific point on a cubic Beizer curve.</summary>
        /// <returns>The coordinates of the point.</returns>
        /// <param name="pointOne">The first point in the quadratic Beizer curve.</param>
        /// <param name="pointTwo">The second point in the quadratic Beizer curve.</param>
        /// <param name="pointThree">The third point in the quadratic Beizer curve.</param>
        /// <param name="pointFour">The fourth point in the quadratic Beizer curve.</param>
        /// <param name="t">The specific point, defined as a normalised interpolant.</param>
        /// <remarks>When we want to find a specific point on a cubic Beizer curve, we can 
        /// find the point using the following equation:
        /// B(t) = (1 - t)^3 * P0 + 3(1 - t)^2 * tP1 + 3(t - 1)t^2 * P2 + t^3 * P3
        public static Vector3 GetPoint(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree, 
            Vector3 pointFour, float t)
        {
            // Clamp t to 0 - 1 to make sure we have a normalised distance
            t = Mathf.Clamp01(t);

            // Create a cached reference to (1 - t), as this equation uses it several times.
            float oneMinusT = 1.0f - t;

            // oneMinusT^3 * pointOne + 3 * oneMinusT^2 * t * pointTwo + 3 * oneMinusT * t^2 
            // * pointThree + t^3 * pointFour
            return (oneMinusT * oneMinusT * oneMinusT * pointOne
                + 3.0f * oneMinusT * oneMinusT * t * pointTwo
                + 3.0f * oneMinusT * t * t * pointThree
                + t * t * t * pointFour);
        }
        
        /// <summary>Finds the first derivative, or rate of change, at a specific point on a 
        /// quadratic Beizer curve.</summary>
        /// <returns>The rate of change at the specified point.</returns>
        /// <param name="pointOne">The start point in the quadratic Beizer curve.</param>
        /// <param name="pointTwo">The middle point in the quadratic Beizer curve.</param>
        /// <param name="pointThree">The end point in the quadratic Beizer curve.</param>
        /// <param name="t">The specific point, defined as a normalised interpolant.</param>
        /// <remarks>When we want to find a specific point on a quadratic Beizer curve, we can 
        /// find the point using the following equation:
        /// B'(t) = 2(1 - t)(P1 - P0) + 2t(P2 - P1)</remarks>
        public static Vector3 GetFirstDerivative(Vector3 pointOne, Vector3 pointTwo, 
                Vector3 pointThree, float t)
        {
            // Clamp t to 0 - 1 to make sure we have a normalised distance
            t = Mathf.Clamp01(t);
            
            // 2 * (1 - t) * (P1 - P3) + 2 * t * (P3 - P2)
            return (2f * (1.0f - t) * (pointTwo - pointOne) 
                + 2f * t * (pointThree - pointTwo));
        }
        
        /// <summary>Finds the first derivative, or rate of change, at a specific point on a 
        /// cubic Beizer curve.</summary>
        /// <returns>The rate of change at the specified point.</returns>
        /// <param name="pointOne">The first point in the quadratic Beizer curve.</param>
        /// <param name="pointTwo">The second point in the quadratic Beizer curve.</param>
        /// <param name="pointThree">The third point in the quadratic Beizer curve.</param>
        /// <param name="pointFour">The fourth point in the quadratic Beizer curve.</param>
        /// <param name="t">The specific point, defined as a normalised interpolant.</param>
        /// <remarks>When we want to find a specific point on a quadratic Beizer curve, we can 
        /// find the point using the following equation:
        /// B'(t) = 3(1 - t)^2 * (P1 - P0) + 6(1 - t) * t * (P2 - P1) + 3t^2 * (P3 - P2)</remarks>
        public static Vector3 GetFirstDerivative(Vector3 pointOne, Vector3 pointTwo, 
            Vector3 pointThree, Vector3 pointFour, float t)
        {
            // Clamp t to 0 - 1 to make sure we have a normalised distance
            t = Mathf.Clamp01(t);
            
            // Create a cached reference to (1 - t), as this equation uses it several times.
            float oneMinusT = 1.0f - t;

            // 3 * oneMinusT^2 * (pointTwo - pointOne) + 6 * oneMinusT * t 
            // * (pointThree - pointOne) + 3 * t^2 * (pointFour - pointThree)
            return (3.0f * oneMinusT * oneMinusT * (pointTwo - pointOne)
                + 6.0f * oneMinusT * t * (pointThree - pointTwo)
                + 3.0f * t * t * (pointFour - pointThree));
        }
    }

    /// <summary>Represents a smoothing mode, used to determine the logic applied to a bezier curve 
    /// to ensure the line smoothly meets with a connecting bezier curve. Mode changes are 
    /// cumulative.</summary>
    public enum BezierPointMode
    {
        /// <summary>A free point will not have enforced smoothing.</summary>
        Free,
        /// <summary>An aligned point will copy the vector delta across to the opposite point, but 
        /// ensure the opposing point's distance to the middle point is retained.</summary>
        Aligned,
        /// <summary>A mirrored point will copy the vector delta across to the opposite point.
        /// </summary>
        Mirrored
    }

    public interface IBezierInterface
    {
        Vector3 GetDirectionOfPointOnCurve(float t);
        
        Vector3 GetPoint(int pointIndex);
        
        Vector3 GetPointOnCurve(float t);
        
        Vector3 GetVelocityOfPointOnCurve(float t);
        
        void SetPoint(int pointIndex, Vector3 newPoint);
        
        bool Loop { get; }
    }
}

//TODO:Further Spline Implementation: Remove Curves from BezierSpline
//TODO:Further Spline Implementation: Split Curves in BezierSpline
//TODO:Further Spline Implementation: Merge Curves into BezierSpline
//TODO:Explore new curve: Centripetal Catmull-Rom
//TODO:Explore new curve: NURB