using UnityEngine;
using UnityEditor;

namespace Narrative
{
    //TODO:Correctly increment ID
    //TODO:Correctly save data
    //TODO:Warning message when remove all
    //TODO:Colour actors with ID
    //TODO:Implement Sprite selector
    //TODO:Implement Font selector
    //TODO:Implement preview for splash
    public class NarrativeActorListEditor : EditorWindow
    {
        public static ActorListing[] actors = new ActorListing[0];
        
        private int length { get { return actors.Length; } }
        
        [MenuItem ("Narrative/Actor List")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(NarrativeActorListEditor));
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Actors", EditorStyles.boldLabel);
            
            if(GUILayout.Button("Add"))
            {
                AddActor();
            }
            
            if(GUILayout.Button("Remove All"))
            {
                ClearAll();
            }
            
            if(length > 0)
            {
                for(int actorID = 0; actorID < length; actorID++)
                {
                    DisplayActor(actorID);
                }
            }
        }
        
        private void AddActor()
        {
            System.Array.Resize(ref actors, length + 1);
            actors[length].id = length - 1;
        }
        
        private void ClearAll()
        {
            System.Array.Resize(ref actors, 0);
        }
        
        private void DisplayActor(int actorID)
        {
            GUILayout.Label(DeriveActorLabel(actors[actorID]), EditorStyles.boldLabel);
            actors[actorID].name = EditorGUILayout.TextField("Name: ", actors[actorID].name);
            actors[actorID].colour = EditorGUILayout.ColorField("Colour: ", actors[actorID].colour);
        }
                
        private string DeriveActorLabel(ActorListing actor)
        {
            return actor.id.ToString() + ": " + actor.name;
        }
    }

    [System.Serializable]
    public struct ActorListing
    {
        public int id;
        public string name;
        public Color colour;
        public Sprite backingImage;
        public Font font;
    }
}