using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneMove : MonoBehaviour
{
    public void Continue()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadCharacter()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void NumberPlayer()
    {
        SceneManager.LoadScene("");
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("2MissionSelect");
    }

    public void Return()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
