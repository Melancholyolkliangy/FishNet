using UnityEngine;

namespace FishNetLibrary.MeshLibrary.Scripts
{
	public static class VectorUtility
	{
		public static Vector4 ToHomogeneousCoordinatesVector(this Vector3 vector)
		{
			return new Vector4(vector.x, vector.y, vector.z, 1);
		}
	}
}