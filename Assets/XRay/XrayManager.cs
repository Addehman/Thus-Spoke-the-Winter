using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class XrayManager : MonoBehaviour
{
	private static readonly int _enableXrayId = Shader.PropertyToID("_EnableXray");
	private static readonly int _characterMaskTexId = Shader.PropertyToID("_CharacterMaskTex");

	[SerializeField] private bool _enableXray = true;

	private static XrayManager _instance;
	private RenderTexture _characterMaskTexture;

	public static XrayManager Instance => _instance;

	public RenderTexture CharacterMaskTexture => _characterMaskTexture;

	public bool EnableXray
	{
		get => _enableXray;
		set
		{
			_enableXray = value;
			UpdateXrayEnabled();
		}
	}

	private void OnEnable()
	{
		if (_instance != null)
		{
			Debug.LogWarning("Multiple XrayManagers in scene is not allowed!");
			SafeDestroy(this);
		}
		else
		{
			_instance = this;
			UpdateXrayEnabled();

			RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
			RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
		}
	}

	private void OnDisable()
	{
		RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
		RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;

		if (_instance == this)
		{
			Shader.SetGlobalFloat(_enableXrayId, 0f);
			_instance = null;
		}
	}

	private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
	{
		if (!_enableXray)
			return;

		Debug.Assert(_characterMaskTexture == null);

		_characterMaskTexture = RenderTexture.GetTemporary(
			cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGB32);
		Shader.SetGlobalTexture(_characterMaskTexId, _characterMaskTexture);
	}

	private void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
	{
		if (_characterMaskTexture != null)
		{
			RenderTexture.ReleaseTemporary(_characterMaskTexture);
			_characterMaskTexture = null;
		}
	}

	private void UpdateXrayEnabled()
	{
		Shader.SetGlobalFloat(_enableXrayId, _enableXray ? 1f : 0f);
	}

	private static void SafeDestroy(Object obj)
	{
		if (Application.isPlaying)
			Destroy(obj);
		else
			DestroyImmediate(obj);
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		UpdateXrayEnabled();
	}
#endif
}
