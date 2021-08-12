using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AirDistortion : ScriptableRendererFeature
{

    [System.Serializable]
    public class RenderSetting
    {
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
        public Material AirDistortionMat;

        public float DistortionTimeFactor;
        public float DistortionStrongFactor;
        public Texture2D NoiseTex;

    }


    class AirDistortionPass : ScriptableRenderPass
    {
        private RenderSetting _renderSetting;
        private string _profilerTag;

        public Material BiltMat = null;
        private RenderTargetIdentifier source { set; get; }
        private RenderTargetHandle destination { set; get; }

        RenderTargetHandle _tempColorTex;

        RenderTargetHandle _colorTex;


        public AirDistortionPass(RenderSetting setting)
        {

            _profilerTag = "AirDistortion";
            _renderSetting = setting;
            BiltMat = setting.AirDistortionMat;
            _tempColorTex.Init("_TempColorTex");

        }

        public void Setup(RenderTargetIdentifier src, RenderTargetHandle desc)
        {
            source = src;
            destination = desc;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);
            RenderTextureDescriptor rtDesc = renderingData.cameraData.cameraTargetDescriptor;

            cmd.GetTemporaryRT(_tempColorTex.id, rtDesc);
            Blit(cmd, source, _tempColorTex.Identifier(), BiltMat);
            Blit(cmd, _tempColorTex.Identifier(), source);

            cmd.ReleaseTemporaryRT(_tempColorTex.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    private static readonly int DistortTimeFactorId = Shader.PropertyToID("_DistorTimeFactor");
    private static readonly int DistortionStrongFactorId = Shader.PropertyToID("_DistorStrongFactor");
    private static readonly int NoiseTexId = Shader.PropertyToID("_NoiseTex");


    AirDistortionPass m_ScriptablePass;

    public RenderSetting Setting = new RenderSetting();

    //public float DistortionStrongFactor { get; private set; }

    /// <inheritdoc/>
    public override void Create()
    {

        if(Setting.AirDistortionMat != null)
        {
            Setting.AirDistortionMat.SetFloat(DistortTimeFactorId, this.Setting.DistortionTimeFactor);
            Setting.AirDistortionMat.SetFloat(DistortionStrongFactorId, this.Setting.DistortionStrongFactor);
            Setting.AirDistortionMat.SetTexture(NoiseTexId, this.Setting.NoiseTex);
        }
        
        m_ScriptablePass = new AirDistortionPass(Setting);
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        
    }

    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        var desc = RenderTargetHandle.CameraTarget;

        renderer.EnqueuePass(m_ScriptablePass);
    }
}


