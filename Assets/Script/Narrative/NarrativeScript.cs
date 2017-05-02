using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narrative
{
    #if UNITY_EDITOR
    using Tooltips = Narrative.Utility.NarrativeScriptTooltips;
    #endif

    /// <summary>A singleton class designed to store all of the narrative-based script 
    /// for use in a game.</summary>
    public class NarrativeScript : MonoBehaviour
    {
        /// <summary>Represents all of the <see cref="Narrative.Dialogue"/> used in the 
        /// game.</summary>
        /// <remarks><see cref="Narrative.Dialogue"/> represents narration between multiple 
        /// <see cref="Narrative.Actor"/>s.</remarks>
        public Dialogue[] dialogue;
        /// <summary>Represents all of the <see cref="Narrative.Monologue"/> used in the 
        /// game.</summary>
        /// <remarks><see cref="Narrative.Monologue"/> represents narration from a single  
        /// <see cref="Narrative.Actor"/>.</remarks>
        public Monologue[] monologue;
        /// <summary>Represents all of the <see cref="Narrative.Narration"/> used in the 
        /// game.</summary>
        /// <remarks><see cref="Narrative.Narration"/> represents narration from an alternate 
        /// source, such as an item description or a prop interaction.</remarks>
        public Narration[] narration;

        /// <summary>Represents the current active instance of 
        /// <see cref="Narrative.NarrativeScript"/>.</summary>
        /// <remarks>This reference is primarily used to enforce the singleton structure, and has 
        /// no additional practical use, as the <see cref="Narrative.NarrativeScript"/> should be 
        /// directly accessed through the Editor.</remarks>
        private static NarrativeScript instance;

        #if UNITY_EDITOR
        /// <summary>Initializes a new instance of the <see cref="Narrative.NarrativeScript"/> 
        /// class.</summary>
        public NarrativeScript()
        {
            // Ensure singleton implementation.
            ImplementSingletonStructure();
        }
        #endif

        /// <summary>This method will be called when this instance of 
        /// <see cref="Narrative.NarrativeScript"/> is loaded.</summary>
        private void Awake()
        {
            // Ensure singleton implementation.
            ImplementSingletonStructure();
        }

        /// <summary>This method should be called upon creation of an instance of 
        /// <see cref="Narrative.NarrativeScript"/> to ensure singleton implementation.</summary>
        private void ImplementSingletonStructure()
        {
            if(instance == null)
            {
                // If there is no instance reference, set this instance as the instance reference.
                instance = this;
            }
            else if(instance != this)
            {
                // If there is an instance reference, and it is not this instance, destroy it.
                #if UNITY_EDITOR
                DestroyImmediate(this);
                #else
                Destroy(this);
                #endif
            }
        }
    }

    /// <summary>Represents dialogue between multiple <see cref="Narrative.Actor"/>s.</summary>
    /// <remarks>Use this structure to represent conversation between multiple <
    /// see cref="Narrative.Actor"/>s. If you intend to represent narration from a single 
    /// <see cref="Narrative.Actor"/>, use <see cref="Narrative.Monologue"/>. If you intend to 
    /// represent narration from an alternate source, such as an item description or a prop 
    /// interaction, use <see cref="Narrative.Narration"/>.<br/>
    /// <br/>
    /// <see cref="Narrative.Dialogue"/> can be treated as an array, in order to directly access 
    /// <see cref="Narrative.Dialogue.lines"/>.</remarks>
    [System.Serializable] public struct Dialogue
    {
        #if UNITY_EDITOR
        /// <summary>The name associated with this <see cref="Narrative.Dialogue"/>. THIS VALUE IS 
        /// EDITOR ONLY.</summary>
        /// <remarks>This value is primarily used for easier association in the Editor. For 
        /// example, if this value is not empty, it will replace the ID identification in the 
        /// Inspector. As such, it is only available to the Editor, will not be included in 
        /// runtime code.</remarks>
        [Tooltip(Tooltips.dialogueName)] public string name;
        #endif

        /// <summary>The <see cref="Narrative.NarrativeLines"/> included in this instance of 
        /// <see cref="Narrative.Dialogue"/>.</summary>
        [Tooltip(Tooltips.dialogueLines)] public NarrativeLine[] lines;

        /// <summary>Provides access to the <see cref="Narrative.NarrativeLine"/> at the specified 
        /// index of <see cref="Narrative.Dialogue.lines"/>.</summary>
        /// <param name="index">The index for the requested <see cref="Narrative.NarrativeLine"/> 
        /// from <see cref="Narrative.Dialogue.lines"/>.</param>
        public NarrativeLine this[int index]
        {
            get
            {
                // Return the specified NarrativeLine, by index.
                return lines[index];
            }
            set
            {
                // Set the specified NarrativeLine, by index.
                lines[index] = value;
            }
        }
    }   

    /// <summary>Represents individual narration lines from a specific 
    /// <see cref="Narrative.Actor"/>.</summary>
    /// <remarks>This structure should primarily represent lines in a 
    /// <see cref="Narrative.Dialogue"/>, where multiple lines may be associated with multiple 
    /// <see cref="Narrative.Actor"/>s. For situations where there are not multiple 
    /// <see cref="Narrative.Actor"/>s present, consider using a <see cref="Narrative.Monologue"/>, 
    /// which contains a single global <see cref="Narrative.Monologue.actorID"/>.</remarks>
    [System.Serializable] public struct NarrativeLine
    {
        /// <summary>The primary <see cref="Narrative.ActorList"/> ID for the 
        /// <see cref="Narrative.Actor"/> delivering this <see cref="Narrative.NarrativeLine"/>.
        /// </summary>
        /// <remarks>This ID should reference an <see cref="Narrative.Actor"/> in a global 
        /// <see cref="Narrative.ActorList"/>. When pooling a localised 
        /// <see cref="Narrative.ActorList"/> for efficiency, consider using a 
        /// <see cref="System.Dictionary"/> to associate the original ID as a key, in order to 
        /// retain correct reference.</remarks>
        [Tooltip(Tooltips.narrativeLineActorID)] public int actorID;
        /// <summary>The narration line to be delivered by the <see cref="Narrative.Actor"/> 
        /// associated with the local <see cref="Narrative.NarrativeLine.actorID"/>.</summary>
        [Tooltip(Tooltips.narrativeLineLine)] public string line;
    }

    /// <summary>Represents monologue from a single <see cref="Narrative.Actor"/>.</summary>
    /// <remarks>Use this structure to represent narration from a single 
    /// <see cref="Narrative.Actor"/>. If you intend to represent conversation between multiple 
    /// <see cref="Narrative.Actor"/>s, use <see cref="Narrative.Dialogue"/>. If you intend to 
    /// represent narration from an alternate source, such as an item description or a prop 
    /// interaction, use <see cref="Narrative.Narration"/>.<br/>
    /// <br/>
    /// <see cref="Narrative.Monologue"/> can be treated as an array, in order to directly access 
    /// <see cref="Narrative.Monologue.lines"/>.</remarks>
    [System.Serializable] public struct Monologue
    {
        #if UNITY_EDITOR
        /// <summary>The name associated with this <see cref="Narrative.Monologue"/>. THIS VALUE IS 
        /// EDITOR ONLY.</summary>
        /// <remarks>This value is primarily used for easier association in the Editor. For 
        /// example, if this value is not empty, it will replace the ID identification in the 
        /// Inspector. As such, it is only available to the Editor, will not be included in 
        /// runtime code.</remarks>
        [Tooltip(Tooltips.monologueName)] public string name;
        #endif

        /// <summary>The primary <see cref="Narrative.ActorList"/> ID for the 
        /// <see cref="Narrative.Actor"/> delivering this <see cref="Narrative.Monologue"/>.
        /// </summary>
        /// <remarks>This ID should reference an <see cref="Narrative.Actor"/> in a global 
        /// <see cref="Narrative.ActorList"/>.</remarks>
        [Tooltip(Tooltips.monologueActorID)] public int actorID;
        /// <summary>The narration lines to be delivered by the <see cref="Narrative.Actor"/> 
        /// associated with the local <see cref="Narrative.NarrativeLine.actorID"/>.</summary>
        [Tooltip(Tooltips.monologueLines)] public string[] lines;

        /// <summary>Gets the string at the specified index of 
        /// <see cref="Narrative.Monologue.lines"/>.</summary>
        /// <param name="index">The index for the requested string 
        /// from <see cref="Narrative.Monologue.lines"/>.</param>
        public string this[int index]
        { 
            get
            {
                // Return the specified line, by index.
                return lines[index]; 
            }
            set
            {
                // Set the specified line, by index.
                lines[index] = value;
            }
        }
    }

    /// <summary>Represents narration from an alternate source.</summary>
    /// <remarks>Use this structure to represent narration from an alternate source, such as an 
    /// item description a or prop interaction.If you intend to represent conversation between 
    /// multiple <see cref="Narrative.Actor"/>s, use <see cref="Narrative.Dialogue"/>. If you 
    /// intend to represent narration from a single <see cref="Narrative.Actor"/>, use 
    /// <see cref="Narrative.Monologue"/>.<br/>
    /// <br/>
    /// <see cref="Narrative.Narration"/> can be treated as an array, in order to directly access 
    /// <see cref="Narrative.Narration.lines"/>.<br/>
    /// <br/>
    /// Due to the more expansive use of this structure, there is no specific 
    /// <see cref="Narrative.Actor"/> associated with it. As such, usage should manually account 
    /// for any further interface changes that might otherwise be automatically associated with 
    /// <see cref="Narrative.Actor"/>s, such as backing image, or the associated icon.</remarks>
    [System.Serializable] public struct Narration
    {
        #if UNITY_EDITOR
        /// <summary>The name associated with this <see cref="Narrative.Narration"/>. THIS VALUE IS 
        /// EDITOR ONLY.</summary>
        /// <remarks>This value is primarily used for easier association in the Editor. For 
        /// example, if this value is not empty, it will replace the ID identification in the 
        /// Inspector. As such, it is only available to the Editor, will not be included in 
        /// runtime code.</remarks>
        [Tooltip(Tooltips.narrationName)] public string name;
        #endif

        /// <summary>The narration lines to be delivered in the <see cref="Narrative.Narration"/>.
        /// </summary>
        [Tooltip(Tooltips.narrationLines)] public string[] lines;

        /// <summary>Provides access to the string at the specified index of 
        /// <see cref="Narrative.Narration.lines"/> .</summary>
        /// <param name="index">The index for the requested string 
        /// from <see cref="Narrative.Narration.lines"/>.</param>
        public string this[int index]
        {
            get
            {
                // Return the specified line, by index.
                return lines[index];
            }
            set
            {
                // Set the specified line, by index.
                lines[index] = value;
            }
        }
    }
}

namespace Narrative.Utility
{
    #if UNITY_EDITOR
    public static class NarrativeScriptTooltips
    {
        public const string dialogue = "The global list of available dialogue.";
        public const string monologue = "The global list of available monologue.";
        public const string narration = "The global list of available narration.";
        public const string dialogueName = "The Editor name associated with this dialogue.";
        public const string dialogueLines = "The narrative lines for this dialogue.";
        public const string narrativeLineActorID = "The global ID for the actor delivering this"
            + " narrative line.";
        public const string narrativeLineLine = "The narrative text for this narrative line.";
        public const string monologueName = "The Editor name associated with this monologue.";
        public const string monologueActorID = "The global ID for the actor delivering this"
            + " monologue.";
        public const string monologueLines = "The narrative text for this monologue.";
        public const string narrationName = "The Editor name associated with this narration.";
        public const string narrationLines = "The narrative text for this narration.";
    }
    #endif
}