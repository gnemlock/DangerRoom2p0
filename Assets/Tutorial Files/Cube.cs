using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour {

    	public int width, height, depth;

	private Mesh mesh;
	private Vector3[] vertices;

    private void Awake () {
		Generate();
	}

	private void Generate () {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Cube";
        CreateVertices();
		CreateTriangles();
	}

	private void CreateVertices () {
		int cornerVertices = 8;
		int edgeVertices = (width + height + depth - 3) * 4;
		int faceVertices = (
			(width - 1) * (height - 1) +
			(width - 1) * (depth - 1) +
			(height - 1) * (depth - 1)) * 2;
		vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

		int v = 0;
		for (int y = 0; y <= height; y++) {
			for (int x = 0; x <= width; x++) {
				vertices[v++] = new Vector3(x, y, 0);
			}
			for (int z = 1; z <= depth; z++) {
				vertices[v++] = new Vector3(width, y, z);
			}
			for (int x = width - 1; x >= 0; x--) {
				vertices[v++] = new Vector3(x, y, depth);
			}
			for (int z = depth - 1; z > 0; z--) {
				vertices[v++] = new Vector3(0, y, z);
			}
		}
		for (int z = 1; z < depth; z++) {
			for (int x = 1; x < width; x++) {
				vertices[v++] = new Vector3(x, height, z);
			}
		}
		for (int z = 1; z < depth; z++) {
			for (int x = 1; x < width; x++) {
				vertices[v++] = new Vector3(x, 0, z);
			}
		}

		mesh.vertices = vertices;
	}

	private void CreateTriangles ()
    {
        int quadCount = (width * height + width * depth + height * depth) * 2;
		int[] triangles = new int[quadCount * 6];
        int ringSize = (width + depth) * 2;
        int triangleIndex = 0, vertexIndex = 0;

		for (int y = 0; y < height; y++, vertexIndex++) {
        for (int quadIndex = 0; quadIndex < ringSize - 1; quadIndex++, vertexIndex++) {
				triangleIndex = SetQuad(triangles, triangleIndex, vertexIndex, vertexIndex + 1, vertexIndex + ringSize, vertexIndex + ringSize + 1);
			}
			triangleIndex = SetQuad(triangles, triangleIndex, vertexIndex, vertexIndex - ringSize + 1, vertexIndex + ringSize, vertexIndex + 1);
		}

		triangleIndex = CreateTopFace(triangles, triangleIndex, ringSize);
		triangleIndex = CreateBottomFace(triangles, triangleIndex, ringSize);
		mesh.triangles = triangles;
	}

	private int CreateTopFace (int[] triangles, int t, int ring) {
		int v = ring * height;
		for (int x = 0; x < width - 1; x++, v++) {
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

		int vMin = ring * (height + 1) - 1;
		int vMid = vMin + 1;
		int vMax = v + 2;

		for (int z = 1; z < depth - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + width - 1);
			for (int x = 1; x < width - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid, vMid + 1, vMid + width - 1, vMid + width);
			}
			t = SetQuad(triangles, t, vMid, vMax, vMid + width - 1, vMax + 1);
		}

		int vTop = vMin - 2;
		t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
		for (int x = 1; x < width - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
		}
		t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

		return t;
	}
 
	private int CreateBottomFace (int[] triangles, int t, int ring) {
		int v = 1;
        int vMid = vertices.Length - (width - 1) * (depth - 1);
        int test = t;
		t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);

		for (int x = 1; x < width - 1; x++, v++, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

		int vMin = ring - 2;
		vMid -= width - 2;
		int vMax = v + 2;

		for (int z = 1; z < depth - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid + width - 1, vMin + 1, vMid);
			for (int x = 1; x < width - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid + width - 1, vMid + width, vMid, vMid + 1);
			}
			t = SetQuad(triangles, t, vMid + width - 1, vMax + 1, vMid, vMax);
		}

		int vTop = vMin - 1;
		t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
		for (int x = 1; x < width - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
		}
		t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

		return t;
	}

	private static int
	SetQuad (int[] triangles, int index, int bottomLeftIndex, int bottomRightIndex, int topLeftIndex, int topRightIndex) 
    {
		triangles[index] = bottomLeftIndex;
		triangles[index + 1] = triangles[index + 4] = topLeftIndex;
		triangles[index + 2] = triangles[index + 3] = bottomRightIndex;
        triangles[index + 5] = topRightIndex;
		return index + 6;
	}

	private void OnDrawGizmos () {
		if (vertices == null) {
			return;
		}
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawSphere(vertices[i], 0.1f);
		}
	}
}