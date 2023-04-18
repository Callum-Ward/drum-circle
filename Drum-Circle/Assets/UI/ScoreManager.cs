using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int[] ScoreMultiplier;
    public int[] ComboCounter;
    public int[] ComboCount;
    public AudioManager audioManager;
    public int[] playerScores;
    public BeatUI beatUI;


    public void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();        
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>();   
        int[] ScoreMultiplier = new int[3];
        int[] ComboCounter = new int[3];
        int[] ComboCount = new int[3];
        int[] playerScores = new int[3];
    }

    public void Update()
    {
        Debug.Log("Length of ScoreM" + ScoreMultiplier.Length);
        Debug.Log("Length of ComboCount" + ComboCount.Length);
        Debug.Log("Length of PlayerSc" + playerScores.Length);
        for(int i = 0; i < 3; i++) {            
            if (ScoreMultiplier[i] > 2)
            {
                //audioManager.Volume("layer2", 1f);
            }
            Debug.Log("I = " + i);
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
        playerScores[player] += Mathf.RoundToInt(Mathf.Pow((proximity * 1000f), 1.25f));

        if (ScoreMultiplier[player] < 5 && ComboCounter[player] >= 10)
        {
            ComboCounter[player] = 0;
            ScoreMultiplier[player]++;
        }
    }
}
