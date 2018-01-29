/* Created by Matthew Francis Keating */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CardAI
{
    using StringFormats = Utility.CardAIStringFormats;
    using Labels = Utility.CardAILabels;
    using Log = Utility.CardAIDebug;
    using Tags = Utility.CardAITags;

    #if UNITY_EDITOR
    using Tooltips = Utility.PyramidSolverTooltips;
    using Colours = Utility.PyramidSolverColours;
    using Dimensions = Utility.PyramidSolverDimensions;
    #endif

    public class PyramidSolver : MonoBehaviour 
    {
        /// <summary>This method will be called at the start of each frame where this instance 
        /// of <see cref="CardAI.PyramidSolver"/> is enabled.</summary>
        void Start ()
        {
        }
    }
}

namespace CardAI.Utility
{
    [CustomEditor(typeof(PyramidSolver))] public class PyramidSolverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PyramidSolver pyramidSolver = target as PyramidSolver;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class PyramidSolverTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class PyramidSolverColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class PyramidSolverDimensions
    {
    }
    #endif

    // Strings containing format for string interface display.
    public static partial class CardAIStringFormats
    {
    }
    
    // Strings used for general labelling.
    public static partial class CardAILabels
    {
    }
    
    // Provides debug functionality, including methods and customised string messages.
    public static partial class CardAIDebug
    {
    }
    
    // Strings used for tag or name comparison
    public static partial class CardAITags
    {
    }
}