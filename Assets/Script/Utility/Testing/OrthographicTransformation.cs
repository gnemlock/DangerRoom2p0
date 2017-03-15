/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/rendering/part-1 ---
 */

using UnityEngine;

namespace Utility.Testing
{
    public class OrthographicTransformation : Transformation
    {
        public CameraProjection cameraProjection;

        public override int transformationOrderRank { get { return 4; } }

        public override Matrix4x4 matrix
        {
            get
            {
                float x = (cameraProjection == CameraProjection.YZ ? 0f : 1.0f);
                float y = (cameraProjection == CameraProjection.XZ ? 0f : 1.0f);
                float z = (cameraProjection == CameraProjection.XY ? 0f : 1.0f);
                Matrix4x4 newMatrix = new Matrix4x4();

                newMatrix.SetRow(0, new Vector4(x, 0f, 0f, 0f));
                newMatrix.SetRow(1, new Vector4(0f, y, 0f, 0f));
                newMatrix.SetRow(2, new Vector4(0f, 0f, z, 0f));
                newMatrix.SetRow(3, new Vector4(0f, 0f, 0f, 1.0f));

                return newMatrix;
            }
        }
    }
}