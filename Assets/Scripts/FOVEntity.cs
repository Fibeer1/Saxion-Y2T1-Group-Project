using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVEntity : MonoBehaviour
{
    private Renderer[] meshRenderers;
    public bool isBeingSeen = false;
    private LayerMask originalLayer;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<Renderer>();
        originalLayer = gameObject.layer;
        isBeingSeen = false;
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = false;
            gameObject.layer = 2;
        }
    }

    private void Update()
    {
        HandleBeingSeen();
    }

    private void HandleBeingSeen()
    {
        if (isBeingSeen)
        {
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = true;
                gameObject.layer = originalLayer;
            }
        }
        else
        {
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = false;               
                gameObject.layer = 2;
            }
        }
    }
}
