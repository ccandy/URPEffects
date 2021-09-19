using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScanEffectFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class RenderSetting
    {
        public RenderPassEvent RenderEvent = RenderPassEvent.AfterRenderingOpaques;
        public float ScanVelociry = 1;
        public Material ScanMat;
    }

    class ScanEffectPass : ScriptableRenderPass
    {

        private Material _scanMat;
        
        private RenderTargetIdentifier src;
        private RenderTargetHandle dest;

        private string _proflieTag;

        private RenderTargetHandle _renderTex;


        public ScanEffectPass(RenderSetting setting)
        {
            _scanMat = setting.ScanMat;
            _proflieTag = "ScanEffect";
            _renderTex.Init("RenderTex");
            
        }

        public void Setup(RenderTargetIdentifier src, RenderTargetHandle des)
        {
            this.src = src;
            this.dest = des;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(_proflieTag);
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;

            cmd.GetTemporaryRT(_renderTex.id, opaqueDesc);

            Blit(cmd, src, _renderTex.Identifier(), _scanMat);
            Blit(cmd, _renderTex.Identifier(), src);

            cmd.ReleaseTemporaryRT(_renderTex.id);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }


    public RenderSetting ScanEffectRenderSetting = new RenderSetting();
    ScanEffectPass _scanEffectPass;
    private readonly int ScanVelocityID = Shader.PropertyToID("_ScanVelocity");
    /// <inheritdoc/>
    public override void Create()
    {
        ScanEffectRenderSetting.ScanMat.SetFloat(ScanVelocityID, ScanEffectRenderSetting.ScanVelociry);
        _scanEffectPass = new ScanEffectPass(ScanEffectRenderSetting);
        _scanEffectPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        RenderTargetIdentifier src = renderer.cameraColorTarget;
        RenderTargetHandle des = RenderTargetHandle.CameraTarget;

        _scanEffectPass.Setup(src, des);
        renderer.EnqueuePass(_scanEffectPass);
    }
}


