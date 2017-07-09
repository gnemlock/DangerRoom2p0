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
    using Tooltips = Utility.ResistorTooltips;
    using Colours = Utility.ResistorColours;
    using Dimensions = Utility.ResistorDimensions;
    #endif

    public class Resistor : ElectricalComponent 
    {
        [SerializeField] private float resistance;

        public override void ApplyPower(float voltage, float current, bool usingFixedVoltage)
        {
            if(outputNode != null)
            {
                if(usingFixedVoltage)
                {
                    current = Electrical.CalculateCurrent(voltage, resistance);
                }
                else
                {
                    voltage = Electrical.CalculateVoltage(current, resistance);
                }

                outputNode.ApplyPower(voltage, current, usingFixedVoltage);
            }
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(Resistor))] public class ResistorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Resistor resistor = target as Resistor;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class ResistorTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class ResistorColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class ResistorDimensions
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