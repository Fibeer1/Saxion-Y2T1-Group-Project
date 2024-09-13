using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public static EndScreen Instance;
    [SerializeField] private GameObject endScreenMenu;
    [SerializeField] private TextMeshProUGUI endText;

    private void Awake() => Instance = this;

    private IEnumerator OnToggleEndScreen(string text, float fadeDuration, float fadeDelay)
    {
        Fader.Fade(true, fadeDuration, fadeDelay);
        yield return new WaitForSeconds(fadeDelay + fadeDuration + 0.5f);
        endScreenMenu.SetActive(true);
        endText.text = text;
        yield return new WaitForSeconds(2);
        while (AudioListener.volume > 0)
        {
            AudioListener.volume -= 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public static void ToggleEndScreen(string text, float fadeDuration, float fadeDelay) => Instance.StartCoroutine(Instance.OnToggleEndScreen(text, fadeDuration, fadeDelay));
}
