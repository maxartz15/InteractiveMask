using UnityEngine;

namespace TAO.InteractiveMask
{
	public class MaskRenderer : MonoBehaviour
	{
		public Shader clearBlit = null;
		private Material clearBlitMaterial;
		// Camera source.
		[HideInInspector]
		public RenderTexture source = null;
		// Output target.
		[HideInInspector]
		public RenderTexture target = null;

		public Layer[] layers;

		public bool debugGui = false;

		public void Init()
		{
			clearBlitMaterial = new Material(clearBlit);

			foreach (Layer layer in layers)
			{
				layer.Init(source, target);
			}
		}

		public void Render()
		{
			Graphics.Blit(target, target, clearBlitMaterial);

			foreach (Layer layer in layers)
			{
				layer.Blit(source, target);
			}
		}

		// Clears the context of the render targets.
		public void Clear()
		{
			Graphics.Blit(target, target, clearBlitMaterial);

			foreach (Layer layer in layers)
			{
				layer.Clear(clearBlitMaterial);
			}
		}

		private void OnValidate()
		{
			if (clearBlit == null)
			{
				clearBlit = Shader.Find("TAO/InteractiveMask/Clear");
			}

			if (layers != null)
			{
				for (int i = 0; i < layers.Length; i++)
				{
					if (layers[i].blit == null)
					{
						layers[i].blit = Shader.Find("TAO/InteractiveMask/Add");
					}

					if (layers[i].persistentBlit == null)
					{
						layers[i].persistentBlit = Shader.Find("TAO/InteractiveMask/AddPersistent");
					}
				}
			}
		}

		public void GUI()
		{
			using (new GUILayout.HorizontalScope())
			{
				GUITexture(source);
				GUITexture(target);

				foreach (var l in layers)
				{
					switch (l.type)
					{
						case Layer.Type.Clear:
							break;
						case Layer.Type.Persistent:
							{
								GUITexture(l.PersistentTarget);
							}
							break;
						default:
							break;
					}
				}
			}
		}

		private void GUITexture(Texture texture)
		{
			using (new GUILayout.VerticalScope())
			{
				GUILayout.Label(texture, GUILayout.Width(256), GUILayout.Height(256));
				GUILayout.Label(string.Format("{0}\n{1}x{2}\n{3}", texture.name, texture.width, texture.height, texture.graphicsFormat));
			}
		}
	}

	[System.Serializable]
	public class Layer
	{
		public static int LayerCount = 0;

		public Type type = Type.Clear;
		public Mask mask = new Mask();
		public Shader blit = null;
		private Material blitMaterial = null;

		// Only when mode is persistent.
		public Shader persistentBlit;
		public RenderTexture PersistentTarget
		{
			get; private set;
		}
		private Material persistentBlitMaterial;

		public void Init(RenderTexture source, RenderTexture target)
		{
			blitMaterial = new Material(blit);

			switch (type)
			{
				case Type.Clear:
					break;
				case Type.Persistent:
					{
						persistentBlitMaterial = new Material(persistentBlit);

						if (PersistentTarget == null)
						{
							PersistentTarget = new RenderTexture(target)
							{
								name = LayerCount.ToString()
							};
						}

						persistentBlitMaterial.SetTexture("_Source", source);
						persistentBlitMaterial.SetTexture("_Persistent", PersistentTarget);
					}
					break;
				default:
					break;
			}

			blitMaterial.SetTexture("_Source", source);
			blitMaterial.SetVector("_Channels", mask.GetMaskVector);

			LayerCount++;
		}

		public void Blit(RenderTexture source, RenderTexture target)
		{
			switch (type)
			{
				case Type.Clear:
					{
						Graphics.Blit(source, target, blitMaterial);
					}
					break;
				case Type.Persistent:
					{
						Graphics.Blit(source, PersistentTarget, persistentBlitMaterial);
						Graphics.Blit(PersistentTarget, target, blitMaterial);
					}
					break;
				default:
					break;
			}
		}

		public void Clear(Material clearBlit)
		{
			if (PersistentTarget)
			{
				Graphics.Blit(PersistentTarget, PersistentTarget, clearBlit);
			}
		}

		public enum Type
		{
			Clear,
			Persistent
		}
	}

	[System.Serializable]
	public struct Mask
	{
		public bool r;
		public bool g;
		public bool b;
		public bool a;

		public Vector4 GetMaskVector
		{
			get { return new Vector4(r ? 1 : 0, g ? 1 : 0, b ? 1 : 0, a ? 1 : 0); }
		}
	}
}