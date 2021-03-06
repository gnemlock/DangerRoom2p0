using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Narrative
{
    using Dimensions = Narrative.Utility.NarrativeActorListEditorDimensions;
    using Labels = Narrative.Utility.NarrativeActorListEditorLabels;

    //TODO:Code Documentation
    //TODO:Help function to bring up in-house help file
    //TODO:Default Font and BackingImage
    //TODO:Set up actor headers to allow expanding/compressing additional data
    //TODO:Colour actors with ID
    //TODO:Implement preview for splash
    //TODO:Possibly replace button character icons with texture icons
    //TODO:Update editor to push data to monobehaviour with edits, adds and removes
    //TODO:Convert implementation to ScriptableObject
    public class NarrativeActorListEditor : EditorWindow
    {
        /// <summary>The list of actors represented by <see cref="Narrative.ActorListing"/> 
        /// structures.</summary>
        public List<Actor> actors;

        /// <summary>Current position of the scroll view displaying the 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</summary>
        private Vector2 scrollPosition;

        /// <summary>Gets the current length of the 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</summary>
        /// <value>The length of the <see cref="Narrative.NarrativeActorListEditor.actors"/> list.
        /// </value>
        /// <remarks>If the list is null, a new 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list will be created, returning 
        /// a length of 0.</remarks>
        private int length
        { 
            get 
            { 
                try
                {
                    // Try to return the length of the actors list.
                    return actors.Count;
                }
                catch(NullReferenceException exception)
                {
                    // If this causes a NullReferenceException, the list has not been created, yet.
                    // Instantiate the actors list, and return a length value of 0.
                    actors = new List<Actor>();
                    return 0;
                }
            } 
        }

        /// <summary>Displays an window instance of the 
        /// <see cref="Narrative.NarrativeActorListEditor"/>, with preference to dock with an 
        /// inspector window.</summary>
        [MenuItem (Labels.menuName)] public static void ShowWindow()
        {
            // Using reflection, retrieve the inspector type, through the editor assembly.
            var editor = typeof(Editor).Assembly;
            var inspector = editor.GetType(Labels.inspector);
            
            // Retrieve and show the NarrativeActorListEditor window; use the Labels.windowHeader 
            // as its title, set it to the current focus, and attempt to dock it next to an 
            // inspector window.
            EditorWindow.GetWindow<NarrativeActorListEditor>(Labels.windowHeader, true, inspector);
        }

        /// <summary>The Awake method will be called when an instance of 
        /// <see cref="Narrative.NarrativeActorListEditor"/> is loaded, specifically when a 
        /// window is opened.</summary>
        void Awake()
        {
            // Set this instance to be hidden from the hierarchy, and unassociated with 
            // the scene and resources. Pull the current actor list into the local scene actors 
            // array.
            hideFlags = HideFlags.HideAndDontSave;
            PullActorList();
        }

        /// <summary>The Awake method will be called when an instance of 
        /// <see cref="Narrative.NarrativeActorListEditor"/> is unloaded, specifically when a 
        /// window is closed.</summary>
        void OnDestroy()
        {
            // Push the local actors list to the current scene actors array.
            PushActorList();
        }

        /// <summary>This method will be called whenever the Unity editor attempts to draw the 
        /// <see cref="Narrative.NarrativeActorListEditor"/> GUI to the inspector.</summary>
        private void OnGUI()
        {
            // Begin a horizontal layout, drawing elements in horizontal alignment with each other.
            GUILayout.BeginHorizontal();

            if(GUILayout.Button(Labels.addButton))
            {
                // Add a button for adding another actor to the actors list.
                AddActor();
            }

            if(GUILayout.Button(Labels.removeAllButton))
            {
                // Add a button for removing all actors from the actors list.
                ClearAll();
            }

            // End the horizontal layout.
            GUILayout.EndHorizontal();

            if(length > 0)
            {
                // If the actors list array is not empty, create a scrolling layout, drawing 
                // elements inside a scrollable area; keep record of the current scroll position.
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                for(int actorID = 0; actorID < length; actorID++)
                {
                    // For each actor in the actors list, draw the actors header label, 
                    // and run its GUI method.
                    DrawActorHeader(actorID);
                    actors[actorID].OnGUI();

                    if(actorID + 1 < length)
                    {
                        // If we are not on the last actor, draw a space.
                        EditorGUILayout.Space();
                    }
                }

                // End the scrolling layout.
                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>Draws the header for an <see cref="Narrative.ActorListing"/> in the GUI.
        /// </summary>
        /// <param name="actorID">The index for the <see cref="Narrative.ActorListing"/> in the 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</param>
        private void DrawActorHeader(int actorID)
        {
            // Begin a horizontal layout, drawing elements in horizontal alignment with each other.
            EditorGUILayout.BeginHorizontal();

            // Display a bold label, identifying the actor name and its respective ID.
            GUILayout.Label(actorID + Labels.seperator + actors[actorID].name, 
                EditorStyles.boldLabel);

            if(GUILayout.Button(Labels.moveActorUpButton, GUILayout.Width(Dimensions.buttonSize)))
            {
                // Add a button for moving the actor up in the list order.
                MoveActorUp(actorID);
            }
            
            if(GUILayout.Button(Labels.moveActorDownButton, GUILayout.Width(Dimensions.buttonSize)))
            {
                // Add a button for moving the actor down in the list order.
                MoveActorDown(actorID);
            }

            if(GUILayout.Button(Labels.removeActorButton, GUILayout.Width(Dimensions.buttonSize)))
            {
                // Add a button for removing the actor from the list.
                RemoveActor(actorID);
            }

            // End the horizontal layout.
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>Moves an <see cref="Narrative.ActorListing"/> in the 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</summary>
        /// <param name="initialActorID">The ID of the <see cref="Narrative.ActorListing"/> to be 
        /// moved in the <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</param>
        /// <param name="newActorID">The ID position in the 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list to which we are moving the 
        /// <see cref="Narrative.ActorListing"/>.</param>
        private void MoveActor(int initialActorID, int newActorID)
        {
            // Store a local copy of the actor we are about to move.
            Actor actor = actors[initialActorID];

            // Remove the actor from the actors list, and replace it at the desired position.
            actors.RemoveAt(initialActorID);
            actors.Insert(newActorID, actor);

            // Set the GUI focus to null, so we no longer have focus on the original actor 
            // position; this will ensure all data is immediately exchanged and visible.
            GUI.FocusControl(null);
        }

        /// <summary>Moves an <see cref="Narrative.ActorListing"/> down in the 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</summary>
        /// <param name="actorID">The ID of the <see cref="Narrative.ActorListing"/> to be 
        /// moved down in the <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</param>
        private void MoveActorDown(int actorID)
        {
            if(actorID >= 0 && actorID < length - 1)
            {
                // If the ID is within the bounds of the list, and we are not already dealing 
                // with the last actor in the list, move the actor down one spot in the list.
                MoveActor(actorID, actorID + 1);
            }
        }

        /// <summary>Moves an <see cref="Narrative.ActorListing"/> up in the 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</summary>
        /// <param name="actorID">The ID of the <see cref="Narrative.ActorListing"/> to be 
        /// moved up in the <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</param>
        private void MoveActorUp(int actorID)
        {
            if(actorID > 0 && actorID < length)
            {
                // If the ID is within the bounds of the list, and we are not already dealing 
                // with the first actor in the list, move the actor up one spot in the list.
                MoveActor(actorID, actorID - 1);
            }
        }

        /// <summary>Adds a new <see cref="Narrative.ActorListing"/> to the 
        /// <see cref="Narrative.NarrativeActorListEditor.actors"/> list.</summary>
        private void AddActor()
        {
            // Add a new ActorListing object to the actors list.
            actors.Add(new Actor());
        }
        
        private void InitialiseList()
        {
            actors = new List<Actor>();
        }
        
        private void ClearAll()
        {
            Undo.RecordObject(this, "Clear all actor listings.");
            actors.Clear();
        }
        
        private void RemoveActor(int actorID)
        {
            Undo.RecordObject(this, "Remove actor " + actorID);
            actors.RemoveAt(actorID);
        }
        
        private void PullActorList()
        {
            ActorList actorList = GetActorList();
            
            actors = new List<Actor>();
            
            for(int actorIndex = 0; actorIndex < actorList.length; actorIndex++)
            {
                actors.Add(actorList[actorIndex]);
            }
        }
        
        private void PushActorList()
        {
            ActorList actorList = GetActorList();
            
            actorList.actors = new Actor[length];
            
            for(int actorIndex = 0; actorIndex < length; actorIndex++)
            {
                actorList[actorIndex] = actors[actorIndex];
            }
        }
        
        private static ActorList GetActorList()
        {
            if(GameManager.instance == null)
            {
                GameObject gameManager = GameObject.Find("__GameManager");

                if(gameManager == null)
                {
                    gameManager = new GameObject("__GameManager");
                }
                
                gameManager.AddComponent<GameManager>();
            }

            ActorList actorList 
                = GameManager.instance.FindInteractable(typeof(ActorList)) as ActorList;

            if(actorList == null)
            {
                actorList = GameManager.instance.gameObject.AddComponent<ActorList>();
                GameManager.instance.AttachInteractable(actorList);
            }
            
            return actorList;
        }
    }
}

namespace Narrative.Utility
{
    public static class NarrativeActorListEditorDimensions
    {
        public const float buttonSize = 25f;
    }
    
    public static class NarrativeActorListEditorLabels
    {
        public const string menuName = "Danger Room/Narrative/Actor List";
        public const string inspector = "UnityEditor.InspectorWindow";
        public const string windowHeader = "Actors";
        public const string addButton = "Add";
        public const string removeAllButton = "Remove All";
        public const string seperator = ": ";
        public const string moveActorDownButton = ">";
        public const string moveActorUpButton = "<";
        public const string removeActorButton = "X";
    }
}