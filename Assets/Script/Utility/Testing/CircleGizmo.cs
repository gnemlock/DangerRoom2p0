using UnityEngine;

namespace Utility.Testing
{
    #if UNITY_EDITOR
    using Colours = Testing.Utility.TestingColours;
    using Dimensions = Testing.Utility.TestingDimensions;
    #endif

    public class CircleGizmo : MonoBehaviour
    {
        public int resolution = 10;

        #if UNITY_EDITOR
        /// <summary>This method is called when the editor draws the gizmos for an instance of 
        /// <see cref="Utility.Testing.CircleGizmo"/> that is selected in the Inspector. THIS METHOD 
        /// IS EDITOR ONLY.</summary>
        private void OnDrawGizmosSelected()
        {
            float step = 2.0f / (float)resolution;

            for(int i = 0; i <= resolution; i++)
            {
                ShowPoint(i * step - 1.0f, -1.0f);
                ShowPoint(i * step - 1.0f, 1.0f);
            }

            for(int i = 0; i <= resolution; i++)
            {
                ShowPoint(-1.0f, i * step - 1.0f);
                ShowPoint(1.0f, i * step - 1.0f);
            }
        }
        #endif

        private void ShowPoint(float x, float y)
        {
            Vector2 squareVertex = new Vector2(x, y);
            Vector2 circleVertex = new Vector2(
                squareVertex.x * Mathf.Sqrt(1.0f - squareVertex.y * squareVertex.y * 0.5f), 
                squareVertex.y * Mathf.Sqrt(1.0f - squareVertex.x * squareVertex.x * 0.5f));

            Gizmos.color = Colours.squareVertexMarkerColour;
            Gizmos.DrawSphere(squareVertex, Dimensions.vertexMarkerRadius);

            Gizmos.color = Colours.circleVertexMarkerColour;
            Gizmos.DrawSphere(circleVertex, Dimensions.vertexMarkerRadius);

            Gizmos.color = Colours.outerLineColour;
            Gizmos.DrawLine(squareVertex, circleVertex);

            Gizmos.color = Colours.innerLineColour;
            Gizmos.DrawLine(circleVertex, Vector2.zero);
        }
    }
}

namespace Utility.Testing.Utility
{
    public static partial class TestingColours
    {
        #if UNITY_EDITOR
        public static Color squareVertexMarkerColour = Color.black;
        public static Color circleVertexMarkerColour = Color.white;
        public static Color outerLineColour = Color.yellow;
        public static Color innerLineColour = Color.grey;
        #endif
    }

    public static partial class TestingDimensions
    {
        #if UNITY_EDITOR
        public const float vertexMarkerRadius = 0.025f;
        #endif
    }
}