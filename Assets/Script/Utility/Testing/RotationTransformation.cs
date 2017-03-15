/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/rendering/part-1 ---
 */

using UnityEngine;

namespace Utility.Testing
{
    public class RotationTransformation : Transformation
    {
        public Vector3 rotation;

        public override int transformationOrderRank { get { return 2; } }

        public override Matrix4x4 matrix
        {
            get
            {
                float radianX = rotation.x * Mathf.Deg2Rad;
                float radianY = rotation.y * Mathf.Deg2Rad;
                float radianZ = rotation.z * Mathf.Deg2Rad;
                float sineX = Mathf.Sin(radianX);
                float sineY = Mathf.Sin(radianY);
                float sineZ = Mathf.Sin(radianZ);
                float cosineX = Mathf.Cos(radianX);
                float cosineY = Mathf.Cos(radianY);
                float cosineZ = Mathf.Cos(radianZ);
                Matrix4x4 newMatrix = new Matrix4x4();

                newMatrix.SetColumn(0, new Vector4(cosineY * cosineZ, 
                    cosineX * sineZ + sineZ * sineY * cosineZ, 
                    sineX * sineZ - cosineX * sineY * cosineZ, 0f));
                newMatrix.SetColumn(1, new Vector4(-cosineY * sineZ, 
                    cosineX * cosineZ - sineX * sineY * sineZ, 
                    sineX * cosineZ + cosineX * sineY * sineZ, 0f));
                newMatrix.SetColumn(2, new Vector4(sineY, -sineX * cosineY, cosineX * cosineY, 0f));
                newMatrix.SetColumn(3, new Vector4(0f, 0f, 0f, 1.0f));

                return newMatrix;
            }
        }
    }
}