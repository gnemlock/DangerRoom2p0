/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/rendering/part-1 ---
 */

using UnityEngine;
using System.Collections.Generic;

namespace Utility.Testing
{
    using Labels = Utility.DrawingLabels;

    public class TransformationGrid : MonoBehaviour
    {
        //TODO: Transformation filters have been set up to apply order for corect sorting. Test.
        //TODO: Check that there is only one camera transformation (perspective or orthographic)
        public Transform gridPointPrefab;
        public int gridResolution = 10;

        private Transform[] grid;
        private List<Transformation> transformations;
        private Matrix4x4 transformation;

        private void Awake()
        {
            grid = new Transform[gridResolution * gridResolution * gridResolution];
            transformations = new List<Transformation>();

            for(int i = 0, z = 0; z < gridResolution; z++)
            {
                for(int y = 0; y < gridResolution; y++)
                {
                    for(int x = 0; x < gridResolution; x++, i++)
                    {
                        grid[i] = CreateGridPoint(x, y, z);
                    }
                }
            }
        }

        private void Update()
        {
            UpdateTransformation();

            for(int i = 0, z = 0; z < gridResolution; z++)
            {
                for(int y = 0; y < gridResolution; y++)
                {
                    for(int x = 0; x < gridResolution; x++, i++)
                    {
                        grid[i].localPosition = TransformPointOnGrid(x, y, z);
                    }
                }
            }
        }

        private Transform CreateGridPoint(int x, int y, int z)
        {
            Transform newPoint = Instantiate<Transform>(gridPointPrefab);
            newPoint.localPosition = GetLocalCoordinates(x, y, z);
            newPoint.parent = transform;
            newPoint.name = string.Format(Labels.pointName, x, y, z);
            newPoint.GetComponent<MeshRenderer>().material.color = GetLocalColour(x, y, z);
            return newPoint;
        }

        private Color GetLocalColour(int x, int y, int z)
        {
            return new Color((float)x / gridResolution, (float)y / gridResolution, 
                (float)z / gridResolution);
        }

        private Vector3 GetLocalCoordinates(int x, int y, int z)
        {
            return new Vector3(x - (gridResolution - 1) * 0.5f, y - (gridResolution - 1) * 0.5f, 
                z - (gridResolution - 1) * 0.5f);
        }

        private Vector3 TransformPointOnGrid(int x, int y, int z)
        {
            Vector3 pointCoordinates = GetLocalCoordinates(x, y, z);

            return transformation.MultiplyPoint(pointCoordinates);
        }
        
        private void UpdateTransformation()
        {
            GetComponents<Transformation>(transformations);
            transformations.Sort();

            if(transformations.Count > 0)
            {
                transformation = transformations[0].matrix;

                for(int i = 1; i < transformations.Count; i++)
                {
                    transformation = transformations[i].matrix * transformation;
                }
            }
            else
            {
                transformation = Matrix4x4.identity;
            }
        }
    }
}

namespace Utility.Testing.Utility
{
    public static partial class DrawingLabels
    {
        public const string pointName = "Point ({0}, {1}, {2})";
    }
}