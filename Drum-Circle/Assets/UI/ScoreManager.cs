using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.StyleSheets;

public class ScoreManager : MonoBehaviour
{
    [HideInInspector] public int[] ScoreMultiplier;
    [HideInInspector] public int[] ComboCounter;
    [HideInInspector] public int[] ComboCount;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public int[] playerScores;
    private string[] scores;
    public BeatUI beatUI;
    private VisualElement endScore;


    public void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();        
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>();   
        endScore = GameObject.Find("EndScore").GetComponent<UIDocument>().rootVisualElement;   
        ScoreMultiplier = new int[3];
        ComboCounter = new int[3];
        ComboCount = new int[3];
        playerScores = new int[3];
        // VisualElement Score1 = new VisualElement endScore.Q<VisualElement>("Score1");
        // VisualElement Score2 = new VisualElement endScore.Q<VisualElement>("Score2");
        // VisualElement Score3 = new VisualElement endScore.Q<VisualElement>("Score3");
        scores = new string[] {"Score1", "Score2", "Score3"};
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
                audioManager.AddLayer();
            }
            beatUI.updateScore(i, playerScores[i], ComboCount[i], ScoreMultiplier[i], maxScore.Item1);
            endScore.Q<Label>(scores[i]).text = playerScores[i].ToString();
        }
        endScore.Q<Label>("ScoreT").text = (playerScores[0] + playerScores[1] + playerScores[2]).ToString();
    }

    //Resets combo and score multiplier on miss. Also mutes a layer of music.
    public void Miss(int player)
    {
        ComboCounter[player] = 0;
        ComboCount[player] = 0;
        ScoreMultiplier[player] = 1;
        //Debug.Log("Miss registered");
        //audioManager.FadeOut("layer2");
        audioManager.RemoveLayer();
    }

    //Increments counters on hit and calculates multiplier increase.
    public void Hit(float proximity, int player)
    {
        ComboCounter[player]++;
        ComboCount[player]++;
        playerScores[player] += Mathf.RoundToInt(Mathf.Pow((proximity * 1000f), 1.25f));

        if (ScoreMultiplier[player] < 5 && ComboCounter[player] >= 5) //10
        {
            ComboCounter[player] = 0;
            ScoreMultiplier[player]++;
        }
    }
}
