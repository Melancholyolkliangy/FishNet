using System;
using UnityEngine;

namespace FishNetLibrary.ShaderLibrary.Scripts.PostEffect
{
	public class BrightnessSaturationAndContrast : PostEffectsBase
	{
		public Shader briSatConShader;
		private Material briSatConMaterial;

		public float brightness;
		public float saturation;
		public float contrast;
		
		public Material Material
		{
			get
			{
				briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader, briSatConMaterial);
				return briSatConMaterial; 
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (Material != null)
			{
				Material.SetFloat("_Brightness",brightness);
				Material.SetFloat("_Saturation",saturation);
				Material.SetFloat("_Contrast",contrast);
				Graphics.Blit(source,destination,Material);
			}
			else
			{
				Graphics.Blit(source,destination);
			}
		}
	}
}