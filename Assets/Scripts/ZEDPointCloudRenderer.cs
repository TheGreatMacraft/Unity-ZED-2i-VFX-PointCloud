using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class ZEDPointCloudRenderer : MonoBehaviour
{
    public ZEDPointCloudManager pointCloudManager;
    public ZEDManager zedManager;
    public float particleSize = 0.003f;
    [Range(0.0f, 10.0f)] public float maxDepth;
    public ComputeShader bgraToRgbaCompute;

    VisualEffect vfx;
    int kernel;
    RenderTexture colorRGBA;

    bool initialized = false;
    int boundWidth, boundHeight;

    static readonly int TexPosScaleID = Shader.PropertyToID("Particle Position Texture");
    static readonly int TexColorID = Shader.PropertyToID("Particle Color Texture");
    static readonly int ResolutionID = Shader.PropertyToID("Resolution");

    void Start()
    {
        vfx = GetComponent<VisualEffect>();

        if (bgraToRgbaCompute != null)
            kernel = bgraToRgbaCompute.FindKernel("CSMain");
        else
            Debug.LogError("Assign bgraToRgbaCompute in the inspector (BGRAToRGBA.compute).");
    }

    void LateUpdate()
    {
        if (pointCloudManager == null || bgraToRgbaCompute == null) return;

        var xyzTex = pointCloudManager.XYZTexture;
        var colorTex = pointCloudManager.colorTexture;
        if (xyzTex == null || colorTex == null) return;

        int w = xyzTex.width;
        int h = xyzTex.height;

        if (!initialized || w != boundWidth || h != boundHeight)
        {
            if (colorRGBA != null) colorRGBA.Release();
            colorRGBA = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
            colorRGBA.enableRandomWrite = true;
            colorRGBA.Create();

            vfx.SetTexture(TexPosScaleID, xyzTex);
            vfx.SetTexture(TexColorID, colorRGBA);
            vfx.SetInt(ResolutionID, w * h);

            vfx.Reinit();

            boundWidth = w;
            boundHeight = h;
            initialized = true;

            Debug.Log($"ZED VFX bound: {w}x{h} = {w * h} particles");
        }
        // vfx.SetFloat(MaxDepthID, maxDepth);

        // ZED overwrites colorTex in place every grab, so re-dispatch every frame.
        bgraToRgbaCompute.SetTexture(kernel, "Source", colorTex);
        bgraToRgbaCompute.SetTexture(kernel, "Result", colorRGBA);
        bgraToRgbaCompute.Dispatch(kernel, Mathf.CeilToInt(w / 8f), Mathf.CeilToInt(h / 8f), 1);
    }

    void OnDestroy()
    {
        if (colorRGBA != null) colorRGBA.Release();
    }
}