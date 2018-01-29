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
    using Tooltips = Utility.CurrentSourceTooltips;
    using Colours = Utility.CurrentSourceColours;
    using Dimensions = Utility.CurrentSourceDimensions;
    #endif

    public class CurrentSource : MonoBehaviour 
    {
        /// <summary>This method will be called at the start of each frame where this instance 
        /// of <see cref="Electrical.CurrentSource"/> is enabled.</summary>
        void Update ()
        {
            
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(CurrentSource))] public class CurrentSourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CurrentSource currentSource = target as CurrentSource;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class CurrentSourceTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class CurrentSourceColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class CurrentSourceDimensions
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