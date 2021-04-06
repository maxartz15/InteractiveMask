using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace TAO.InteractiveMask
{
	[RequireComponent(typeof(MaskRenderer))]
	public class MaskCamera : MonoBehaviour
	{
		public Mode mode = Mode.Auto;

		[SerializeField]
		private Camera maskCamera = null;
		private MaskRenderer maskRenderer = null;

		[SerializeField]
		private int resolution = 512;
		[SerializeField]
		private RenderTextureFormat format = RenderTextureFormat.Default;
		public RenderTexture Target
		{
			get; private set;
		}
		private RenderTexture source = null;

		[SerializeField]
		private string maskTextureGlobalParameterName = "_TAO_Mask";
		[SerializeField]
		private string maskDataGlobalParameterName = "_TAO_MaskData";

		public bool debugGui = false;

		private void Awake()
		{
			Target = CreateRenderTexture(resolution, format);
			source = new RenderTexture(Target);

			maskCamera.targetTexture = source;

			switch (mode)
			{
				case Mode.Auto:
					{
						maskCamera.enabled = true;
					}
					break;
				case Mode.Manual:
					{
						maskCamera.enabled = false;
					}
					break;
				default:
					break;
			}

			maskRenderer = GetComponent<MaskRenderer>();
			maskRenderer.target = Target;
			maskRenderer.source = source;
			maskRenderer.Init();

			// Set texture.
			Shader.SetGlobalTexture(maskTextureGlobalParameterName, Target);
		}

		private void OnDestroy()
		{
			Target.Release();
			source.Release();
			maskRenderer.Release();
		}

		private void LateUpdate()
		{
			switch (mode)
			{
				case Mode.Auto:
					{
						Render();
					}
					break;
				case Mode.Manual:
					break;
				default:
					break;
			}
		}

		public void Render()
		{
			SnapCameraPosition();

			switch (mode)
			{
				case Mode.Auto:
					break;
				case Mode.Manual:
					{
						maskCamera.Render();
					}
					break;
				default:
					break;
			}

			maskRenderer.Render();

			// Set data.
			Shader.SetGlobalVector(maskDataGlobalParameterName, new Vector4(maskCamera.transform.position.x, maskCamera.transform.position.z, maskCamera.orthographicSize, 0));
		}

		private void SnapCameraPosition()
		{
			float pws = (1.0f / resolution) * maskCamera.orthographicSize;

			// Snap position to pixel.
			Vector3 newPos = Vector3.zero;
			newPos.x = (Mathf.Floor(maskCamera.transform.position.x / pws) + 0.5f) * pws;
			newPos.y = maskCamera.transform.position.y;
			newPos.z = (Mathf.Floor(maskCamera.transform.position.z / pws) + 0.5f) * pws;
			maskCamera.transform.position = newPos;
		}

		public static RenderTexture CreateRenderTexture(int resolution, RenderTextureFormat format)
		{
			var rt = new RenderTexture(resolution, resolution, 0, format, 0);
			rt.Create();

			return rt;
		}

		private void OnGUI()
		{
			if (debugGui)
			{
				using (new GUILayout.VerticalScope())
				{
					using (new GUILayout.HorizontalScope())
					{
						if (GUILayout.Button("Render"))
						{
							Render();
						}

						if (GUILayout.Button("MaskRenderer.Clear"))
						{
							maskRenderer.Clear();
						}
					}
				}

				maskRenderer.GUI();
			}
		}

		public enum Mode
		{
			Auto,
			Manual
		}
	}
}