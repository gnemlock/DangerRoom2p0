/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/rendering/part-1 ---
 */

using UnityEngine;

namespace Utility.Testing
{
    public class PerspectiveTransformation : Transformation 
    {
        public CameraProjection cameraProjection;
        public float focalLength = 1.0f;

        public override int transformationOrderRank { get { return 4; } }

        public override Matrix4x4 matrix
        {
            get
            {
                float xScale = (cameraProjection == CameraProjection.YZ ? 1.0f : 0f);
                float yScale = (cameraProjection == CameraProjection.XZ ? 1.0f : 0f);
                float zScale = (cameraProjection == CameraProjection.XY ? 1.0f : 0f);
                float xZoom = (cameraProjection == CameraProjection.YZ ? 0f : focalLength);
                float yZoom = (cameraProjection == CameraProjection.XZ ? 0f : focalLength);
                float zZoom = (cameraProjection == CameraProjection.XY ? 0f : focalLength);
                Matrix4x4 newMatrix = new Matrix4x4();

                newMatrix.SetRow(0, new Vector4(xZoom, 0f, 0f, 0f));
                newMatrix.SetRow(1, new Vector4(0f, yZoom, 0f, 0f));
                newMatrix.SetRow(2, new Vector4(0f, 0f, zZoom, 0f));
                newMatrix.SetRow(3, new Vector4(xScale, yScale, zScale, 0f));

                return newMatrix;
            }
        }
    }
}