using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderFeature : ScriptableRendererFeature
{

    [System.Serializable]
    public class RenderSetting
    {
        public RenderPassEvent RenderEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material BlurMat;
        public float BlurSize;
    }

    class CustomRenderPass : ScriptableRenderPass
    {
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.

        string _profileTag;

        Material _blurMat;

        private RenderTargetIdentifier src;
        private RenderTargetHandle dest;

        private RenderTargetHandle _bufferTex0;
        private RenderTargetHandle _bufferTex1;


        public CustomRenderPass(RenderSetting setting)
        {
            _profileTag = "BlurEffect";
            this.renderPassEvent = setting.RenderEvent;
            _blurMat = setting.BlurMat;

            _bufferTex0.Init("BufferTex0");
            _bufferTex1.Init("BufferTex1");
        }

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            src = source;
            dest = destination;
        }


        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(_profileTag);
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;

            cmd.GetTemporaryRT(_bufferTex0.id, opaqueDesc);
            cmd.GetTemporaryRT(_bufferTex1.id, opaqueDesc);

            Blit(cmd, src, _bufferTex0.Identifier(), _blurMat);

            //Blit(cmd, _bufferTex0.Identifier(), _bufferTex1.Identifier(), _blurMat, 0);
            //Blit(cmd, _bufferTex1.Identifier(), _bufferTex0.Identifier(), _blurMat, 1);

            //Blit(cmd, _bufferTex0.Identifier(), src, _blurMat);
            Blit(cmd, _bufferTex0.Identifier(), src);

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

    public RenderSetting BlurRenderSetting = new RenderSetting();
    private static readonly int BlurSizeID = Shader.PropertyToID("_BlurSize");

    /// <inheritdoc/>
    public override void Create()
    {

        BlurRenderSetting.BlurMat.SetFloat(BlurSizeID, this.BlurRenderSetting.BlurSize);
        m_ScriptablePass = new CustomRenderPass(BlurRenderSetting);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        var desc = RenderTargetHandle.CameraTarget;

        m_ScriptablePass.Setup(src, desc);

        renderer.EnqueuePass(m_ScriptablePass);
    }
}


