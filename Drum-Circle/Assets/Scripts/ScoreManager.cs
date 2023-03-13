using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float Score = 0f;
    public float ScoreMultiplier = 1f;
    public int ComboCounter = 0;
    public int ComboCount = 0;
    public AudioManager audioManager;


    public void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void Update()
    {
        if (ScoreMultiplier > 2)
        {
            audioManager.Volume("layer2", 1f);
        }

    }

    public void Miss()
    {
        ComboCounter = 0;
        ComboCount = 0;
        ScoreMultiplier = 1f;
        Debug.Log("Miss registered");
        audioManager.Volume("layer2", 0f);
    }

    public void Hit(float proximity)
    {
        ComboCounter++;
        ComboCount++;
        Score += proximity * 100;

        Debug.Log("Count: " + ComboCount + " Counter: " + ComboCounter);

        if (ScoreMultiplier < 5 && ComboCounter >= 10)
        {
            ComboCounter = 0;
            ScoreMultiplier++;
        }
    }
}
