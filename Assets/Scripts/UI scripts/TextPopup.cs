using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopup : MonoBehaviour
{
    public static TextPopup instance;
    private Vector3 originalPosition;
    [SerializeField] private TextMeshProUGUI textMesh;
    private float elapsedTime = 0;

    private void Awake()
    {
        originalPosition = textMesh.rectTransform.localPosition;
        textMesh.gameObject.SetActive(false);
        instance = this;
    }

    private IEnumerator OnTextPopUp(string text, float fadeDuration, float lifeTime)
    {
        if (textMesh.gameObject.activeInHierarchy)
        {
            textMesh.text = textMesh.text + "\n" + text;
            Debug.Log("Before: " + elapsedTime);
            elapsedTime -= 3; //Increase the lifetime of the text by 1 second each time text is added
            Debug.Log("After: " + elapsedTime);
            yield break;
        }
        textMesh.text = text;
        bool shouldFadeIn = true;
        textMesh.gameObject.SetActive(true);
        textMesh.transform.localPosition = originalPosition;        
        textMesh.alpha = 0;
        
        elapsedTime = 0;
        float fadeElapsedTime = 0;
        
        while (true)
        {
            elapsedTime += Time.deltaTime;
            textMesh.transform.position += textMesh.transform.up / 50;
            if (shouldFadeIn)
            {
                fadeElapsedTime += Time.deltaTime;
                textMesh.alpha = Mathf.Lerp(0, 1, fadeElapsedTime / fadeDuration);
            }
            if (fadeElapsedTime >= fadeDuration)
            {
                shouldFadeIn = false;
                fadeElapsedTime = 0;
            }


            if (elapsedTime >= lifeTime)
            {
                if (!shouldFadeIn)
                {
                    fadeElapsedTime += Time.deltaTime;
                    textMesh.alpha = Mathf.Lerp(1, 0, fadeElapsedTime / fadeDuration);
                    if (fadeElapsedTime >= fadeDuration)
                    {
                        break;
                    }
                }               
            }
            yield return null;
        }
        textMesh.gameObject.SetActive(false);
    }

    public static void PopUpText(string text, float fadeDuration, float lifeTime)
    {
        instance.StartCoroutine(instance.OnTextPopUp(text, fadeDuration, lifeTime));
    }

}
