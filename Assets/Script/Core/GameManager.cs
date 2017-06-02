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

[ExecuteInEditMode] public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int targetComponent;
    
    [SerializeField] public IGameManagerInteractable[] interactableComponents;

    private void Awake()
    {
        ImplementSingletonStructure();
    }

    private void ImplementSingletonStructure()
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
    
    public IGameManagerInteractable FindInteractable(System.Type type)
    {
        if(interactableComponents != null)
        {
            for(int i = 0; i < interactableComponents.Length; i++)
            {
                Debug.Log(interactableComponents[i].GetType().ToString() + " " + type.ToString() 
                    + " " + (interactableComponents[i].GetType() == type));
                
                if(interactableComponents[i].GetType() == type)
                {
                    return interactableComponents[i];
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
    
    public void OutputComponentList()
    {
        if(interactableComponents == null)
        {
            Debug.Log("No components");
        }
        else
        {
            for(int i = 0; i < interactableComponents.Length; i++)
            {
                Debug.Log(i + ": " + interactableComponents[i].GetType().ToString());
            }
        }
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
}

public interface IGameManagerUpdatable
{
    void Update();
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GameManager gameManager = target as GameManager;
        if(GUILayout.Button("Output to Debug"))
        {            
            gameManager.OutputComponentsToDebug();
        }
        
        if(GUILayout.Button("Print Component List"))
        {
            gameManager.OutputComponentList();
        }
        
        if(GUILayout.Button("Remove Target Component"))
        {
            gameManager.RemoveInteractable(gameManager.targetComponent);
        }
    }
}
#endif