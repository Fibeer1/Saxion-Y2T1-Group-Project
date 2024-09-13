using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    public static TextPopup instance;

    private void Awake()
    {
        instance = this;

    }

    private IEnumerator TextPopUp()
    {
        yield break;
    }

}
