using UnityEngine;

namespace Drawing.Meshes
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshDeformer : MonoBehaviour
    {
        public float springForce = 20.0f;
        public float dampeningForce = 5.0f;

        private float averageScale = 1.0f;
        private Mesh mesh;
        private Vector3[] originalVertices;
        private Vector3[] deformedVertices;
        private Vector3[] vertexVelocities;

        private void Start()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            originalVertices = mesh.vertices;
            deformedVertices = new Vector3[originalVertices.Length];
            vertexVelocities = new Vector3[originalVertices.Length];

            for(int i = 0; i < originalVertices.Length; i++)
            {
                deformedVertices[i] = originalVertices[i];
            }
        }

        private void Update()
        {
            averageScale 
                = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 2.0f;

            for(int i = 0; i < deformedVertices.Length; i++)
            {
                UpdateVertex(i);
            }

            mesh.vertices = deformedVertices;
            mesh.RecalculateNormals();
        }

        public void ApplyDeformingForce(Vector3 forcePosition, float force)
        {
            forcePosition = transform.InverseTransformPoint(forcePosition);

            for(int i = 0; i < deformedVertices.Length; i++)
            {
                ApplyForceToVertex(i, forcePosition, force);
            }
        }
        
        private void ApplyForceToVertex(int vertexIndex, Vector3 forcePosition, float force)
        {
            Vector3 forcePositionToVertex = deformedVertices[vertexIndex] - forcePosition;
            forcePositionToVertex *= averageScale;
            float forceDegradation = force / (1.0f + forcePositionToVertex.sqrMagnitude);
            float velocity = forceDegradation * Time.deltaTime;
            vertexVelocities[vertexIndex] += forcePositionToVertex.normalized * velocity;
        }

        private void UpdateVertex(int vertexIndex)
        {
            Vector3 velocity = vertexVelocities[vertexIndex];
            Vector3 displacement = deformedVertices[vertexIndex] - originalVertices[vertexIndex];
            displacement *= averageScale;
            velocity -= displacement * springForce * Time.deltaTime;
            velocity *= 1.0f - dampeningForce * Time.deltaTime;
            vertexVelocities[vertexIndex] = velocity;
            deformedVertices[vertexIndex] += velocity * (Time.deltaTime / averageScale);
        }
    }
}