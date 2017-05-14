using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Debugging.Physics
{
    
    public class VelocityGrapher : MonoBehaviour
    {
        public int sampleSize;
        float sampleLength;
        float maximumVelocity;
        float minimumVelocity;
        public int[] velocitySamples;
        public int newSampleSize;
        
        public void AdjustSampleRate(int newSampleSize)
        {
            sampleLength = Time.fixedDeltaTime * newSampleSize;
            
            if(velocitySamples == null)
            {
                velocitySamples = new int[newSampleSize];
            }
            else
            {
                int sampleSizeDelta = newSampleSize - sampleSize;
                
                if(sampleSizeDelta > 0)
                {
                    System.Array.Resize(ref velocitySamples, newSampleSize);
                    
                    for(int i = 1; i < sampleSizeDelta; i++)
                    {
                        velocitySamples[newSampleSize - i] = velocitySamples[sampleSize - i];
                        velocitySamples[sampleSize - i] = 0;
                    }
                }
                else if(sampleSizeDelta < 0)
                {
                    sampleSizeDelta *= -1;
                    
                    Debug.Log(newSampleSize);
                    for(int i = velocitySamples.Length - 1; i > newSampleSize; i--)
                    {
                        Debug.Log(velocitySamples.Length + " " + newSampleSize);
                        Debug.Log(velocitySamples[i - sampleSizeDelta] + " < " + velocitySamples[i]);
                        velocitySamples[i - sampleSizeDelta] = velocitySamples[i];
                    }   
                    System.Array.Resize(ref velocitySamples, newSampleSize);
                }
            }
            
            //this.sampleSize = sampleSize;
        }
        
        public void AdjustSampleRate(float sampleLength)
        {
            int sampleSize = (int)(sampleLength / Time.fixedDeltaTime);
            AdjustSampleRate(sampleSize);
        }
    }
}

namespace Debugging.Physics.Utility
{
    [CustomEditor(typeof(VelocityGrapher))] public class VelocityGrapherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if(GUILayout.Button("AdjustSampleRate"))
            {
                VelocityGrapher velocityGrapher = target as VelocityGrapher;
                
                velocityGrapher.AdjustSampleRate(velocityGrapher.newSampleSize);
            }
        }
    }
}