using System;
using UnityEngine;

namespace FishNetLibrary.ShaderLibrary.Scripts.Buffer
{
	public struct Vertex
	{
		public Vector3 position;
		public Vector3 normal;

		public Vertex(Vector3 position, Vector3 normal)
		{
			this.position = position;
			this.normal = normal;
		}
	}

	public static class VertexExtensions
	{
		public static Vector3[] PositionToVectorArray(this Vertex[] source)
		{
			Vector3[] v = new Vector3[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				v[i] = source[i].position;
			}

			return v;
		}
		public static Vector3[] NormalToVectorArray(this Vertex[] source)
		{
			Vector3[] v = new Vector3[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				v[i] = source[i].normal;
			}

			return v;
		}
	}
}