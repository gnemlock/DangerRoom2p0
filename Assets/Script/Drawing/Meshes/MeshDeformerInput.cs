using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drawing.Meshes
{
    public class MeshDeformerInput : MonoBehaviour
    {
        public float inputForce = 10.0f;
        public float forceOffset = 0.1f;

        private void Update()
        {
            if(Input.GetMouseButton(0))
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            Ray inputProjection = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if(Physics.Raycast(inputProjection, out raycastHit))
            {
                MeshDeformer meshDeformer = raycastHit.collider.GetComponent<MeshDeformer>();

                if(meshDeformer != null)
                {
                    Vector3 forcePosition = raycastHit.point;
                    forcePosition += raycastHit.normal * forceOffset;
                    meshDeformer.ApplyDeformingForce(forcePosition, inputForce);
                }
            }
        }
    }
}