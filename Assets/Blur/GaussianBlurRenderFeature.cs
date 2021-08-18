using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianBlurRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]

    public class BlurRenderSetting
    {
        public RenderPassEvent RenderEvent = RenderPassEvent.AfterRenderingOpaques;
        public float BlurSize = 0.1f;
        public float Gaussian = 0f;
        public float StandardDeviation = 0.02f;
        public float Sample = 1f;
        public Material BlurMat;
        
    
    }

    class CustomRenderPass : ScriptableRenderPass
    {

        private Material _blurMat;

        private RenderTargetIdentifier _source;
        private RenderTargetHandle _desc;

        private string _proflieTag;

        private RenderTargetHandle _buffer0Tex;
        private RenderTargetHandle _buffer1Tex;
        


        public CustomRenderPass(BlurRenderSetting setting)
        {
            this.renderPassEvent = setting.RenderEvent;
            _blurMat = setting.BlurMat;

            _proflieTag = "Gaussian Blur";

            _buffer0Tex.Init("BufferTex0");
            _buffer1Tex.Init("BufferTex1");
        }

        public void Setup(RenderTargetIdentifier src, RenderTargetHandle desc)
        {
            _source = src;
            _desc = desc;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(_proflieTag);
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;

            cmd.GetTemporaryRT(_buffer0Tex.id, opaqueDesc);
            cmd.GetTemporaryRT(_buffer1Tex.id, opaqueDesc);

            Blit(cmd, _source, _buffer0Tex.Identifier(), _blurMat);
            Blit(cmd, _buffer0Tex.Identifier(), _source);

            cmd.ReleaseTemporaryRT(_buffer0Tex.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }


    public BlurRenderSetting RenderSetting = new BlurRenderSetting();
    CustomRenderPass m_ScriptablePass;

    private readonly int _blurSizeID = Shader.PropertyToID("_BlurSize");
    private readonly int _sampleID = Shader.PropertyToID("_Sample");
    private readonly int _gaussianID = Shader.PropertyToID("_Gaussian");
    private readonly int _standardDeviationID = Shader.PropertyToID("StandardDeviation");

    /// <inheritdoc/>
    public override void Create()
    {

        RenderSetting.BlurMat.SetFloat(_blurSizeID, RenderSetting.BlurSize);
        RenderSetting.BlurMat.SetFloat(_sampleID, RenderSetting.Sample);
        RenderSetting.BlurMat.SetFloat(_gaussianID, RenderSetting.Gaussian);
        RenderSetting.BlurMat.SetFloat(_standardDeviationID, RenderSetting.StandardDeviation);

        m_ScriptablePass = new CustomRenderPass(RenderSetting);

        // Configures where the render pass should be injected.
        //m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
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


