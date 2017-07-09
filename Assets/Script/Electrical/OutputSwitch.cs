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
    using Tooltips = Utility.OutputSwitchTooltips;
    using Colours = Utility.OutputSwitchColours;
    using Dimensions = Utility.OutputSwitchDimensions;
    #endif

    public class OutputSwitch : ElectricalComponent
    {
        [SerializeField] protected ElectricalComponent[] outputNodes;
        [SerializeField] protected int connectionID = 0;
        [SerializeField] protected bool canBreakCircuit = false;

        [SerializeField] new protected ElectricalComponent outputNode 
        {
            get 
            {
                if(canBreakCircuit && (connectionID < 0 || connectionID >= outputNodes.Length))
                {
                    return null;
                }
                else
                {
                    return outputNodes[connectionID];
                }
            }
        }

        public int Switch()
        {
            return Switch(connectionID + 1);
        }

        public int Switch(int newConnectionID)
        {
            if(outputNode != null)
            {
                outputNode.DisconnectInputNode();
            }

            connectionID = newConnectionID;

            if(!canBreakCircuit && newConnectionID >= outputNodes.Length)
            {
                connectionID = 0;
            }

            if(outputNode != null)
            {
                outputNode.ConnectInputNode((ElectricalComponent)this);
            }

            return connectionID;
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(OutputSwitch))] public class OutputSwitchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            OutputSwitch outputSwitch = target as OutputSwitch;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class OutputSwitchTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class OutputSwitchColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class OutputSwitchDimensions
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