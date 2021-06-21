using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderMask : ScriptableRendererFeature
{
    [System.Serializable]
    public class RenderMaskSetting
    {
        public Material MaskMat;
        
        public LayerMask MaskLayer;

        public RenderPassEvent MaskEvent = RenderPassEvent.AfterRenderingOpaques;

        public int RenderTextSize;
    }

    class RenderMaskPass : ScriptableRenderPass
    {

        private int _soildColorID = 0;

        private RenderMaskSetting _renderMaskSetting;
        private FilteringSettings _filterSetting;
        private ShaderTagId _shaderTag = new ShaderTagId("UniversalForward");
        public RenderMaskPass(RenderMaskSetting ms)
        {
            _renderMaskSetting = ms;
            _filterSetting = new FilteringSettings(RenderQueueRange.all, ms.MaskLayer);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            int temp = Shader.PropertyToID("_MaskTex");
            _soildColorID = temp;
            RenderTextureDescriptor desc = new RenderTextureDescriptor(_renderMaskSetting.RenderTextSize, _renderMaskSetting.RenderTextSize);
            cmd.GetTemporaryRT(temp, desc);

            ConfigureTarget(_soildColorID);

            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var drawSetting = CreateDrawingSettings(_shaderTag, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSetting.overrideMaterial = _renderMaskSetting.MaskMat;
            drawSetting.overrideMaterialPassIndex = 0;
            context.DrawRenderers(renderingData.cullResults, ref drawSetting, ref _filterSetting);
        }

    }

    RenderMaskPass m_ScriptablePass;
    
    public RenderMaskSetting MaskSetting;
    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new RenderMaskPass(MaskSetting);
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


