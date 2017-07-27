/* Created by Matthew Francis Keating */

using UnityEngine;
using Electrical;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Debugging.Electrical
{
    using StringFormats = Utility.DebuggingElectricalStringFormats;
    using Labels = Utility.DebuggingElectricalLabels;
    using Log = Utility.DebuggingElectricalDebug;
    using Tags = Utility.DebuggingElectricalTags;

    #if UNITY_EDITOR
    using Tooltips = Utility.WireFollowerTooltips;
    using Colours = Utility.WireFollowerColours;
    using Dimensions = Utility.WireFollowerDimensions;
    #endif

    public class WireFollower : MonoBehaviour 
    {
        public ElectricalWire electricalWire;
        private bool following;
        public float speed;
        public float position = 0f;

        /// <summary>This method will be called at the start of each frame where this instance 
        /// of <see cref="Debugging.Electrical.WireFollower"/> is enabled.</summary>
        void Update ()
        {
            if(following)
            {
                position = (position + (speed * Time.deltaTime)) % 1.0f;
                Move();
            }
        }

        public void Follow()
        {
            following = !following;
        }

        public void Move()
        {
            transform.position = electricalWire.GetNormalisedPosition(position);
        }
    }
}

namespace Debugging.Electrical.Utility
{
    [CustomEditor(typeof(WireFollower))] public class WireFollowerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WireFollower wireFollower = target as WireFollower;

            if(GUILayout.Button("Follow"))
            {
                wireFollower.Follow();
            }

            if(GUILayout.Button("Move"))
            {
                wireFollower.Move();
            }
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class WireFollowerTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class WireFollowerColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class WireFollowerDimensions
    {
    }
    #endif

    // Strings containing format for string interface display.
    public static partial class DebuggingElectricalStringFormats
    {
    }
    
    // Strings used for general labelling.
    public static partial class DebuggingElectricalLabels
    {
    }
    
    // Provides debug functionality, including methods and customised string messages.
    public static partial class DebuggingElectricalDebug
    {
    }
    
    // Strings used for tag or name comparison
    public static partial class DebuggingElectricalTags
    {
    }
}