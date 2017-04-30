using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Narrative
{
    [System.Serializable]
    public class ActorList : MonoBehaviour, IGameManagerInteractable
    {
        public static ActorList source;
        
        public ActorListing[] actors;
        
        public int length
        { 
            get 
            { 
                try
                {
                    return actors.Length;
                }
                catch(NullReferenceException exception)
                {
                    return 0;
                }
            } 
        }
        
       /* private void Awake()
        {
            if(source == null)
            {
                source = this;
                DontDestroyOnLoad(this);
            }
            else if(source != this)
            {
                #if UNITY_EDITOR
                DestroyImmediate(this);
                #else
                Destroy(this);
                #endif
            }
        }*/
        
        public ActorListing this[int index]
        {
            get 
            { 
                return actors[index]; 
            }
            set 
            { 
                actors[index] = value; 
            }
        }
    }

    [System.Serializable]
    public class ActorListing
    {
        public string name;
        public Color colour;
        public Sprite backingSprite;
        public Sprite actorSprite;
        public Font font;

        #if UNITY_EDITOR
        public void OnGUI()
        {
            name = EditorGUILayout.TextField("Name: ", name);
            colour = EditorGUILayout.ColorField("Text Colour: ", colour);
            font = (Font)EditorGUILayout.ObjectField("Text Font: ", font, typeof(Font), false);
            actorSprite = (Sprite)EditorGUILayout.ObjectField("Actor Image: ", actorSprite, 
                typeof(Sprite), false);
            backingSprite = (Sprite)EditorGUILayout.ObjectField("Backing Image: ", backingSprite, 
                typeof(Sprite), false);
        }
        #endif
    }
}