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
    using Tooltips = Utility.ElectricalTooltips;
    using Colours = Utility.ElectricalColours;
    using Dimensions = Utility.ElectricalDimensions;
    #endif

    public abstract class ElectricalComponent : MonoBehaviour
    {
        [SerializeField] protected ElectricalComponent inputNode;
        [SerializeField] protected ElectricalComponent outputNode;
        [SerializeField] protected int id;

        private static int electricalComponentCount = 0;

        public int totalCount { get { return electricalComponentCount; } }
        public int componentID { get { return id; } }

        public virtual void ApplyPower(float voltage, float current, bool usingFixedVoltage)
        {
            if(outputNode != null)
            {
                outputNode.ApplyPower(voltage, current, usingFixedVoltage);
            }
        }

        protected virtual void Start()
        {
            id = electricalComponentCount;
            electricalComponentCount++;
        }

        public void DisconnectInputNode()
        {
            inputNode = null;
        }

        public void DisconnectOutputNode()
        {
            outputNode = null;
        }

        public void ConnectInputNode(ElectricalComponent inputNode)
        {
            this.inputNode = inputNode;
        }

        public void ConnectOutputNode(ElectricalComponent outputNode)
        {
            this.outputNode = outputNode;
        }

        public virtual bool TestLiveCircuit()
        {
            if(outputNode == null)
            {
                return false;
            }
            else
            {
                return outputNode.TestLiveCircuit(id);
            }
        }

        protected virtual bool TestLiveCircuit(int sourceID)
        {
            if(id == sourceID)
            {
                return true;
            }
            else if(outputNode == null)
            {
                return false;
            }
            else
            {
                return outputNode.TestLiveCircuit(sourceID);
            }
        }
    }

    public static class Electrical
    {
        #region Ohm's Law
        public static float CalculateVoltage(float current, float resistance)
        {
            return current * resistance;
        }

        public static float CalculateCurrent(float voltage, float resistance)
        {
            return voltage / resistance;
        }

        public static float CalculateResistance(float current, float voltage)
        {
            return voltage / current;
        }
        #endregion

        #region Total Resistance
        public static float CalculateTotalParallelResistance(params float[] resistances)
        {
            float totalInvertedResistance = 0f;

            for(int i = 0; i < resistances.Length; i++)
            {
                totalInvertedResistance += (1.0f / resistances[i]);
            }

            return (1.0f / totalInvertedResistance);
        }

        public static float CalculateTotalResistance(float[][] resistances)
        {
            int circuitCount = resistances.GetLength(0);
            float[] totalSeriesResistances = new float[circuitCount];

            for(int i = 0; i < circuitCount; i++)
            {
                totalSeriesResistances[i] = CalculateTotalSeriesResistance(resistances[i]);
            }

            return CalculateTotalParallelResistance(totalSeriesResistances);
        }

        public static float CalculateTotalSeriesResistance(params float[] resistances)
        {
            float totalResistance = 0f;

            for(int i = 0; i < resistances.Length; i++)
            {
                totalResistance += resistances[i];
            }

            return totalResistance;
        }
        #endregion
    }
}

namespace Electrical.Utility
{
    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class ElectricalTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class ElectricalColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class ElectricalDimensions
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