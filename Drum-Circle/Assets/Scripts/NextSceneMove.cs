using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneMove : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadCharacter()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
