using UnityEngine;

namespace Utility.Testing
{
    public abstract class Transformation : MonoBehaviour, System.IComparable<Transformation>
    {
        public abstract Matrix4x4 matrix { get; }
        public abstract int transformationOrderRank { get; }

        public Vector3 Apply(Vector3 point)
        {
            return matrix.MultiplyPoint(point);
        }

        public int CompareTo(Transformation otherTransformation)
        {
            return transformationOrderRank - otherTransformation.transformationOrderRank;
        }
    }

    [System.Serializable]
    public enum CameraProjection { XY, XZ, YZ };
}
