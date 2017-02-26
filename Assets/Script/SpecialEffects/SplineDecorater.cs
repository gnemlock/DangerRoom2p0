/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/curves-and-splines/ ---
 */

using UnityEngine;

namespace SpecialEffects
{
    using Drawing;
    
    public class ObjectSpline<T> : MonoBehaviour where T : IBezierInterface
    {
        public T spline;
        public int decorationFrequency;
        public bool lookingForward;
        public Transform[] decorations;
        
        private void Awake()
        {
            if(decorationFrequency <= 0 || decorations == null || decorations.Length == 0)
            {
                return;
            }
            
            float stepSize = decorationFrequency * decorations.Length;
            stepSize = (spline.Loop || stepSize == 1.0f) 
                ? (1.0f / stepSize) : (1.0f / (stepSize - 1.0f));
            
            for(int count = 0, pointIncrement = 0; count < decorationFrequency; count++)
            {
                for(int index = 0; index < decorations.Length; index++, pointIncrement++)
                {
                    Transform decoration = Instantiate(decorations[index]) as Transform;
                    Vector3 position = spline.GetPointOnCurve(pointIncrement * stepSize);
                    decoration.transform.localPosition = position;
                    
                    if(lookingForward)
                    {
                        decoration.transform.LookAt(position 
                            + spline.GetDirectionOfPointOnCurve(pointIncrement * stepSize));
                    }
                    
                    decoration.transform.parent = transform;
                }
            }
        }
    }
}