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
    using Tooltips = Utility.VoltageSourceTooltips;
    using Colours = Utility.VoltageSourceColours;
    using Dimensions = Utility.VoltageSourceDimensions;
    #endif

    public class VoltageSource : Source 
    {
        [SerializeField] protected float voltage;
        [SerializeField] protected float current;

        /// <summary>This method will be called at the start of each frame where this instance 
        /// of <see cref="Electrical.VoltageSource"/> is enabled.</summary>
        void Update ()
        {
            
        }

        public override void ApplyPower(float voltage, float current, bool usingFixedVoltage)
        {
            //TODO:Determine best reaction
        }

        public virtual void ApplyPower()
        {
            ApplyPower(voltage, current, true);
        }

        public float GetVoltage()
        {
            return voltage;
        }

        public float GetCurrent()
        {
            return current;
        }

        public float GetPower()
        {
            return Electrical.CalculatePower(voltage, current);
        }

        public void SetCurrent(float current)
        {
            this.current = current;
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(VoltageSource))] public class VoltageSourceEditor : Editor
    {
        private VoltageSource voltageSource;
        private Transform transform;
        private Quaternion handleRotation;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            VoltageSource voltageSource = target as VoltageSource;
        }

        //TODO:Draw in input and output nodes; implement wire creation on click
        private void OnSceneGUI()
        {
            voltageSource = target as VoltageSource;
            transform = voltageSource.transform;

            float sourceRadius = VoltageSourceDimensions.sourceRadius;
            float connectionLength = VoltageSourceDimensions.connectionLength;
            float symbolMargin = VoltageSourceDimensions.symbolMargin;
            float symbolLength = VoltageSourceDimensions.symbolLength;
            Vector3 outputPosition = transform.position + (Vector3.right * sourceRadius);
            Vector3 inputPosition = transform.position + (Vector3.left * sourceRadius);

            Handles.color = VoltageSourceColours.sourceColour;

            Handles.CircleHandleCap(0, transform.position, transform.rotation, sourceRadius, 
                EventType.Repaint);
            
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
    public static class VoltageSourceTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class VoltageSourceColours
    {
        public static Color sourceColour = Color.yellow;
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class VoltageSourceDimensions
    {
        public const float sourceRadius = 5.0f;
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