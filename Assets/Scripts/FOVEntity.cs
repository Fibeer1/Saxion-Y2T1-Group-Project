using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVEntity : MonoBehaviour
{
    private Renderer[] meshRenderers;
    public bool isBeingSeen = false;
    private LayerMask originalLayer;
    [SerializeField] private Material transparentMaterial;
    private List<Material> rendererMaterials = new List<Material>();
    private bool isFading;
    private bool isVisible;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in meshRenderers)
        {
            rendererMaterials.Add(renderer.material);
        }
        originalLayer = gameObject.layer;
        isBeingSeen = false;
        isVisible = false;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].enabled = false;
            rendererMaterials[i] = meshRenderers[i].material;
            gameObject.layer = 2;
        }
    }

    private void Update()
    {
        HandleBeingSeen();
    }

    private void HandleBeingSeen()
    {
        if (isBeingSeen && !isVisible)
        {
            StartCoroutine(FadeOnAppearing(true));
        }
        else if (isVisible)
        {
            StartCoroutine(FadeOnAppearing(false));
        }
    }

    public IEnumerator FadeOnAppearing(bool fadeIn)
    {
        if (isFading)
        {
            yield break;
        }
        isFading = true;
        float duration = 1;
        float elapsedTime = 0;
        gameObject.layer = fadeIn ? originalLayer : 2;
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = true;
        }


        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeIn)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].material.Lerp(transparentMaterial, rendererMaterials[i], elapsedTime / duration);
                }
            }
            else
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].material.Lerp(rendererMaterials[i], transparentMaterial, elapsedTime / duration);
                }
            }
            yield return null;
        }

        if (!fadeIn)
        {
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = true;
            }
        }
        isVisible = fadeIn;
        isFading = false;
    }
}
