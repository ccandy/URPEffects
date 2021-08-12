using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomPassFeature : ScriptableRendererFeature
{

    [System.Serializable]
    public class RenderSetting
    {
        public RenderPassEvent RenderEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material BloomMat;
        public int Iterator;

    }
    class CustomRenderPass : ScriptableRenderPass
    {
        string _profileTag;

        private RenderTargetIdentifier src;
        private RenderTargetHandle dest;

        public Material BiltMat;

        private RenderTargetHandle _bufferTex0;
        private RenderTargetHandle _bufferTex1;

        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
        private int _iterator;

        public CustomRenderPass(RenderSetting setting)
        {
            _profileTag = "BloomEffect";
            this.renderPassEvent = setting.RenderEvent;
            BiltMat = setting.BloomMat;
            
            _bufferTex0.Init("_BufferTex0");
            _bufferTex1.Init("_BufferTex1");

            _iterator = setting.Iterator;
        }

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {

        }



        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(_profileTag);
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;

            opaqueDesc.width /= _iterator;
            opaqueDesc.height /= _iterator;

            cmd.GetTemporaryRT(_bufferTex0.id, opaqueDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(_bufferTex1.id, opaqueDesc, FilterMode.Bilinear);

            Blit(cmd, source, _bufferTex0.Identifier());

            

            //for (int n = 0; n < _iterator; n++)
            //{
                Blit(cmd, _bufferTex0.Identifier(), _bufferTex1.Identifier());
                Blit(cmd, _bufferTex1.Identifier(), _bufferTex0.Identifier());
            //}
            Blit(cmd, _bufferTex0.Identifier(), source);

            cmd.ReleaseTemporaryRT(_bufferTex1.id);
            cmd.ReleaseTemporaryRT(_bufferTex0.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;
    public RenderSetting _renderSetting = new RenderSetting();
    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(_renderSetting);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        var dest = RenderTargetHandle.CameraTarget;
        m_ScriptablePass.Setup(src, dest);

        renderer.EnqueuePass(m_ScriptablePass);
    }
}


