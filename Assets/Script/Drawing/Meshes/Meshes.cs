using UnityEngine;

namespace Drawing.Meshes
{
    interface IMeshGeneratable
    {
        void Generate();
        
        bool generated { get; }
    }
}