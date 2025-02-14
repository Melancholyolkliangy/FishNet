using UnityEngine;

namespace FishNetLibrary.ShaderLibrary.Scripts
{
	[ExecuteInEditMode]
	public class ProceduralTextureMat : MonoBehaviour
	{
		public Material material;
		[SerializeField,SetProperty("textureWidth")]
		private int m_TextureWidth;

		public int textureWidth
		{
			get { return m_TextureWidth; }
			set { m_TextureWidth = value; }
		}

		void UpdateMaterial()
		{
			
		}
	}
}