/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/rendering/part-1 ---
 */

using UnityEngine;

namespace Utility.Testing
{
    public class ScaleTransformation : Transformation
    {
        public Vector3 scale = Vector3.one;

        public override int transformationOrderRank { get { return 3; } }

        public override Matrix4x4 matrix
        {
            get
            {
                Matrix4x4 newMatrix = new Matrix4x4();

                newMatrix.SetRow(0, new Vector4(scale.x, 0f, 0f, 0f));
                newMatrix.SetRow(1, new Vector4(0f, scale.y, 0f, 0f));
                newMatrix.SetRow(2, new Vector4(0f, 0f, scale.z, 0f));
                newMatrix.SetRow(3, new Vector4(0f, 0f, 0f, 1.0f));

                return newMatrix;
            }
        }
    }
}