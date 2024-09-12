using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVEntity : MonoBehaviour
{
    private Renderer[] meshRenderers;
    [SerializeField] private Collider coll;
    public bool isBeingSeen = false;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<Renderer>();
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
            }
        }
        else
        {
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = false;
            }
        }
    }
}
