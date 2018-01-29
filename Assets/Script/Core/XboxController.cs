/* Created by Matthew Francis Keating */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    using StringFormats = Utility.CoreStringFormats;
    using Labels = Utility.CoreLabels;
    using Log = Utility.CoreDebug;
    using Tags = Utility.CoreTags;

    #if UNITY_EDITOR
    using Tooltips = Utility.XboxControllerTooltips;
    using Colours = Utility.XboxControllerColours;
    using Dimensions = Utility.XboxControllerDimensions;
    #endif

    public class XboxController : MonoBehaviour 
    {
        /// <summary>This method will be called at the start of each frame where this instance 
        /// of <see cref="Core.XboxController"/> is enabled.</summary>
        void Update ()
        {
            
        }
    }
}

namespace Core.Utility
{
    [CustomEditor(typeof(XboxController))] public class XboxControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            XboxController xboxController = target as XboxController;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class XboxControllerTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class XboxControllerColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class XboxControllerDimensions
    {
    }
    #endif

    // Strings containing format for string interface display.
    public static partial class CoreStringFormats
    {
    }
    
    // Strings used for general labelling.
    public static partial class CoreLabels
    {
    }
    
    // Provides debug functionality, including methods and customised string messages.
    public static partial class CoreDebug
    {
    }
    
    // Strings used for tag or name comparison
    public static partial class CoreTags
    {
    }
}