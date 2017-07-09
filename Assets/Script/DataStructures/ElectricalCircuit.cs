/* Created by Matthew Francis Keating */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DataStructures
{
    using StringFormats = Utility.DataStructuresStringFormats;
    using Labels = Utility.DataStructuresLabels;
    using Log = Utility.DataStructuresDebug;
    using Tags = Utility.DataStructuresTags;

    #if UNITY_EDITOR
    using Tooltips = Utility.ElectricalCircuitTooltips;
    using Colours = Utility.ElectricalCircuitColours;
    using Dimensions = Utility.ElectricalCircuitDimensions;
    #endif

    public class ElectricalCircuit : MonoBehaviour 
    {
        /// <summary>This method will be called at the start of each frame where this instance 
        /// of <see cref="DataStructures.ElectricalCircuit"/> is enabled.</summary>
        void Update()
        {
            
        }
    }
}

namespace DataStructures.Utility
{
    [CustomEditor(typeof(ElectricalCircuit))] public class ElectricalCircuitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ElectricalCircuit electricalCircuit = target as ElectricalCircuit;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class ElectricalCircuitTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class ElectricalCircuitColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class ElectricalCircuitDimensions
    {
    }
    #endif

    // Strings containing format for string interface display.
    public static partial class DataStructuresStringFormats
    {
    }
    
    // Strings used for general labelling.
    public static partial class DataStructuresLabels
    {
    }
    
    // Provides debug functionality, including methods and customised string messages.
    public static partial class DataStructuresDebug
    {
    }
    
    // Strings used for tag or name comparison
    public static partial class DataStructuresTags
    {
    }
}