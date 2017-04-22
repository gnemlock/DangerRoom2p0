using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Narrative
{
    using Dimensions = Narrative.Utility.NarrativeActorListEditorDimensions;

    //TODO:Colour actors with ID
    //TODO:Implement preview for splash
    public class NarrativeActorListEditor : EditorWindow
    {
        public List<ActorListing> actors;
        
        private Vector2 scrollPosition;
        
        private int length
        { 
            get 
            { 
                try
                {
                    return actors.Count;
                }
                catch(NullReferenceException exception)
                {
                    actors = new List<ActorListing>();
                    return 0;
                }
            } 
        }
        
        [MenuItem ("Danger Room/Narrative/Actor List")]
        public static void ShowWindow()
        {
            var editor = typeof(Editor).Assembly;
            var mainInspector = editor.GetType("UnityEditor.InspectorWindow");
            
            EditorWindow.GetWindow<NarrativeActorListEditor>("Actors", true, mainInspector);
        }
        
        void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }
        
        private void OnGUI()
        {            
            GUILayout.BeginHorizontal();
            
            if(GUILayout.Button("Add"))
            {
                AddActor();
            }
            
            if(GUILayout.Button("Remove All"))
            {
                ClearAll();
            }
            
            GUILayout.EndHorizontal();
            
            if(length > 0)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                for(int actorID = 0; actorID < length; actorID++)
                {
                    DrawActorHeader(actorID);
                    actors[actorID].OnGUI();
                    
                    if(actorID + 1 < length)
                    {
                        EditorGUILayout.Space();
                    }
                }
                
                EditorGUILayout.EndScrollView();
            }
        }
        
        private void DrawActorHeader(int actorID)
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label(actorID + ": " + actors[actorID].name, EditorStyles.boldLabel);
            
            if(GUILayout.Button("<", GUILayout.Width(Dimensions.buttonWidth)))
            {
                MoveActorUp(actorID);
            }
            
            if(GUILayout.Button(">", GUILayout.Width(Dimensions.buttonWidth)))
            {
                MoveActorDown(actorID);
            }
            
            if(GUILayout.Button("X", GUILayout.Width(Dimensions.buttonWidth)))
            {
                RemoveActor(actorID);
            }
        
            EditorGUILayout.EndHorizontal();
        }
        
        private void MoveActor(int initialActorID, int newActorID)
        {
            ActorListing actor = actors[initialActorID];
            actors.RemoveAt(initialActorID);
            actors.Insert(newActorID, actor);
            GUI.FocusControl(null);
        }
        
        private void MoveActorDown(int actorID)
        {
            if(actorID >= 0 && actorID < length - 1)
            {
                MoveActor(actorID, actorID + 1);
            }
        }
        
        private void MoveActorUp(int actorID)
        {
            if(actorID > 0 && actorID < length)
            {
                MoveActor(actorID, actorID - 1);
            }
        }
        
        private void AddActor()
        {
            actors.Add(new ActorListing());
        }
        
        private void InitialiseList()
        {
            actors = new List<ActorListing>();
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
    }

    [System.Serializable]
    public class ActorListing
    {
        public string name;
        public Color colour;
        public Sprite backingImage;
        public Font font;
        
        public void OnGUI()
        {
            name = EditorGUILayout.TextField("Name: ", name);
            colour = EditorGUILayout.ColorField("Text Colour: ", colour);
            font = (Font)EditorGUILayout.ObjectField("Text Font: ", font, typeof(Font), false);
            backingImage = (Sprite)EditorGUILayout.ObjectField("Backing Image: ", backingImage, 
                typeof(Sprite), false);
        }
    }
}

namespace Narrative.Utility
{
    public static class NarrativeActorListEditorDimensions
    {
        public const float buttonWidth = 25f;
    }
}