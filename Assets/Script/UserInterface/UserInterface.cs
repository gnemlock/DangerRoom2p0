﻿using UnityEngine;

namespace UserInterface
{
    [System.Serializable]
    public struct IntColour
    {
        public Color colour;
        public int value;
        
        public IntColour(Color colour, int value)
        {
            this.colour = colour;
            this.value = value;
        }
    }
    
    [System.Serializable]
    public struct PercentColour
    {
        public Color colour;
        [RangeAttribute(0f, 1.0f)] public float value;
    }
}