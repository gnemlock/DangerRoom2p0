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
        public float maximumVelocity;
        public float minimumVelocity;
        public float[] velocitySamples;
        public Rigidbody rigidbody;
        //TODO:Currently, this works specifically for Rigidbody. Apply logic to work for both Rigidbody and Rigidbody2D. This can be done in the single script, in theory, as this is an editor script. If not, enforce a reliable pattern to ensure both versions are kept up to date.


        /// <summary>The time period over which this 
        /// <see cref="Debugging.Physics.VelocityGrapher"/> is displaying velocity for.</summary>
        /// <remarks>This value is derived from <see cref="Debugging.Physics.sampleSize"/>; setting 
        /// a value to <see cref="Debugging.Physics.sampleLength"/> will lead to calculating a new 
        /// <see cref="Debugging.Physics.sampleSize"/> length, rounded down to the nearest integer 
        /// value.</remarks>   
        float sampleLength
        {
            get
            {
                // The current sampleLength is the sampleSize multiplied by the current 
                // fixedDeltaTime, as a sample will be taken at the end of each FixedUpdate method.
                return sampleSize * Time.fixedDeltaTime;
            }

            set
            {
                // The new sampleSize will be the new sampleLength divided by the fixedDeltaTime, 
                // as a sample will be taken at the end of each FixedUpdate method.
                sampleSize = (int)(value / Time.fixedDeltaTime);
            }
        }

        void Start()
        {
            // Cache the local Rigidbody.
            rigidbody = GetComponent<Rigidbody>();

            // Start the LateFixedUpdate Coroutine to allow calculations at the end of each 
            // FixedUpdate method.
            StartCoroutine(LateFixedUpdate());
        }

        /// <summary>Performs logic after waiting for the FixedUpdate method.</summary>
        /// <remarks>Any logic provided past the <c>yield return</c> statement will be performed 
        /// directly after physics calculations.</remarks>
        /// <seealso cref="UnityEngine.MonoBehaviour.LateUpdate()"/>
        IEnumerator LateFixedUpdate()
        {
            while(true)
            {
                // While this VelocityGrapher is running, wait for the next FixedUpdate, before
                // performing further logic.
                yield return new WaitForFixedUpdate();
//TODO:Perhaps we could use pointers, instead, to adapt this same grapher to other measurements. i.e. make VelocityGrapher generic?
                // Push another velocity sample to the velocitySamples array.
                PushVelocityValue(rigidbody.velocity.magnitude);
            }
        }

        /// <summary>Changes the size of the <see cref="Debugging.Physics.sampleSize"/>, pushing 
        /// changes to the size of <see cref="Debugging.Physics.velocitySamples"/>.</summary>
        /// <param name="newSampleSize">The new size for the 
        /// <see cref="Debugging.Physics.velocitySamples"/> array.</param>
        /// <remarks>To ensure data still provides accurate velocity readings after the size 
        /// change, the array will be resized with 
        /// see cref="DataStructures.ArrayManipulation.ResizeArrayWithGravity()"/>, ensuring that 
        /// only the most recent data is kept, and from the back of the array.</remarks>
        public void AdjustSampleRate(int newSampleSize)
        {
            if(newSampleSize != sampleSize)
            {
                // If we are not attempting to resize the velocitySamples array to the same size, 
                // resize the velocitySamples array to the new size, using gravity to retain data.
                DataStructures.ArrayManipulation
                    .ResizeArrayWithGravity<float>(ref velocitySamples, newSampleSize);

                // Update the sampleSize value to reflect the new velocitySamples size.
                sampleSize = newSampleSize;
            }
        }

        /// <summary>Changes the size of the <see cref="Debugging.Physics.sampleSize"/>, pushing 
        /// changes to the size of <see cref="Debugging.Physics.velocitySamples"/>.</summary>
        /// <param name="newSampleLength">The new period of time, in seconds, of which to measure 
        /// velocity samples over.</param>
        /// <remarks>To assure consistency, the time value will be converted into the resulting 
        /// number of samples, and fed into 
        /// <see cref="Debugging.Physics.VelocityGrapher.AdjustSampleRate(int)"/>. Derived 
        /// number used as the sampleSize will be rounded down to the nearest integer.</remarks>
        public void AdjustSampleRate(float newSampleLength)
        {
            // A sample will be taken at the end of each FixedUpdate(); derive the appropriate 
            // sample size from the newSampleLength, and feed it into the alternate 
            // AdjustSampleRate method that takes a direct sample size.
            AdjustSampleRate((int)(newSampleLength / Time.fixedDeltaTime));
        }

        //TODO:VelocityGrapher could be generic, to measure a greater variety of values. Ensure, when complete, the below summary is updated.
        /// <summary>Pushes a new value to the 
        /// <see cref="Debugging.Physics.VelocityGrapher.velocitySamples"/> array.</summary>
        /// <param name="velocityMagnitude">The new value to log, in the 
        /// <see cref="Debugging.Physics.VelocityGrapher"/>. Ideally, this will be the current 
        /// <see cref="Debugging.Physics.VelocityGrapher.rigidbody.velocity.magnitude"/>, 
        /// though in practicallity, any value set can be passed in for measurement.</param>
        /// <remarks>During this update, data in 
        /// <see cref="Debugging.Physics.VelocityGrapher.velocitySamples"/> will be shuffled 
        /// forward, in order to retain the most recent data.</remarks>
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
        private Material material;

        void OnEnable()
        {
            AdjustInternalMeasurements();
            material = new Material(Shader.Find("Hidden/Internal-Colored"));
        }
        
        private void OnDisable()
        {
            DestroyImmediate(material);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            /*VelocityGrapher velocityGrapher = target as VelocityGrapher;

            if(GUILayout.Button(DebuggingPhysicsLabels.testArrays))
            {
                DataStructures.ArrayManipulation.TestGravityArrayAdjustment();
            }

            if(GUILayout.Button("TestOutput"))
            {
                Debug.Log(Screen.width); // < definately pings back the inspector width
            }

            GUILayout.BeginHorizontal(EditorStyles.helpBox);*/
            
            Rect rect = GUILayoutUtility.GetRect(10,10000,200,200);
            if(Event.current.type == EventType.Repaint)
            {
                Debug.Log("Drawing");
                GUI.BeginClip(rect);
                GL.Viewport(rect);
                GL.LoadPixelMatrix(0, rect.width, rect.height, 0);
                GL.Clear(true, true, Color.red);
                material.SetPass(0);

                GL.Begin(GL.LINES);
                GL.Color(Color.black);
                GL.Vertex3(200, 0, 0);
                GL.Vertex3(200, 10000, 0);
                GL.Vertex3(0, 10000, 0);
                GL.Vertex3(0, 0, 0);
                GL.End();
                GUI.EndClip();
            }
            
            //GUILayout.EndHorizontal();
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

    public static class VelocityGrapherTooltips
    {
        public const string sampleSize = "The number of samples to use when graphing the current "
            + "velocity.";
        public const string maximumVelocity = "The maximum velocity value to graph.";
        public const string minimumVelocity = "The minimum velocity value to graph.";
        public const string velocitySamples = "The array of current velocity samples, as recorded.";
        public const string rigidbody = "The local rigidbody.";
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