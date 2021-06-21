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

        RenderTargetHandle _colorTex;


        public AirDistortionPass(RenderSetting setting)
        {
            _renderSetting = setting;
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
            
        }
    }

    AirDistortionPass m_ScriptablePass;
    public RenderSetting Setting;
    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new AirDistortionPass(Setting);
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        var desc = RenderTargetHandle.CameraTarget;

        renderer.EnqueuePass(m_ScriptablePass);
    }
}


