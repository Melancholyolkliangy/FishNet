using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FishNetLibrary.MeshLibrary.Scripts
{
	public class CylinderLineRenderer : MonoBehaviour
	{
		public MeshFilter meshFilter;
		public float radius;
		[Range(0,90)]
		public float cornerVertices;
		public List<Vector3> positions;
		public int numSegments = 20; // 圆柱体的细分数
		public bool useWorldSpace = false;
		
		void DrawMesh(Mesh meshRef,Vector3 top,Vector3 bottom)
		{
			var height = (top - bottom).magnitude;
			// 顶点数组
			Vector3[] vertices = new Vector3[numSegments * 2 + 2];
			// 三角形数组
			int[] triangles = new int[numSegments * 12];
			// UV坐标数组
			Vector2[] uv = new Vector2[vertices.Length];

			// 生成顶点
			for (int i = 0; i < numSegments; i++)
			{
				float angle = 2 * Mathf.PI * i / numSegments;
				float x = Mathf.Cos(angle) * radius;
				float z = Mathf.Sin(angle) * radius;

				vertices[i] = new Vector3(x, height, z);
				vertices[i + numSegments] = new Vector3(x, 0, z);

				uv[i] = new Vector2((float)i / numSegments, 1);
				uv[i + numSegments] = new Vector2((float)i / numSegments, 0);
			}

			vertices[numSegments * 2] = new Vector3(0, height, 0); // 顶部中心点
			vertices[numSegments * 2 + 1] = new Vector3(0, 0, 0); // 底部中心点
			
			for (int i = 0; i < numSegments; i++)
			{	
				// 生成上下三角形
				int next = (i + 1) % numSegments;
				triangles[i * 6] = i;
				triangles[i * 6 + 1] = numSegments * 2;
				triangles[i * 6 + 2] = next;
			
				triangles[i * 6 + 3] = i + numSegments;
				triangles[i * 6 + 4] = next + numSegments;
				triangles[i * 6 + 5] = numSegments * 2 + 1;
				
				// 生成侧面三角形
				int triangleIndex = i * 6 + 6 * numSegments;
				triangles[triangleIndex] = i;
				triangles[triangleIndex + 1] = next;
				triangles[triangleIndex + 2] = i + numSegments;
				
				triangles[triangleIndex + 3] = next;
				triangles[triangleIndex + 4] = next + numSegments;
				triangles[triangleIndex + 5] = i + numSegments;
			}
			//从圆柱空间转换到模型空间
			var mTrans = Matrix4x4.TRS(bottom,
				Quaternion.FromToRotation(Vector3.up,(top - bottom).normalized),
				Vector3.one);
			if (useWorldSpace)
			{
				mTrans = _parentMMatrix.inverse * mTrans;
			}
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = mTrans * vertices[i].ToHomogeneousCoordinatesVector();
			}

			for (int i = 0; i < triangles.Length; i++)
			{
				triangles[i] += meshRef.vertexCount;
			}
			int preVertexCount = meshRef.vertexCount;
			meshRef.vertices = meshRef.vertices.Concat(vertices).ToArray();
			meshRef.triangles = meshRef.triangles.Concat(triangles).ToArray();
			if (meshRef.uv.Length == 0)
			{
				meshRef.uv = uv;
			}
			else
			{
				for (int i = 0; i < uv.Length; i++)
				{
					meshRef.uv[i + preVertexCount] = uv[i];
				}
			}
		}
		Matrix4x4 _parentMMatrix;
		[Button("Generate Cylinder Line")]
		public void DrawAllMesh()
		{
			if (positions is not { Count: > 1 })
			{
				throw new Exception("至少要有两个点才能确定一个线段");
			}
			_parentMMatrix = Matrix4x4.TRS(transform.position,transform.rotation,Vector3.one);
			Mesh meshRef = new Mesh();
			meshRef.vertices = Array.Empty<Vector3>();
			meshRef.triangles = Array.Empty<int>();
			meshRef.uv = Array.Empty<Vector2>();
			for (int i = 0; i < positions.Count - 1; i++)
			{
				var bottom = positions[i];
				var top = positions[i + 1];
				DrawMesh(meshRef,top, bottom);
			}
			meshRef.RecalculateNormals();
			meshRef.RecalculateBounds();
			
			meshFilter.mesh = meshRef;
		}
		public void SetPositions(List<Vector3> positions)
		{
			this.positions = positions;
			DrawAllMesh();
		}
	}
}