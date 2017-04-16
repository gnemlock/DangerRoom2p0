using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narrative
{
    public class Script : MonoBehaviour
    {
        public Dialogue[] dialogue;
    }
    
    [System.Serializable]
    public struct Dialogue
    {
        public DialogueLine[] lines;
    }
    
    [System.Serializable]
    public struct DialogueLine
    {
        public int actorID;
        public string line;
    }
}