using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVEntity : MonoBehaviour
{
    private Renderer[] meshRenderers;
    public bool isBeingSeen = false;
    private LayerMask originalLayer;
    [SerializeField] private LayerMask ignoreRaycastLayer;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<Renderer>();
        originalLayer = gameObject.layer;
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = false;
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
                gameObject.layer = ignoreRaycastLayer.value;
            }
        }
    }
}
