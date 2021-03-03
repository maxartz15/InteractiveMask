using System.Collections.Generic;
using UnityEngine;

namespace TAO.InteractiveMask
{
	[RequireComponent(typeof(MaskRenderer))]
	public class MaskCamera : MonoBehaviour
	{
		public Mode mode = Mode.EachFrame;

		[SerializeField]
		private Camera maskCamera = null;
		private MaskRenderer maskRenderer = null;

		public RenderTexture target = null;
		private RenderTexture source = null;

		public List<Material> materials = new List<Material>();

		private int targetWidth = 0;

		private void Awake()
		{
			maskRenderer = GetComponent<MaskRenderer>();

			source = new RenderTexture(target);

			maskCamera.targetTexture = source;

			maskRenderer.target = target;
			maskRenderer.source = source;

			targetWidth = target.width;

			maskRenderer.Init();
		}

		private void Update()
		{
			if (mode == Mode.EachFrame)
			{
				Render();
			}
		}

		public void Render()
		{
			SnapCameraPosition();

			maskCamera.Render();
			maskRenderer.Render();

			foreach (var m in materials)
			{
				m.SetVector("_MaskData", new Vector4(maskCamera.transform.position.x, maskCamera.transform.position.z, maskCamera.orthographicSize, 0));
			}
		}

		private void SnapCameraPosition()
		{
			float pws = (1.0f / targetWidth) * maskCamera.orthographicSize;

			// Snap position to pixel.
			Vector3 newPos = Vector3.zero;
			newPos.x = (Mathf.Floor(maskCamera.transform.position.x / pws) + 0.5f) * pws;
			newPos.y = maskCamera.transform.position.y;
			newPos.z = (Mathf.Floor(maskCamera.transform.position.z / pws) + 0.5f) * pws;
			maskCamera.transform.position = newPos;
		}

		public enum Mode
		{
			EachFrame,
			Manual
		}
	}
}