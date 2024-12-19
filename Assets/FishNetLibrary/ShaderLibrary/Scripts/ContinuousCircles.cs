using FishNetLibrary.ShaderLibrary.Scripts.Buffer;
using UnityEngine;

namespace FishNetLibrary.ShaderLibrary.Scripts
{
	public class ContinuousCircles : MonoBehaviour
	{
		public string clearKernelName;
		public string circleKernelName;
		// ComputeShader 用于在 GPU 上执行计算任务
		public ComputeShader shader;
		// 纹理分辨率
		public int texResolution = 256;
		public Color clearColor;
		public Color circleColor;

		public int groupCount;
		// 渲染器组件
		private Renderer _rend;
		// 渲染纹理
		private RenderTexture _outputTexture;
		// 计算着色器内核句柄
		private int _clearHandle;
		private int _circleHandle;
		private uint _threadGroupSizeX;
		private Circle[] _circleDatas;
		private ComputeBuffer _circleDataBuffer;
		private ComputeBuffer _resultBuffer;

		private Vector3[] _output;
		// Start is called before the first frame update
		void Start()
		{
			// 创建一个新的渲染纹理，指定宽度、高度和位深度（此处位深度为0）
			_outputTexture = new RenderTexture(texResolution, texResolution, 0);
			// 允许随机写入
			_outputTexture.enableRandomWrite = true;
			// 创建渲染纹理实例
			_outputTexture.Create();

			// 获取当前对象的渲染器组件
			_rend = GetComponent<Renderer>();
			// 启用渲染器
			_rend.enabled = true;

			InitShader();
		}
		private void InitShader()
		{
			// 查找计算着色器内核 "CSMain" 的句柄
			_clearHandle = shader.FindKernel(clearKernelName);
			_circleHandle = shader.FindKernel(circleKernelName);
			shader.GetKernelThreadGroupSizes(_circleHandle,out _threadGroupSizeX,out _,out _);
			//创建圆形数据
			var total = _threadGroupSizeX * groupCount;
			_circleDatas = new Circle[total];
			float speed = 100;
			float halfSpeed = speed * 0.5f;
			float minRadius = 10.0f;
			float maxRadius = 30.0f;
			float radiusRange = maxRadius - minRadius;
			for(int i=0; i<total; i++)
			{
				Circle circle = _circleDatas[i];
				circle.origin.X = Random.value * texResolution;
				circle.origin.Y = Random.value * texResolution;
				circle.velocity.X = (Random.value * speed) - halfSpeed;
				circle.velocity.Y = (Random.value * speed) - halfSpeed;
				circle.radius = Random.value * radiusRange + minRadius;
				_circleDatas[i] = circle;
			}

			int stride = (2 + 2 + 1) * 4;
			_circleDataBuffer = new ComputeBuffer(_circleDatas.Length, stride);
			_circleDataBuffer.SetData(_circleDatas);
			shader.SetBuffer(_circleHandle,"circlesBuffer",_circleDataBuffer);

			_resultBuffer = new ComputeBuffer(_circleDatas.Length, 3 * 4);
			shader.SetBuffer(_circleHandle,"resultBuffer",_resultBuffer);
			_output = new Vector3[_circleDatas.Length];
			// 设置计算着色器中使用的纹理
			shader.SetInt("texResolution",texResolution);
			shader.SetVector("clearColor",clearColor);
			shader.SetVector("circleColor",circleColor);
			shader.SetTexture(_clearHandle, "Result", _outputTexture);
			shader.SetTexture(_circleHandle, "Result", _outputTexture);
			
			// 将渲染纹理设置为材质的主纹理
			_rend.material.SetTexture("_MainTex", _outputTexture);

			
			// shader.Dispatch(_clearHandle, texResolution/8, texResolution/8, 1);
			// shader.Dispatch(_circleHandle, groupCount, 1, 1);
			// 调度计算着色器的执行，传入计算组的大小
			// 这里假设每个工作组是 16x16
			// 简单的说就是，要分配多少个组，才能完成计算，目前只分了xy的各一半，因此只渲染了1/4的画面。
			// DispatchShader(texResolution / 8, texResolution / 8);
		}
		private void DispatchShader(int x, int y)
		{
			// 调度计算着色器的执行
			// x 和 y 表示计算组的数量，1 表示 z 方向上的计算组数量（这里只有一个）
			shader.Dispatch(_clearHandle, x, y, 1);
		}

		private void DispatchShader(int x)
		{
			shader.SetFloat("time",Time.time);
			shader.Dispatch(_circleHandle, x, 1, 1);
			_resultBuffer.GetData(_output);
			Debug.Log(_output[0]);
		}
		void Update()
		{
			DispatchShader(texResolution/8, texResolution/8);
			DispatchShader(groupCount);
		}
	}
}