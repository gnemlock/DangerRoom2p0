using UnityEngine;

namespace Generic
{
    [System.Serializable]
    public struct FloatRange
    {
        public float minimum;
        public float maximum;
        
        public float randomRange
        {
            get 
            {
                return Random.Range(minimum, maximum);
            }
        }
    }
}