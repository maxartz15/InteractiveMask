using UnityEngine;

namespace TAO.InteractiveMask
{
	[RequireComponent(typeof(MaskRenderer))]
	public class MaskCamera : MonoBehaviour
	{
		[SerializeField]
		private Camera maskCamera = null;
		private MaskRenderer maskRenderer = null;

		public RenderTexture target = null;

		private RenderTexture source = null;

		private void Awake()
		{
			maskRenderer = GetComponent<MaskRenderer>();

			source = new RenderTexture(target);

			maskCamera.targetTexture = source;

			maskRenderer.target = target;
			maskRenderer.source = source;
		}

		private void Start()
		{
			maskCamera.enabled = true;
		}
	}
}