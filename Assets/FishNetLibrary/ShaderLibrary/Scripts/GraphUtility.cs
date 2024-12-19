using UnityEngine;

namespace FishNetLibrary.ShaderLibrary.Scripts
{
	public class GraphUtility
	{
		public static Mesh SubdivideCube(Mesh mesh, int n)
		{
			Vector3[] oldVertices = mesh.vertices;
			int[] oldTriangles = mesh.triangles;
			Vector3[] newVertices = new Vector3[oldVertices.Length * n * n];
			int[] newTriangles = new int[oldTriangles.Length * n * n];

			int vertsPerSide = n + 1;
			int vertexIndex = 0;
			int triangleIndex = 0;

			for (int i = 0; i < oldTriangles.Length; i += 6)
			{
				// Get the corner vertices of the face
				Vector3 v0 = oldVertices[oldTriangles[i]];
				Vector3 v1 = oldVertices[oldTriangles[i + 1]];
				Vector3 v2 = oldVertices[oldTriangles[i + 2]];
				Vector3 v3 = oldVertices[oldTriangles[i + 5]];

				for (int y = 0; y < n; y++)
				{
					for (int x = 0; x < n; x++)
					{
						// Calculate new vertices for a small quad
						Vector3 v00 = Vector3.Lerp(Vector3.Lerp(v0, v3, (float)y / n),
							Vector3.Lerp(v1, v2, (float)y / n), (float)x / n);
						Vector3 v01 = Vector3.Lerp(Vector3.Lerp(v0, v3, (float)(y + 1) / n),
							Vector3.Lerp(v1, v2, (float)(y + 1) / n), (float)x / n);
						Vector3 v10 = Vector3.Lerp(Vector3.Lerp(v0, v3, (float)y / n),
							Vector3.Lerp(v1, v2, (float)y / n), (float)(x + 1) / n);
						Vector3 v11 = Vector3.Lerp(Vector3.Lerp(v0, v3, (float)(y + 1) / n),
							Vector3.Lerp(v1, v2, (float)(y + 1) / n), (float)(x + 1) / n);

						newVertices[vertexIndex] = v00;
						newVertices[vertexIndex + 1] = v01;
						newVertices[vertexIndex + 2] = v10;
						newVertices[vertexIndex + 3] = v11;

						// Define two triangles
						newTriangles[triangleIndex] = vertexIndex;
						newTriangles[triangleIndex + 1] = vertexIndex + 1;
						newTriangles[triangleIndex + 2] = vertexIndex + 2;
						newTriangles[triangleIndex + 3] = vertexIndex + 2;
						newTriangles[triangleIndex + 4] = vertexIndex + 1;
						newTriangles[triangleIndex + 5] = vertexIndex + 3;

						vertexIndex += 4;
						triangleIndex += 6;
					}
				}
			}

			mesh.vertices = newVertices;
			mesh.triangles = newTriangles;
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}