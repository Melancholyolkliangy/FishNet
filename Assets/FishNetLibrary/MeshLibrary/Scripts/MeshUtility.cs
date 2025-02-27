using System;
using UnityEngine;

namespace FishNetLibrary.MeshLibrary.Scripts
{
	public static class MeshUtility
	{
		private static readonly Vector3[] CubeVertex = new Vector3[]
		{
			new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f),new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, -0.5f), 
			new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), 
			new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f),
		};
		public static void CreateSubdividedCube(Mesh cube, int subdivisions)
		{
			int vertexCount = subdivisions + 1; // 每条边的顶点数
			int faceVertexCount = vertexCount * vertexCount; // 每个面的顶点数
			Span<Vector3> vertices = stackalloc Vector3[6 * vertexCount * vertexCount];
			Span<Vector2> uvs = stackalloc Vector2[vertices.Length];
			Span<int> triangles = stackalloc int[36 * subdivisions * subdivisions];
			// 细分每个面
			for (int face = 0; face < 6; face++)
			{
				int startIndex = face * 4; // 每个面 4 个顶点
				Vector3 v0 = CubeVertex[startIndex];
				Vector3 v1 = CubeVertex[startIndex + 1];
				Vector3 v2 = CubeVertex[startIndex + 2];
				Vector3 v3 = CubeVertex[startIndex + 3];
				
				// 生成细分后的顶点
				for (int y = 0; y < vertexCount; y++)
				{
					for (int x = 0; x < vertexCount; x++)
					{
						float u = (float)x / subdivisions;
						float v = (float)y / subdivisions;

						// 插值计算顶点位置
						Vector3 top = Vector3.Lerp(v2, v3, u);
						Vector3 bottom = Vector3.Lerp(v0, v1, u);
						Vector3 vertex = Vector3.Lerp(bottom, top, v);
						
						int vertexIndex = face * faceVertexCount + y * vertexCount + x;
						vertices[vertexIndex] = (vertex);
						uvs[vertexIndex] = new Vector2(u, v);
					}
				}

				// 生成细分后的三角形
				int filledTriangles = face * 6 * subdivisions * subdivisions;
				for (int y = 0; y < subdivisions; y++)
				{
					for (int x = 0; x < subdivisions; x++)
					{
						int i = face * faceVertexCount + y * vertexCount + x;
						int triangleIndex = 6 * (y * subdivisions + x) + filledTriangles;
						// 第一个三角形
						triangles[triangleIndex] = (i);
						triangles[triangleIndex + 1] = (i + vertexCount);
						triangles[triangleIndex + 2] = (i + 1);

						// 第二个三角形
						triangles[triangleIndex + 3] = (i + 1);
						triangles[triangleIndex + 4] = (i + vertexCount);
						triangles[triangleIndex + 5] = (i + vertexCount + 1);
					}
				}
			}
			
			cube.vertices = vertices.ToArray();
			cube.triangles = triangles.ToArray();
			cube.uv = uvs.ToArray();
			cube.RecalculateNormals();
		}
	}
}