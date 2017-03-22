/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/curves-and-splines/ ---
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpecialEffects
{
    using Drawing;

    public class SplineWalker : MonoBehaviour
    {
        public BezierSpline spline;
        public float duration;
        public bool lookingForward;
        public SplineWalkMode walkMode;
        
        private float progress;
        private bool movingForward = true;
        
        private void Update()
        {
            if(movingForward)
            {
                progress += Time.deltaTime / duration;
                
                if(progress > 1.0f)
                {
                    if(walkMode == SplineWalkMode.Once)
                    {
                        progress = 1.0f;
                    }
                    else if(walkMode == SplineWalkMode.Loop)
                    {
                        progress -= 1.0f;
                    }
                    else if(walkMode == SplineWalkMode.BackAndForth)
                    {
                        progress = 2.0f - progress;
                        movingForward = false;
                    }
                }
            }
            else
            {
                progress -= Time.deltaTime / duration;
                
                if(progress < 0f)
                {
                    progress = -progress;
                    movingForward = true;
                }
            }
            
            Vector3 position = spline.GetPointOnCurve(progress);
            transform.localPosition = position;
            
            if(lookingForward)
            {
                transform.LookAt(position + spline.GetDirectionOfPointOnCurve(progress));
            }
        }
        
        [System.Serializable]
        public enum SplineWalkMode
        {
            Once,
            Loop,
            BackAndForth
        }
    }
}

//TODO: Make this universal to the bezier interface; TEST IF THIS ACTUALLY WORKS