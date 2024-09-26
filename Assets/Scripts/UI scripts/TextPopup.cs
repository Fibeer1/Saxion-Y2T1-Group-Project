using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopup : MonoBehaviour
{
    public static TextPopup instance;
    [SerializeField] private Transform textMeshParent;
    [SerializeField] private GameObject textMeshPrefab;
    public List<TextPopUpInstance> textMeshes;

    private void Awake()
    {
        instance = this;
    }

    private void OnTextPopUp(string text, Color textColor, float fadeDuration, float lifeTime)
    {
        TextPopUpInstance textInstance = Instantiate(textMeshPrefab, textMeshParent.position, Quaternion.identity, textMeshParent).GetComponent<TextPopUpInstance>();
        textInstance.transform.localPosition = Vector3.zero;
        textInstance.textMesh.text = text;
        textInstance.opaqueColor = textColor;
        textInstance.opaqueColor.a = 1;
        textInstance.transparentColor = new Color(textColor.r, textColor.g, textColor.b, 0);
        textInstance.fadeDuration = fadeDuration;
        textInstance.lifeTime = lifeTime;
        textInstance.transform.position -= Vector3.up * 75 * textMeshes.Count;
        textInstance.textPopupHandler = this;
        textMeshes.Add(textInstance);
    }

    public static void PopUpText(string text, Color textColor, float fadeDuration, float lifeTime)
    {
        instance.OnTextPopUp(text, textColor, fadeDuration, lifeTime);
    }

}
