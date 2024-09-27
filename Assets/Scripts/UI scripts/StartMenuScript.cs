using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        Debug.Log("Loading Scene: " + SceneManager.GetSceneByBuildIndex(sceneIndex));
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void OpenURL()
    {
        Application.OpenURL("https://safeworldwide.org/projects/anti-poaching-campaign/");
    }
}   