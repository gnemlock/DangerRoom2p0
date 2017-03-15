/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/rendering/part-1 ---
 */

using UnityEngine;

namespace Utility.Testing
{
    public class PositionTransformation : Transformation
    {
        public Vector3 position;
        
        public override int transformationOrderRank { get { return 1; } }

        public override Matrix4x4 matrix
        {
            get
            {
                Matrix4x4 newMatrix = new Matrix4x4();

                newMatrix.SetRow(0, new Vector4(1.0f, 0f, 0f, position.x));
                newMatrix.SetRow(1, new Vector4(0f, 1.0f, 0f, position.y));
                newMatrix.SetRow(2, new Vector4(0f, 0f, 1.0f, position.z));
                newMatrix.SetRow(3, new Vector4(0f, 0f, 0f, 1.0f));

                return newMatrix;
            }
        }
    }
}