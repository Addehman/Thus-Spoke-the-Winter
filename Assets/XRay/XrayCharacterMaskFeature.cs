using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class XrayCharacterMaskFeature : ScriptableRendererFeature
{
	[SerializeField] private LayerMask _characterLayer;

	private XrayCharacterMaskPass _pass;

	public override void Create()
	{
		_pass = new XrayCharacterMaskPass
		{
			renderPassEvent = RenderPassEvent.BeforeRenderingTransparents,
			_characterLayer = _characterLayer,
		};
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(_pass);
	}


	private class XrayCharacterMaskPass : ScriptableRenderPass
	{
		private static readonly List<ShaderTagId> _shaderTagIds = new List<ShaderTagId>
		{
			new ShaderTagId("UniversalForward"),
			new ShaderTagId("SRPDefaultUnlit"),
		};

		public LayerMask _characterLayer;

		private ProfilingSampler _profilingSampler = new ProfilingSampler("Xray Thing");

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (XrayManager.Instance == null || XrayManager.Instance.CharacterMaskTexture == null)
				return;

			CommandBuffer cmd = CommandBufferPool.Get();

			using (new ProfilingScope(cmd, _profilingSampler))
			{
				cmd.SetRenderTarget(XrayManager.Instance.CharacterMaskTexture);
				cmd.ClearRenderTarget(false, true, Color.clear);
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();

				DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, SortingCriteria.SortingLayer);
				FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.transparent, _characterLayer);
				context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

				cmd.SetRenderTarget(renderingData.cameraData.renderer.cameraColorTarget);
				context.ExecuteCommandBuffer(cmd);
			}

			CommandBufferPool.Release(cmd);
		}
	}
}
