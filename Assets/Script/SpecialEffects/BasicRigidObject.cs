/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace SpecialEffects
{
    using DataStructures;
    using Tags = Utility.SpecialEffectsTags;
    
    [RequireComponent(typeof(Rigidbody))]
    public class BasicRigidObject : PooledObject 
    {
        public Rigidbody rigidbody { get; private set; }
        
        private MeshRenderer[] meshRenderers;
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
        }
        
        private void OnTriggerEnter(Collider otherCollider)
        {
            if(otherCollider.CompareTag(Tags.killZone))
            {
                ReturnToPool();
            }
        }
        
        public void SetMaterial(Material material)
        {
            for(int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = material;
            }
        }
    }
}