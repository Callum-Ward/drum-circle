using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int[] ScoreMultiplier = {1, 1, 1};
    public int[] ComboCounter = {0, 0, 0};
    public int[] ComboCount = {0, 0, 0};
    public AudioManager audioManager;
    public int[] playerScores = {0, 0, 0};
    public BeatUI beatUI;


    public void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();        
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>();
    }

    public void Update()
    {
        for(int i = 0; i < 3; i++) {            
            if (ScoreMultiplier[i] > 2)
            {
                //audioManager.Volume("layer2", 1f);
            }
            beatUI.updateScore(i, playerScores[i], ComboCount[i], ScoreMultiplier[i]);
        }

    }

    //Resets combo and score multiplier on miss. Also mutes a layer of music.
    public void Miss(int player)
    {
        ComboCounter[player] = 0;
        ComboCount[player] = 0;
        ScoreMultiplier[player] = 1;
        //Debug.Log("Miss registered");
        //audioManager.FadeOut("layer2");
    }

    //Increments counters on hit and calculates multiplier increase.
    public void Hit(float proximity, int player)
    {
        ComboCounter[player]++;
        ComboCount[player]++;
        playerScores[player] += Mathf.RoundToInt(Mathf.Pow((proximity * 100f), 1.25f));

        if (ScoreMultiplier[player] < 5 && ComboCounter[player] >= 10)
        {
            ComboCounter[player] = 0;
            ScoreMultiplier[player]++;
        }
    }
}
