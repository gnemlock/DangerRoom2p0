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
    using Tooltips = Utility.InputSwitchTooltips;
    using Colours = Utility.InputSwitchColours;
    using Dimensions = Utility.InputSwitchDimensions;
    #endif

    public class InputSwitch : ElectricalComponent 
    {
        [SerializeField] protected ElectricalComponent[] inputNodes;
        [SerializeField] protected int connectionID = 0;
        [SerializeField] protected bool canBreakCircuit = false;

        [SerializeField] new protected ElectricalComponent inputNode 
        {
            get 
            {
                if(canBreakCircuit && (connectionID < 0 || connectionID >= inputNodes.Length))
                {
                    return null;
                }
                else
                {
                    return inputNodes[connectionID];
                }
            }
        }

        public int Switch()
        {
            return Switch(connectionID + 1);
        }

        public int Switch(int newConnectionID)
        {
            if(inputNode != null)
            {
                inputNode.DisconnectOutputNode();
            }

            connectionID = newConnectionID;

            if(!canBreakCircuit && newConnectionID >= inputNodes.Length)
            {
                connectionID = 0;
            }

            if(inputNode != null)
            {
                inputNode.ConnectOutputNode((ElectricalComponent)this);
            }

            return connectionID;
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(InputSwitch))] public class InputSwitchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InputSwitch inputSwitch = target as InputSwitch;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class InputSwitchTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class InputSwitchColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class InputSwitchDimensions
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