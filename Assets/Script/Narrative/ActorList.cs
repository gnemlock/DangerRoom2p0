using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Narrative
{
    #if UNITY_EDITOR
    using Tooltips = Narrative.Utility.ActorListTooltips;
    #endif

    /// <summary>A singleton class designed to store all of the common 
    /// <see cref="Narrative.Actor"/>s for use in a game.</summary>
    /// <remarks><see cref="Narrative.Actor"/> represents characters that are commonly used in 
    /// narration, primarily through either <see cref="Narrative.Dialogue"/> or 
    /// <see cref="Narrative.Monologue"/>.</remarks>
    [System.Serializable] public class ActorList : MonoBehaviour, IGameManagerInteractable
    {
        /// <summary>Represents all of the <see cref="Narrative.Actor"/>s used in the game.
        /// </summary>
        /// <remarks><see cref="Narrative.Actor"/> represents characters that are commonly used in 
        /// narration, primarily through either <see cref="Narrative.Dialogue"/> or 
        /// <see cref="Narrative.Monologue"/>.</remarks>
        [Tooltip(Tooltips.actors)] public Actor[] actors;

        /// <summary>Represents the current active instance of <see cref="Narrative.ActorList"/>.
        /// </summary>
        /// <remarks>This reference is primarily used to enforce the singleton structure, and has 
        /// no additional practical use, as the <see cref="Narrative.ActorList"/> should be 
        /// directly accessed through the Editor.</remarks>
        private static ActorList instance;

        /// <summary>Retrieves the length of the <see cref="Narrative.ActorList.Actor"/> array.
        /// </summary>
        /// <remarks>An encountered <see cref="System.NullReferenceException"/> will return a value 
        /// of 0, so a seemingly empty array should not be assumed to be initialised.</remarks>
        public int length
        { 
            get 
            { 
                try
                {
                    // Try to return the length of the actors array.
                    return actors.Length;
                }
                catch(NullReferenceException exception)
                {
                    // If we encounter a NullReferenceException, return a length of 0.
                    return 0;
                }
            } 
        }

        #if UNITY_EDITOR
        /// <summary>Initializes a new instance of the <see cref="Narrative.ActorList"/> 
        /// class.</summary>
        public ActorList()
        {
            // Ensure singleton implementation.
            ImplementSingletonStructure();
        }
        #endif

        /// <summary>This method will be called when this instance of 
        /// <see cref="Narrative.ActorList"/> is loaded.</summary>
        private void Awake()
        {
            // Ensure singleton implementation.
            ImplementSingletonStructure();
        }

        /// <summary>This method should be called upon creation of an instance of 
        /// <see cref="Narrative.ActorList"/> to ensure singleton implementation.</summary>
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

        /// <summary>Provides access to the <see cref="Narrative.Actor"/> at the specified index of 
        /// <see cref="Narrative.ActorList.actors"/>.</summary>
        /// <param name="index">The index for the requested string 
        /// from <see cref="Narrative.Narration.lines"/>.</param>
        public Actor this[int index]
        {
            get 
            { 
                // Return the specified actor, by index.
                return actors[index]; 
            }
            set
            {
                // Set the specified actor, by index.
                actors[index] = value;
            }
        }
    }

    /// <summary>Represents a character for use in narration.</summary>
    /// <remarks><see cref="Narrative.Actor"/> represents characters that are commonly used in 
    /// narration, primarily through either <see cref="Narrative.Dialogue"/> or 
    /// <see cref="Narrative.Monologue"/>.<br/>
    /// <br/>
    /// An <see cref="Narrative.Actor"/> may also be used to temporarily represent other sources 
    /// of narrative text, such as an item being inspected, or a prop being interacted with.
    /// </remarks>
    [System.Serializable] public class Actor
    {
        /// <summary>The name of the <see cref="Narrative.Actor"/>.</summary>
        [Tooltip(Tooltips.name)] public string name;
        /// <summary>The <see cref="UnityEngine.Font"/> used in displaying text from this 
        /// <see cref="Narrative.Actor"/>.</summary>
        /// <remarks>This reference may be null, so application should permit for a default 
        /// <see cref="UnityEngine.Font"/>.</remarks>
        [Tooltip(Tooltips.font)] public Font font;
        /// <summary>The <see cref="UnityEngine.Color"/> used in displaying text from this 
        /// <see cref="Narrative.Actor"/>.</summary>
        /// <remarks>This reference may be null, so application should permit for a default 
        /// <see cref="UnityEngine.Color"/>.</remarks>
        [Tooltip(Tooltips.colour)] public Color colour;
        /// <summary>The <see cref="UnityEngine.Sprite"/> displayed behind the 
        /// <see cref="Narrative.Actor"/> when corresponding text is being displayed.</summary>
        /// <remarks>This reference may not be used, so application should permit for a default or 
        /// absent <see cref="UnityEngine.Sprite"/></remarks>
        [Tooltip(Tooltips.backingSprite)] public Sprite backingSprite;
        /// <summary>The <see cref="UnityEngine.Sprite"/> used to represent the 
        /// <see cref="Narrative.Actor"/> when corresponding text is being displayed.</summary>
        /// <remarks>This reference may not be used, so application should permit for a default or 
        /// absent <see cref="UnityEngine.Sprite"/></remarks>
        [Tooltip(Tooltips.actorSprite)] public Sprite actorSprite;

        #region Constructors

        /* DESIGN DECISION: CONSTRUCTORS
         * 
         * Constructors are fully qualified to take any combination of internal components. Order 
         * follows an approximate binary pattern, though at times, parameter order needs to be 
         * swapped due to the presence of two Sprites. A fully fledged binary order would result 
         * in several identical signatures. Furthermore, this means that one constructor can not 
         * accept a default value for the string, as default values can not be placed in front of 
         * non-default values. Instead, there is a final constructor that qualifies the removal of 
         * the string.
         */

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="font">The font used to display this <see cref="Narrative.Actor"/>s text 
        /// during narration.</param>
        /// <param name="colour">The colour used to display this <see cref="Narrative.Actor"/>s 
        /// text during narration.</param>
        /// <param name="backingSprite">The sprite to display behind this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Font font, Color colour, Sprite backingSprite, Sprite actorSprite,
            string name = "")
        {
            this.font = font;
            this.colour = colour;
            this.backingSprite = backingSprite;
            this.actorSprite = actorSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="font">The font used to display this <see cref="Narrative.Actor"/>s text 
        /// during narration.</param>
        /// <param name="colour">The colour used to display this <see cref="Narrative.Actor"/>s 
        /// text during narration.</param>
        /// <param name="backingSprite">The sprite to display behind this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Font font, Color colour, Sprite backingSprite, string name = "")
        {
            this.font = font;
            this.colour = colour;
            this.backingSprite = backingSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="font">The font used to display this <see cref="Narrative.Actor"/>s text 
        /// during narration.</param>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="colour">The colour used to display this <see cref="Narrative.Actor"/>s 
        /// text during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Font font, Sprite actorSprite, Color colour, string name = "")
        {
            this.font = font;
            this.colour = colour;
            this.actorSprite = actorSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="font">The font used to display this <see cref="Narrative.Actor"/>s text 
        /// during narration.</param>
        /// <param name="colour">The colour used to display this <see cref="Narrative.Actor"/>s 
        /// text during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Font font, Color colour, string name = "")
        {
            this.font = font;
            this.colour = colour;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="font">The font used to display this <see cref="Narrative.Actor"/>s text 
        /// during narration.</param>
        /// <param name="backingSprite">The sprite to display behind this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Font font, Sprite backingSprite, Sprite actorSprite, string name = "")
        {
            this.font = font;
            this.backingSprite = backingSprite;
            this.actorSprite = actorSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="font">The font used to display this <see cref="Narrative.Actor"/>s text 
        /// during narration.</param>
        /// <param name="backingSprite">The sprite to display behind this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Font font, Sprite backingSprite, string name = "")
        {
            this.font = font;
            this.backingSprite = backingSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="font">The font used to display this <see cref="Narrative.Actor"/>s text 
        /// during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Sprite actorSprite, Font font, string name = "")
        {
            this.font = font;
            this.actorSprite = actorSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="font">The font used to display this <see cref="Narrative.Actor"/>s text 
        /// during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Font font, string name = "")
        {
            this.font = font;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="colour">The colour used to display this <see cref="Narrative.Actor"/>s 
        /// text during narration.</param>
        /// <param name="backingSprite">The sprite to display behind this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Color colour, Sprite backingSprite, Sprite actorSprite, string name = "")
        {
            this.colour = colour;
            this.backingSprite = backingSprite;
            this.actorSprite = actorSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="colour">The colour used to display this <see cref="Narrative.Actor"/>s 
        /// text during narration.</param>
        /// <param name="backingSprite">The sprite to display behind this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Color colour, Sprite backingSprite, string name = "")
        {
            this.colour = colour;
            this.backingSprite = backingSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="colour">The colour used to display this <see cref="Narrative.Actor"/>s 
        /// text during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Sprite actorSprite, Color colour, string name = "")
        {
            this.colour = colour;
            this.actorSprite = actorSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="colour">The colour used to display this <see cref="Narrative.Actor"/>s 
        /// text during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Color colour, string name = "")
        {
            this.colour = colour;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="backingSprite">The sprite to display behind this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Sprite backingSprite, Sprite actorSprite, string name = "")
        {
            this.backingSprite = backingSprite;
            this.actorSprite = actorSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="backingSprite">The sprite to display behind this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(Sprite backingSprite, string name = "")
        {
            this.backingSprite = backingSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        public Actor(string name, Sprite actorSprite)
        {
            this.actorSprite = actorSprite;
            this.name = name;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="actorSprite">"The sprite to display when representing this 
        /// <see cref="Narrative.Actor"/> during narration.</param>
        public Actor(Sprite actorSprite)
        {
            this.actorSprite = actorSprite;
        }

        /// <summary>Initializes a new instance of the <see cref="Narrative.Actor"/> class.
        /// </summary>
        /// <param name="name">The name of this <see cref="Narrative.Actor"/>, as it should appear 
        /// during narration.</param>
        public Actor(string name = "")
        {
            this.name = name;
        }
        #endregion

        #if UNITY_EDITOR
        /// <summary>This method should be called to draw the <see cref="Narrative.Actor"/> GUI to 
        /// the Inspector via a seperate OnGUI method. THIS METHOD IS EDITOR ONLY.</summary>
        public void OnGUI()
        {
            // Draw a text field to allow the user to enter a name for the actor.
            name = EditorGUILayout.TextField("Name: ", name);

            // Draw an object field to allow the user to specify a font for the actor text, from 
            // fonts available in the project view. Do not allow scene objects.
            font = (Font)EditorGUILayout.ObjectField("Text Font: ", font, typeof(Font), false);

            // Draw a colour field to allow the user to select a colour for the actor text.
            colour = EditorGUILayout.ColorField("Text Colour: ", colour);

            // Draw an object field to allow the user to specify sprites for the actor and backing 
            // image, from the sprites available in the project view. Do not allow scene objects.
            actorSprite = (Sprite)EditorGUILayout.ObjectField("Actor Image: ", actorSprite, 
                typeof(Sprite), false);
            backingSprite = (Sprite)EditorGUILayout.ObjectField("Backing Image: ", backingSprite, 
                typeof(Sprite), false);
        }
        #endif
    }
}

namespace Narrative.Utility
{
    #if UNITY_EDITOR
    public static class ActorListTooltips
    {
        public const string actors = "The global list of available actors.";
        public const string name = "The name of this actor, as it should appear during narration.";
        public const string font = "The font used to display the actors text during narration.";
        public const string colour = "The colour used to display the actors text during narration.";
        public const string backingSprite = "The sprite to display behind the actor during "
            + "narration.";
        public const string actorSprite = "The sprite to display when representing the actor "
            + "during narration.";
    }
    #endif
}