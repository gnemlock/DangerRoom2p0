using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Debugging.Physics
{
    #if UNITY_EDITOR
    using Log = Utility.DebuggingPhysicsDebug;
    using StringFormats = Utility.DebuggingPhysicsStringFormats;

    [RequireComponent(typeof(Rigidbody))] public class VelocityGrapher : MonoBehaviour
    {
        public int sampleSize = 100;
        float sampleLength;
        public float maximumVelocity;
        public float minimumVelocity;
        public float[] velocitySamples;
        public Rigidbody rigidbody;

        void Start()
        {
            sampleLength = Time.fixedDeltaTime * (float)sampleSize;

            rigidbody = GetComponent<Rigidbody>();

            StartCoroutine(LateFixedUpdate());
        }

        IEnumerator LateFixedUpdate()
        {
            while(true)
            {
                yield return new WaitForFixedUpdate();

                PushVelocityValue(rigidbody.velocity.magnitude);
            }
        }

        public void AdjustSampleRate(int newSampleSize)
        {
            sampleLength = Time.fixedDeltaTime * newSampleSize;

            if(newSampleSize != sampleSize)
            {
                DataStructures.ArrayManipulation
                    .ResizeForwardArray<float>(ref velocitySamples, newSampleSize);

                sampleSize = newSampleSize;
            }
        }

        public void AdjustSampleRate(float sampleLength)
        {
            int sampleSize = (int)(sampleLength / Time.fixedDeltaTime);
            AdjustSampleRate(sampleSize);
        }

        private void PushVelocityValue(float velocityMagnitude)
        {
            if(velocityMagnitude > maximumVelocity)
            {
                velocityMagnitude = maximumVelocity;
            }
            else if(velocityMagnitude < minimumVelocity)
            {
                velocityMagnitude = minimumVelocity;
            }

            int lastIndex = velocitySamples.Length - 1;

            for(int i = 1; i < lastIndex; i++)
            {
                velocitySamples[i - 1] = velocitySamples[i];
            }

            velocitySamples[lastIndex] = velocityMagnitude;
        }

        public override string ToString()
        {
            if(velocitySamples != null)
            {
                if(velocitySamples.Length != 0)
                {
                    return string
                        .Format(StringFormats.velocityGrapherBase, velocitySamples[0], ToString(1));
                }
                else
                {
                    return string.Format(StringFormats.velocityGrapherBase, "", "");
                }
            }
            else
            {
                return StringFormats.velocityGrapherIsNull;
            }
        }

        private string ToString(int index)
        {
            if(velocitySamples != null && velocitySamples.Length > index)
            {
                return string.Format(StringFormats.velocityGrapherContent, velocitySamples[index])
                    + ToString(index + 1);
            }
            else
            {
                return "";
            }
        }
    }
    #endif
}

namespace Debugging.Physics.Utility
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(VelocityGrapher))] public class VelocityGrapherEditor : Editor
    {
        private float horizontalIncrement;
        private float verticalIncrement;
        private float currentWidth;
        private bool involveZero;

        void OnEnable()
        {
            AdjustInternalMeasurements();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            VelocityGrapher velocityGrapher = target as VelocityGrapher;

            if(GUILayout.Button(DebuggingPhysicsLabels.testArrays))
            {
                DataStructures.ArrayManipulation.TestForwardArrayAdjustment();
            }

            if(GUILayout.Button("TestOutput"))
            {
                Debug.Log(Screen.width); // < definately pings back the inspector width
            }

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            //DrawVelocityGraph(velocityGrapher.velocitySamples, velocityGrapher.minimumVelocity, 
            //    velocityGrapher.maximumVelocity);
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            GL.Color(Color.red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(10, 10, 10);
            GL.End();
            GL.PopMatrix();
            GUILayout.EndHorizontal();
        }
//TODO:Account for cases where user provides a minimumvelocity higher than their maximumvelocity
        private void AdjustInternalMeasurements()
        {
            VelocityGrapher velocityGrapher = target as VelocityGrapher;
            currentWidth 
                = Screen.width - (VelocityGrapherDimensions.velocityGrapherBorderWidth * 2f);
            int sampleCount = velocityGrapher.velocitySamples.Length;
            horizontalIncrement = (float)sampleCount / currentWidth;

            float minimumVelocity = velocityGrapher.minimumVelocity;
            float maximumVelocity = velocityGrapher.maximumVelocity;

            if((minimumVelocity < 0 && maximumVelocity > 0)
               || minimumVelocity == 0 || maximumVelocity == 0)
            {
                involveZero = true;
            }
            else
            {
                involveZero = false;
            }

            float velocityBreadth = maximumVelocity - minimumVelocity;
            horizontalIncrement = velocityBreadth / VelocityGrapherDimensions.velocityGrapherHeight;
        }

        private void DrawVelocityGraph(float[] velocityValues, float minimumVelocity, 
            float maximumVelocity)
        {
            float velocityValue = velocityValues[0];
            float increment = 0;
            Vector3 lineStart = Vector3.zero;
            Vector3 lineEnd  = new Vector3(horizontalIncrement, velocityValue * verticalIncrement);
            
            GL.Begin(GL.LINES);

            for(int i = 1; i < velocityValues.Length; i++)
            {
                if(velocityValue == velocityValues[i] 
                    && (velocityValue == minimumVelocity || velocityValue == maximumVelocity))
                {
                    Handles.color = VelocityGrapherColours.outOfRangeColour;
                }
                else
                {
                    velocityValue = velocityValues[i];
                    Handles.color = VelocityGrapherColours.lineColour;
                }

                lineStart = lineEnd;
                lineEnd.x += increment;
                lineEnd.y = velocityValue * verticalIncrement;

                GL.Vertex(lineStart);
                GL.Vertex(lineEnd);
            }

            GL.End();
        }
    }

    public static class VelocityGrapherColours
    {
        public static Color lineColour = Color.blue;
        public static Color outOfRangeColour = Color.red;
        public static Color zeroLineColour = Color.white;
    }

    public static class VelocityGrapherDimensions
    {
        public const float velocityGrapherHeight = 100f;
        public const float velocityGrapherBorderWidth = 0.1f;
        public const float velocityGrapherLineWidth = 0.5f;
    }

    public static partial class DebuggingPhysicsLabels
    {
        public const string testArrays = "Test Array Resizing";
    }

    public static partial class DebuggingPhysicsDebug
    {
    }

    public static partial class DebuggingPhysicsStringFormats
    {
        public const string velocityGrapherBase = "VelocityGrapher [{0}{1}]";
        public const string velocityGrapherContent = ", {0}";
        public const string velocityGrapherIsNull = "VelocityGrapher does not currently contain " 
            + "any velocity samples.";
    }
    #endif
}