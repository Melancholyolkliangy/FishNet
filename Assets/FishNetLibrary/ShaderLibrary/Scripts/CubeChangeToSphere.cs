using FishNetLibrary.ShaderLibrary.Scripts.Buffer;
using UnityEngine;

namespace FishNetLibrary.ShaderLibrary.Scripts
{
	public class CubeChangeToSphere : MonoBehaviour
	{
		public string kernelName;
		public ComputeShader shader;
		public float radius;
		private Vertex[] _originalVertex;
		private Vertex[] _currentVertex;
		
		private ComputeBuffer _originVertexBuffer;
		private ComputeBuffer _currentVertexBuffer;
		
		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;

		private int _kernelHandle;
		void Start()
		{
			// 获取当前对象的渲染器组件
			_meshRenderer = GetComponent<MeshRenderer>();
			_meshFilter = GetComponent<MeshFilter>();
			// 启用渲染器
			_meshRenderer.enabled = true;

			InitShader();
		}
		private void InitShader()
		{
			// 查找计算着色器内核的句柄
			_kernelHandle = shader.FindKernel(kernelName);
			//获得所有顶点
			GraphUtility.SubdivideCube(_meshFilter.mesh, 10);	//细分顶点
			int vertexCount = _meshFilter.mesh.vertices.Length;
			_originalVertex = new Vertex[vertexCount];
			_currentVertex = new Vertex[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				_originalVertex[i] = new Vertex(_meshFilter.mesh.vertices[i], _meshFilter.mesh.normals[i]);
			}

			_originVertexBuffer = new ComputeBuffer(vertexCount, 3 * 4 * 2);
			_originVertexBuffer.SetData(_originalVertex);
			shader.SetBuffer(_kernelHandle,"initialBuffer",_originVertexBuffer);
			_currentVertexBuffer = new ComputeBuffer(vertexCount, 3 * 4 * 2);
			shader.SetBuffer(_kernelHandle,"vertexBuffer",_currentVertexBuffer);
		}

		private void Update()
		{
			shader.SetFloat("radius",radius);
			float delta = (Mathf.Sin(Time.time) + 1) / 2;
			shader.SetFloat("delta",delta);
			shader.Dispatch(_kernelHandle,_originalVertex.Length / 8,1,1);
			_currentVertexBuffer.GetData(_currentVertex);
			_meshFilter.mesh.vertices = _currentVertex.PositionToVectorArray();
			_meshFilter.mesh.normals = _currentVertex.NormalToVectorArray();
		}
	}
}