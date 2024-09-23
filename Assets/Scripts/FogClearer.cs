using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogClearer : MonoBehaviour
{
    public Camera fogCamera;
    private Renderer[] renderers;
    [Range(0f, 1f)] public float threshold = 0.1f;

    // made so all instances share the same texture, reducing texture reads
    private static Texture2D myT2D;
    private static Rect r_rect;
    private static bool isDirty = true;// used so that only one instance will update the RenderTexture per frame

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private Color GetColorAtPosition()
    {
        if (!fogCamera)
        {
            // if no camera is referenced script assumes there no fog and will return white (which should show the entity)
            return Color.white;
        }

        RenderTexture renderTexture = fogCamera.targetTexture;
        if (!renderTexture)
        {
            //fallback to Camera's Color
            return fogCamera.backgroundColor;
        }

        if (myT2D == null || renderTexture.width != r_rect.width || renderTexture.height != r_rect.height)
        {
            r_rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            myT2D = new Texture2D((int)r_rect.width, (int)r_rect.height, TextureFormat.RGB24, false);
        }

        if (isDirty)
        {
            RenderTexture.active = renderTexture;
            myT2D.ReadPixels(r_rect, 0, 0);
            RenderTexture.active = null;
            isDirty = false;
        }

        var pixel = fogCamera.WorldToScreenPoint(transform.position);
        return myT2D.GetPixel((int)pixel.x, (int)pixel.y);
    }

    private void Update()
    {
        isDirty = true;
    }

    void LateUpdate()
    {

        bool shouldEnableRenderers = GetColorAtPosition().grayscale >= threshold;
        foreach (var renderer in renderers)
        {
            renderer.enabled = shouldEnableRenderers;
        }
    }
}
