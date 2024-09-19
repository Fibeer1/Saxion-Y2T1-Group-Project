using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetMarker : UIMarker
{

    private void Start()
    {
        playerCam = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        HandleTargetTracking();
    }
}
