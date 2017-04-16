using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narrative
{
    public class ActorList : MonoBehaviour
    {
        public ActorListing[] actors;
    }
    
    [System.Serializable]
    public struct ActorListing
    {
        public Font font;
        public Sprite background;
        public Color fontColour;
        public string name;
    }
}