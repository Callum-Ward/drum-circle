using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [HideInInspector] public int[] ScoreMultiplier;
    [HideInInspector] public int[] ComboCounter;
    [HideInInspector] public int[] ComboCount;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public int[] playerScores;
    public BeatUI beatUI;


    public void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();        
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>();   
        ScoreMultiplier = new int[3];
        ComboCounter = new int[3];
        ComboCount = new int[3];
        playerScores = new int[3];
    }

    public void Update()
    {
        Tuple<int, int> maxScore = new Tuple<int, int>(0, 0);
        for(int i = 0; i < 3; i++)
        {
            if(playerScores[i] > maxScore.Item2)
            {
                maxScore = new Tuple<int, int>(i, playerScores[i]);
            }
        }

        for(int i = 0; i < 3; i++) {            
            if (ScoreMultiplier[i] > 2)
            {
                //audioManager.Volume("layer2", 1f);
            }
            beatUI.updateScore(i, playerScores[i], ComboCount[i], ScoreMultiplier[i], maxScore.Item1);
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
        playerScores[player] += Mathf.RoundToInt(Mathf.Pow((proximity * 1000f), 1.25f));

        if (ScoreMultiplier[player] < 5 && ComboCounter[player] >= 10)
        {
            ComboCounter[player] = 0;
            ScoreMultiplier[player]++;
        }
    }
}
