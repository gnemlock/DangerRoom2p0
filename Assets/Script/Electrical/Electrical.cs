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

    /// <summary>Represents the base functionality of all electrical components.</summary>
    public abstract class ElectricalComponent : MonoBehaviour
    {
        /// <summary>The <see cref="Electrical.ElectricalComponent"> that connects to supply power to this 
        /// <see cref="Electrical.ElectricalComponent">.</summary>
        /// <remarks>All <see cref="Electrical.ElectricalComponent"> types must be able to receive power from another 
        /// <see cref="Electrical.ElectricalComponent">. However, some types might have the option of multiple 
        /// <see cref="Electrical.ElectricalComponent"> inputs, and thus, might treat this field differently.</remarks>
        [SerializeField][Tooltip(Tooltips.inputNode)] protected ElectricalComponent inputNode;
        /// <summary>The <see cref="Electrical.ElectricalComponent"> that connects to receive power from this 
        /// <see cref="Electrical.ElectricalComponent">.</summary>
        /// <remarks>All <see cref="Electrical.ElectricalComponent"> types must be able to provide power to another 
        /// <see cref="Electrical.ElectricalComponent">. However, some types might have the option of multiple 
        /// <see cref="Electrical.ElectricalComponent"> outputs, and thus, might treat this field differently.</remarks>
        [SerializeField][Tooltip(Tooltips.outputNode)] protected ElectricalComponent outputNode;
        /// <summary>The current ID number used to uniquely identify this <see cref="Electrical.ElectricalComponent">.</summary>
        /// <remarks>The ID is allocated dynamically, at run time; it should never be directly changed, and is only used for 
        /// internal <see cref="Electrical.ElectricalComponent"> comparison. It is serialized for the purpose of easy editor 
        /// observation.</remarks>
        [SerializeField][Tooltip(Tooltips.id)] protected int id;
        // TODO: Set id to draw as read only.

        /// <summary>Internal count for current <see cref="Electrical.ElectricalComponent" types that have been 
        /// instantiated.</summary>
        /// <remarks>This value is mostly used to determine the next available number for 
        /// <see cref="Electrical.ElectricalComponent.id">.</remarks>
        private static int electricalComponentCount = 0;

        /// <summary>Retrieves the total count of instantiated <see cref="Electrical.ElectricalComponent"> types.</summary>
        /// <remarks>This property is derived from a value used to ensure unique identification numbers; as such, it should not be 
        /// used as a reliable count of currently active <see cref="Electrical.ElectricalComponent"> types.</remarks>
        public int totalCount { get { return electricalComponentCount; } }
        /// <summary>Retrieves the current <see cref="Electrical.ElectricalComponent.id"> being used for this 
        /// <see cref="Electrical.ElectricalComponent">.</summary>
        public int componentID { get { return id; } }

        /// <summary>Applys power to this <see cref="Electrical.ElectricalComponent">.</summary>
        /// <param name="voltage">The value of the voltage being applied to this <see cref="Electrical.ElectricalComponent">, 
        /// in volts.</param>
        /// <param name="current">The value of the current being applied to this <see cref="Electrical.ElectricalComponent">, 
        /// in amps.</param>
        /// <param name="usingFixedVoltage">If <c>true</c>, this <see cref="Electrical.ElectricalComponent"> will assume a 
        /// fixed voltage, and apply manipulation to the current. If <c>false</c>, this 
        /// <see cref="Electrical.ElectricalComponent"></param> will assume a fixed current, and apply manipulation to the 
        /// voltage.</params>
        /// <remarks>The default behaviour, on applied power, is to simply allow the power to move directly into the 
        /// <see cref="Electrical.ElectricalComponent.outputNode">; however, this behaviour should be overridden for 
        /// more advanced <see cref="Electrical.ElectricalComponent"> types.</remarks>
        public virtual void ApplyPower(float voltage, float current, bool usingFixedVoltage)
        {
            if(outputNode != null)
            {
                // If the outputNode is not null, and thus connected; forward the power to the outputNode.
                outputNode.ApplyPower(voltage, current, usingFixedVoltage);
            }
        }

        /// <summary>This method will be called just before the first Update call.</summary>
        protected virtual void Start()
        {
            // Set the id for this ElectricalComponent to the next available id value, and increment 
            /// electricalComponentCount to reflect an available id value for the next ElectricalComponent.
            id = electricalComponentCount;
            electricalComponentCount++;
        }

        /// <summary>Disconnects the <see cref="Electrical.ElectricalComponent.inputNode> from this 
        /// <see cref="Electrical.ElectricalComponent">, nullifying the reference.</summary>
        public virtual void DisconnectInputNode()
        {
            // Disconnect the inputNode, by nullifying the reference.
            inputNode = null;
        }

        /// <summary>Disconnects the <see cref="Electrical.ElectricalComponent.outputNode> from this 
        /// <see cref="Electrical.ElectricalComponent">, nullifying the reference.</summary>
        public virtual void DisconnectOutputNode()
        {
            // Disconnect the outputNode, by nullifying the reference.
            outputNode = null;
        }

        /// <summary>Connects a new <see cref="Electrical.ElectricalComponent.inputNode> to this 
        ///  <see cref="Electrical.ElectricalComponent">.</summary>
        /// <params name="inputNode">The <see cref="Electrical.ElectricalComponent"> to use as the new 
        /// input node.</params>
        public virtual void ConnectInputNode(ElectricalComponent inputNode)
        {
            // Connect the new inputNode via reference.
            this.inputNode = inputNode;
        }

        /// <summary>Connects a new <see cref="Electrical.ElectricalComponent.outputNode> to this 
        ///  <see cref="Electrical.ElectricalComponent">.</summary>
        /// <params name="outputNode">The <see cref="Electrical.ElectricalComponent"> to use as the new 
        /// output node.</params>
        public virtual void ConnectOutputNode(ElectricalComponent outputNode)
        {
            // Connect the new outputNode via reference.
            this.outputNode = outputNode;
        }

        /// <summary>Tests this <see cref="Electrical.ElectricalComponent">, to see if it is connected to a 
        /// live circuit.</summary>
        /// <returns><c>True</c>, if this <see cref="Electrical.ElectricalComponent"> is connected to a live 
        /// circuit, else returns <c>false</c>.
        /// <remarks>This test will start reference hopping through the circuit via way of the 
        /// <see cref="Electrical.ElectricalComponent.outputNode"> reference. If this method loop only reaches null references, 
        /// this <see cref="Electrical.ElectricalComponent"> is not in a live circuit; if the method reaches the same 
        /// <see cref="Electrical.ElectricalComponent">, identified by <see cref="Electrical.ElectricalComponent.id">, we know 
        /// that this <see cref="Electrical.ElectricalComponent"> is in a live circuit.</remarks>
        public virtual bool TestLiveCircuit()
        {
            if(outputNode == null)
            {
                // If the outputNode reference is null, we are not connected to a live circuit; return false.
                return false;
            }
            else
            {
                // Else, we have a valid outputNode reference; start looping the TestLiveCircuit method, 
                // comparing against the local id.
                return outputNode.TestLiveCircuit(id);
            }
        }

        /// <summary>Tests this <see cref="Electrical.ElectricalComponent">, to see if it is connected to a 
        /// live circuit in relation to a prior <see cref="Electrical.ElectricalComponent.inputNode">.</summary>
        /// <params name="sourceID">The <see creft=Electrical.ElectricalComponent.id"> value to which we are comparing 
        /// this <see cref="Electrical.ElectricalComponent"> against, in order to determine if we have confirmed a 
        /// complete circuit.</params>
        /// <returns><c>True</c>, if the referenced <see cref="Electrical.ElectricalComponent"> is connected to a live 
        /// circuit, else returns <c>false</c>.
        /// <remarks>This test continues reference hopping through the circuit via way of the 
        /// <see cref="Electrical.ElectricalComponent.outputNode"> reference. If this method loop only reaches null references, 
        /// this <see cref="Electrical.ElectricalComponent"> is not in a live circuit; if the method reaches the same 
        /// <see cref="Electrical.ElectricalComponent">, identified by the sourceID, we know 
        /// that we have confirmed a live circuit.</remarks>
        protected virtual bool TestLiveCircuit(int sourceID)
        {
            if(id == sourceID)
            {
                // If the local ID is the same as the source ID, we have arrived back at the original ElectricalComponent;
                // confirming a live circuit. Return true.
                return true;
            }
            else if(outputNode == null)
            {
                // Else, if we have not arrived at the original ElectricalComponent, and the outputNode reference is null, 
                // we are have confirmed an incomplete circuit; return false.
                return false;
            }
            else
            {
                // Else, we can not confirm a complete or incomplete circuit; continue looping the test, using the next outputNode.
                return outputNode.TestLiveCircuit(sourceID);
            }
        }
    }

    /// <summary>Holds base calculations for determining electrical values and functionality.</summary>
    public static class Electrical
    {
        #region Ohm's Law
        /// <summary>Calculates a derived voltage, in volts, using Ohm's law.</summary>
        /// <params name="current">The value of the current, in amps.</params>
        /// <params name="resistance">The value of the resistance, in ohms.</params>
        /// <returns>The total voltage, derived from the current and resistance, in volts.</returns>
        public static float CalculateVoltage(float current, float resistance)
        {
            // Ohm's law states that voltage = current * resistance.
            return current * resistance;
        }

        /// <summary>Calculates a derived current, in amps, using Ohm's law.</summary>
        /// <params name="voltage">The value of the voltage, in volts.</params>
        /// <params name="resistance">The value of the resistance, in ohms.</params>
        /// <returns>The total current, derived from the voltage and resistance, in amps.</returns>
        public static float CalculateCurrent(float voltage, float resistance)
        {
            // Ohm's law states that current = voltage / resistance.
            return voltage / resistance;
        }

        /// <summary>Calculates a derived resistance, in ohms, using Ohm's law.</summary>
        /// <params name="current">The value of the current, in amps.</params>
        /// <params name="voltage">The value of the voltage, in volts.</params>
        /// <returns>The total resistance, derived from the current and voltage, in ohms.</returns>
        public static float CalculateResistance(float current, float voltage)
        {
            // Ohm's law states that resistance = voltage / current.
            return voltage / current;
        }
        #endregion

        #region Total Resistance
        /// <summary>Calculates the total resistance of a set of resistors in parallel.</summary>
        /// <params name="resistances">The set of resistance values being totalled, in ohms.</summary>
        /// <returns>The equivalent resistance value of the provided resistances, in ohms.</summary>
        public static float CalculateTotalParallelResistance(params float[] resistances)
        {
            // Create a placeholder value, to start adding the resistances into an inverted sum.
            float totalInvertedResistance = 0f;

            for(int i = 0; i < resistances.Length; i++)
            {
                // Equivalent resistance in parallel states that (1/Rt = 1/R1 + .. 1/RN). For each provided 
                // resistance value, calculate the 1/Rn value into the inverted sum.
                totalInvertedResistance += (1.0f / resistances[i]);
            }

            // Final equivalent resistance in parallel states that Rt = 1 / (1/R1 + .. 1/RN). Find the total 
            // resistance using the inverted sum, and return it as the final sum.
            return (1.0f / totalInvertedResistance);
        }

        /// <summary>Calculates the total resistance of a set of resistors in both series and parallel.</summary>
        /// <params name="resistances">The set of resistance values being totalled, in ohms, provided as a series 
        /// of arrays; where each individual array is a series in parallel with each other.</summary>
        /// <returns>The equivalent resistance value of the provided resistances, in ohms.</summary>
        /// <remarks>It is important to take note of the order of dimension, when handling the resistances array. Each 
        /// individual array should represent a seperate series of resistors, where each array of resistors runs in 
        /// parallel.</remarks>
        public static float CalculateTotalResistance(float[][] resistances)
        {
            // Get the number of circuits in the resistances array, to create an array for storing individual summed values.
            int circuitCount = resistances.GetLength(0);
            float[] totalSeriesResistances = new float[circuitCount];

            for(int i = 0; i < circuitCount; i++)
            {
                // When finding equivalent resistance in a complex circuit, we first minimalise the circuit; we can 
                // do this by first finding the individual equivalent resistances of each set of resistors in series.
                // For each individual array of resistances running in series, CalculateTotalSeriesResistance, and 
                // set it as the respective equivalent resistance in the totalSeriesResistances array.
                totalSeriesResistances[i] = CalculateTotalSeriesResistance(resistances[i]);
            }
            
            // With the minimalised array of equivalent resistances in parallel, we can work out the total equivalent sum.
            // CalculateTotalParallelResistance for the totalSeriesResistances array, and return it as the final sum.
            return CalculateTotalParallelResistance(totalSeriesResistances);
        }

        /// <summary>Calculates the total resistance of a set of resistors in series.</summary>
        /// <params name="resistances">The set of resistance values being totalled, in ohms.</summary>
        /// <returns>The equivalent resistance value of the provided resistances, in ohms.</summary>
        public static float CalculateTotalSeriesResistance(params float[] resistances)
        {
            // Create a placeholder value, to start adding the resistances into a total sum.
            float totalResistance = 0f;

            for(int i = 0; i < resistances.Length; i++)
            {
                // Equivalent resistance in series states that Rt = R1 + .. RN. For each provided resistance 
                // value, add it to the total sum.
                totalResistance += resistances[i];
            }

            // Return the total sum.
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
        public const string inputNode = "The ElectricalComponent node supplying power into this ElectricalComponent.";
        public const string outputNode = "The ElectricalComponent node drawing power out of this ElectricalComponent.";
        public const string id = "The unique identification ID for this ElectricalComponent. Set dynamically; DO NOT EDIT";
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
