using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

//TODO:Get this working
//TODO:Get this exchanging with the NarrativeActorListEditor
//TODO:Convert components to ScriptableObject
//TODO:Implement serialisation in ScriptableObjects

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] public IGameManagerInteractable[] interactableComponents;
    
    #if UNITY_EDITOR
    public GameManager()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            #if UNITY_EDITOR
            DestroyImmediate(this);
            #else
            Destroy(this);
            #endif
        }
    }
    #endif

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            #if UNITY_EDITOR
            DestroyImmediate(this);
            #else
            Destroy(this);
            #endif
        }
    }
    
    #if UNITY_EDITOR
    public int AttachInteractable(IGameManagerInteractable interactable)
    {
        try
        {
            Array.Resize(ref interactableComponents, interactableComponents.Length + 1);
        }
        catch(NullReferenceException nullReferenceException)
        {
            interactableComponents = new IGameManagerInteractable[1];
        }

        interactableComponents[interactableComponents.Length - 1] = interactable;
        
        return interactableComponents.Length - 1;
         }
    
    public IGameManagerInteractable FindInteractable(string name)
    {
        if(interactableComponents != null)
        {
            foreach(IGameManagerInteractable interactable in interactableComponents)
            {
                Debug.Log(interactable.name + " " + name);
                
                if(interactable.name == name)
                {
                    return interactable;
                }
            }
        }
        
        return null;
    }
    
    public void RemoveInteractable(int interactableIndex)
    {
        for(int i = interactableIndex; i < interactableComponents.Length - 1; i++)
        {
            interactableComponents[i] = interactableComponents[i - 1];
        }
        
        Array.Resize(ref interactableComponents, interactableComponents.Length - 1);
    }
    
    public void OutputComponentsToDebug()
    {
        if(interactableComponents != null)
        {
            foreach(IGameManagerInteractable interactable in interactableComponents)
            {
                Debug.Log(interactable.ToString());
            }
        }
    }
    #endif
}

public interface IGameManagerInteractable
{
    string name { get; }
}

public interface IGameManagerUpdatable
{
    void Update();
    
    string name { get; }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if(GUILayout.Button("Output to Debug"))
        {
            GameManager gameManager = target as GameManager;
            
            gameManager.OutputComponentsToDebug();
        }
    }
}
#endif