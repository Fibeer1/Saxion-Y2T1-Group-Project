using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "MainScene";
    [SerializeField] private string loadSettings = "Settings Menu";
    [SerializeField] private string loadMainMenu = "Main Menu";

    public void NewGameButton()
    {
        SceneManager.LoadScene(newGameLevel);
    }
    public void LoadSettings()
    {
        SceneManager.LoadScene(loadSettings);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(loadMainMenu);
    }
}   