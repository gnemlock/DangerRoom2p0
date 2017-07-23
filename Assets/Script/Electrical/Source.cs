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
    using Tooltips = Utility.SourceTooltips;
    using Colours = Utility.SourceColours;
    using Dimensions = Utility.SourceDimensions;
    #endif

    //TODO:Create Source class for managing all source drawing functions; determine additional automatable functions
    public abstract class Source : ElectricalComponent 
    {
        /// <summary>This method will be called at the start of each frame where this instance 
        /// of <see cref="Electrical.Source"/> is enabled.</summary>
        void Update ()
        {
            
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(Source))] public class SourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Source source = target as Source;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class SourceTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class SourceColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class SourceDimensions
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