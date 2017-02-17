/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/frames-per-second/ ---
 */

using UnityEngine;

namespace Utility.Testing
{
    [RequireComponent(typeof(Rigidbody))]
    public class Nucleon : MonoBehaviour 
    {
        public float attractionForce;
        
        private Rigidbody rigidbody;
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }
        
        private void FixedUpdate()
        {
            rigidbody.AddForce(transform.localPosition * -attractionForce);
        }
    }
}