using System;
using UnityEngine;

namespace FishNetLibrary.MeshLibrary.Scripts
{
	public class JellyEffectTrigger : MonoBehaviour
	{
		public MeshFilter CubeMeshFilter;
		public float deformAmount = 0.1f; // 形变强度
		public float deformSpeed = 1.0f; // 形变速度
		public float deformDecay = 0.5f; // 形变衰减

		private Material material;
		[SerializeField]
		private float deformTime = 0f;
		[SerializeField]
		private RenderTexture depthTexture;
		public void Start()
		{
			MeshUtility.CreateSubdividedCube(CubeMeshFilter.mesh,10);
			// 获取材质
			Renderer renderer = GetComponent<Renderer>();
			if (renderer != null)
			{
				material = renderer.material;
			}
			depthTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.R8);
			Graphics.Blit(Texture2D.blackTexture, depthTexture);
		}
		void Update()
		{
			// 检测鼠标点击
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
				{
					TriggerJellyEffect();
				}
			}

			// 更新形变时间
			if (deformTime > 0)
			{
				deformTime -= Time.deltaTime;
				material.SetFloat("_DeformAmount", deformAmount);
				material.SetFloat("_DeformSpeed", deformSpeed);
				material.SetFloat("_DeformDecay", deformDecay);
			}
			else
			{
				material.SetFloat("_DeformAmount", 0);
			}
		}

		void TriggerJellyEffect()
		{
			// 重置形变时间
			deformTime = 3.0f;
		}
	}
}