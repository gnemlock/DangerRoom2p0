/* Created by Matthew Francis Keating */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Electrical
{
    using StringFormats = Utility.ElectricalStringFormats;
    using Labels = Utility.ElectricalLabels;
    using Log = Utility.ElectricalDebug;
    using Tags = Utility.ElectricalTags;

    #if UNITY_EDITOR
    using Tooltips = Utility.ElectricalWireTooltips;
    using Colours = Utility.ElectricalWireColours;
    using Dimensions = Utility.ElectricalWireDimensions;
    #endif

    public class ElectricalWire : ElectricalComponent 
    {
        [SerializeField] private Vector2[] positions;

        public Vector2 start { get { return positions[0]; } }
        public Vector2 end { get { return positions[segmentCount]; } }
        public int segmentCount { get { return positions.Length - 1; } }

        public float TotalLength()
        {
            float totalDistance = 0f;

            for(int i = 1; i < positions.Length; i++)
            {
                totalDistance += Vector2.Distance(positions[i - 1], positions[i]);
            }

            return totalDistance;
        }

        public float SegmentLength(int segmentID)
        {
            if(segmentID < 1 || segmentID > segmentCount)
            {
                return 0f;
            }

            return Vector2.Distance(positions[segmentID - 1], positions[segmentID]);
        }

        public float SegmentLength(int startSegmentID, int endSegmentID)
        {
            if(startSegmentID < 1)
            {
                startSegmentID = 1;
            }

            if(endSegmentID > segmentCount)
            {
                endSegmentID = segmentCount;
            }

            if(startSegmentID == endSegmentID)
            {
                return 0f;
            }

            if(startSegmentID > endSegmentID)
            {
                int newStartSegmentID = endSegmentID;
                endSegmentID = startSegmentID;
                startSegmentID = newStartSegmentID;
            }

            float totalDistance = 0f;

            for(int i = startSegmentID; i <= endSegmentID; i++)
            {
                totalDistance += Vector2.Distance(positions[i - 1], positions[i]);
            }

            return totalDistance;
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(ElectricalWire))] public class ElectricalWireEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ElectricalWire electricalWire = target as ElectricalWire;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class ElectricalWireTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class ElectricalWireColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class ElectricalWireDimensions
    {
    }
    #endif

    // Strings containing format for string interface display.
    public static partial class ElectricalStringFormats
    {
    }
    
    // Strings used for general labelling.
    public static partial class ElectricalLabels
    {
    }
    
    // Provides debug functionality, including methods and customised string messages.
    public static partial class ElectricalDebug
    {
    }
    
    // Strings used for tag or name comparison
    public static partial class ElectricalTags
    {
    }
}