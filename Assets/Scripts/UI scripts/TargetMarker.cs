using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetMarker : MonoBehaviour
{
    public Transform target;
    private Transform playerCam;

    private void Start()
    {
        playerCam = FindObjectOfType<Camera>().transform;
    }

    private void Update()
    {
        float minX = GetComponent<Image>().GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = GetComponent<Image>().GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 targetPos = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 1.5f);

        //If the target is behind the camera
        if (Vector3.Dot(target.position - playerCam.position, playerCam.forward) < 0)
        {
            if (targetPos.x < Screen.width / 2)
            {
                targetPos.x = maxX;
            }
            else
            {
                targetPos.x = minX;
            }

        }

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        Vector2 markerPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 markerRectTransform = GetComponent<RectTransform>().position;
        Vector2 direction = markerPos - markerRectTransform;

        transform.up = -direction;

        transform.position = targetPos;
    }
}
