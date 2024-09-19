using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMarker : MonoBehaviour
{
    public Transform target;
    private protected Camera playerCam;

    private protected void HandleTargetTracking()
    {
        float minX = GetComponent<Image>().GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = GetComponent<Image>().GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 targetPos = playerCam.WorldToScreenPoint(target.position);

        //If the target is behind the camera
        if (Vector3.Dot(target.position - playerCam.transform.position, playerCam.transform.forward) < 0)
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

        targetPos.y += 50;
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        transform.position = targetPos;
    }
}
