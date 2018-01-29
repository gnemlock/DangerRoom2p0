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
        /// <summary>The resistance value of this <see cref="Electrical.Resistor"/>.</summary>
        [SerializeField][Tooltip(Tooltips.resistance)] protected float resistance;

        /// <summary>The current voltage value flowing across this 
        /// <see cref="Electrical.Resistor"/>.</summary>
        public float voltage { get; protected set; }
        /// <summary>The current current value flowing across this 
        /// <see cref="Electrical.Resistor"/>.</summary>
        public float current { get; protected set; }

        /// <summary>Delegate method for whenever there is a change in the power being drawn by 
        /// this <see cref="Electrical.Resistor"/>.</summary>
        public ComponentUpdate ChangeInDrawnPower;

        /// <summary>Applys power to this <see cref="Electrical.Resistor">.</summary>
        /// <param name="voltage">The value of the voltage being applied to this 
        /// <see cref="Electrical.ElectricalComponent">, in volts.</param>
        /// <param name="current">The value of the current being applied to this 
        /// <see cref="Electrical.Resistor">, in amps.</param>
        /// <param name="usingFixedVoltage">If <c>true</c>, this 
        /// <see cref="Electrical.Resistor"> will assume a fixed voltage, and apply 
        /// manipulation to the current. If <c>false</c>, this 
        /// <see cref="Electrical.Resistor"> will assume a fixed current, and apply 
        /// manipulation to the voltage.</params>
        /// <remarks>A <see cref="Electrical.Resistor">, on applied power, will manipulate the 
        /// voltage or current of the incoming power, before forwarding it on to the next 
        /// <see cref="Electrical.ElectricalComponent"> type.</remarks>
        public override void ApplyPower(float voltage, float current, bool usingFixedVoltage)
        {
            if(outputNode != null)
            {
                bool detectChange = false;

                // If there is a valid node to output power to,
                if(usingFixedVoltage)
                {
                    // If we are using a fixed voltage, use OHMs law to determine the derived 
                    // current.
                    float newCurrent = Electrical.CalculateCurrent(voltage, resistance);

                    if(current != newCurrent)
                    {
                        // If the new current differs from the current current, update the current 
                        // current, and flag for a detected change.
                        current = newCurrent;
                        detectChange = true;
                    }
                }
                else
                {
                    // Else, we are using a fixed current; use OHMs law to determine the derived 
                    // voltage.
                    float newVoltage = Electrical.CalculateVoltage(current, resistance);

                    if(voltage != newVoltage)
                    {
                        // If the new voltage differs from the current voltage, update the current 
                        // voltage, and flag for a detected change.
                        voltage = newVoltage;
                        detectChange = true;
                    }
                }

                if(detectChange)
                {
                    // If there is a detected change, apply the power change to the connected node.
                    outputNode.ApplyPower(voltage, current, usingFixedVoltage);

                    if(ChangeInDrawnPower != null)
                    {
                        // If there are connected listeners to changes in drawn power, 
                        // run the connected methods.
                        ChangeInDrawnPower();
                    }
                }
            }
        }

        /// <summary>Returns the current <see cref="Electrical.Resistor.resistance"/> 
        /// value.</summary>
        /// <returns>The current resistance value.</returns>
        public float GetResistance()
        {
            // Return the current resistance value.
            return resistance;
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(Resistor))] public class ResistorEditor : Editor
    {
        private Resistor resistor;
        private Transform transform;
        private Quaternion handleRotation;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Resistor resistor = target as Resistor;
        }

        //TODO:Draw in input and output nodes; implement wire creation on click
        private void OnSceneGUI()
        {
            resistor = target as Resistor;
            transform = resistor.transform;

            float resistorWidth = ResistorDimensions.resistorWidth;
            float resistorHeight = ResistorDimensions.resistorHeight;
            float connectionLength = ResistorDimensions.connectionLength;
            float symbolMargin = ResistorDimensions.symbolMargin;
            float symbolLength = ResistorDimensions.symbolLength;

            Vector3 outputPosition = transform.position + (Vector3.right * (resistorWidth/2.0f));
            Vector3 inputPosition = transform.position + (Vector3.left * (resistorWidth/2.0f));

            Handles.color = ResistorColours.resistorColour;

          /*  Handles.DrawSolidRectangleWithOutline(new Rect(0, transform.position, resistorWidth, 
                resistorHeight), ResistorColours.resistorColour);
              // EventType.Repaint);*/

            Handles.DrawLine(outputPosition, outputPosition + (Vector3.right * connectionLength));
            Handles.DrawLine(inputPosition, inputPosition + (Vector3.left * connectionLength));

            Handles.color = ElectricalColours.positiveSymbol;

            Vector3 positiveHorizontalStart = outputPosition - (Vector3.right * symbolMargin);
            Vector3 positiveHorizontalEnd 
            = positiveHorizontalStart - (Vector3.right * symbolLength);
            Vector3 positiveVerticalStart = positiveHorizontalStart
                - (Vector3.right * symbolLength * 0.5f) + (Vector3.up * symbolLength * 0.5f);
            Vector3 positiveVerticalEnd = positiveVerticalStart + (Vector3.down * symbolLength);

            Handles.DrawLine(positiveHorizontalStart, positiveHorizontalEnd);
            Handles.DrawLine(positiveVerticalStart, positiveVerticalEnd);

            Handles.color = ElectricalColours.negativeSymbol;

            Vector3 negativeHorizontalStart = inputPosition - (Vector3.left * symbolMargin);
            Vector3 negativeHorizontalEnd 
            = negativeHorizontalStart + (Vector3.right * symbolLength);

            Handles.DrawLine(negativeHorizontalStart, negativeHorizontalEnd);
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class ResistorTooltips
    {
        public const string resistance = "The resistance value of this Resistor. V = RC.";
    }

    // Colours for use in displaying custom editor GUI.
    public static class ResistorColours
    {
        public static Color resistorColour = Color.green;
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class ResistorDimensions
    {
        public const float resistorWidth = 10.0f;
        public const float resistorHeight = 3.0f;
        public const float connectionLength = 1.0f;
        public const float symbolMargin = 1.0f;
        public const float symbolLength = 1.0f;
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