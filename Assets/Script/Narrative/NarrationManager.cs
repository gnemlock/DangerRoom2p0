using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narrative
{
    public class NarrationManager : MonoBehaviour
    {
        public ActorList mainActorList;
        public NarrativeScript script;
        public ScrollingDialogue dialogueUI;
        
        public static NarrationManager instance { get; private set; }
    
        #if UNITY_EDITOR
        public NarrationManager()
        {
            ImplementSingletonStructure();
        }
        #endif
    
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
        
        public void AttachMainActorList(ActorList mainActorList)
        {
            this.mainActorList = mainActorList;
        }
        
        public Actor GetActorListing(int actorID)
        {
            return mainActorList[actorID];
        }
    }
}