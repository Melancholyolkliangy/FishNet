using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FishNetLibrary.MeshLibrary.Scripts
{
	public class CubeRenderer : MonoBehaviour
	{
		public MeshFilter cubeMeshFilter;
		public int subLevel;
		[Button("细分网格")]
		public void SubDivideCube()
		{
			MeshUtility.CreateSubdividedCube(cubeMeshFilter.mesh, subLevel);
		}
	}
}