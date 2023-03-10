using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BeatUI : MonoBehaviour
{
    public bool gameStart = true;
    public int playerNo = 0;
    public int count = 0;
    VisualElement root;
    Label playerTag1, playerTag2, playerTag3;
    Label scoreTag1, scoreTag2, scoreTag3;
    Label comboTag1, comboTag2, comboTag3;
    Label[] playerTags;
    Label[] scoreTags;
    Label[] comboTags;
    Label[][] tags;
    public float screenWidth;
    public float screenHeight;
    public float textWidth;
    public float textHeight;

    private float[] score;
    private int[] combo;
    private int[] comboMulti;

    BeatmapScript beatmapScript;

    float test = 0;
    int ctest = 0;
    int mtest = 0;
    

    
    private void OnEnable()
    {
        root = GameObject.Find("BeatSpawnUI").GetComponent<UIDocument>().rootVisualElement;

        playerTag1 = root.Q<Label>("Player1");
        playerTag2 = root.Q<Label>("Player2");
        playerTag3 = root.Q<Label>("Player3");
        scoreTag1 = root.Q<Label>("Score1");
        scoreTag2 = root.Q<Label>("Score2");
        scoreTag3 = root.Q<Label>("Score3");
        comboTag1 = root.Q<Label>("Combo1");
        comboTag2 = root.Q<Label>("Combo2");
        comboTag3 = root.Q<Label>("Combo3");

        playerTag1.style.display = DisplayStyle.None;
        playerTag2.style.display = DisplayStyle.None;
        playerTag3.style.display = DisplayStyle.None;
        
        playerTags = new Label[] {playerTag1, playerTag2, playerTag3};
        scoreTags = new Label[] {scoreTag1, scoreTag2, scoreTag3};
        comboTags = new Label[] {comboTag1, comboTag2, comboTag3};
        tags = new Label[][] {playerTags, scoreTags, comboTags};

    }

    public void setPlayerCount(int number) {
        playerNo = number;
    }

    public void updateScore(int player, float scoreVal, int comboVal, int multiVal) {
        // score[player-1] = scoreVal;
        // combo[player-1] = comboVal;
        // comboMulti[player-1] = multiVal;
        scoreTags[player-1].text = "Score: " + scoreVal;
        comboTags[player-1].text = "Combo: " + comboVal + "\nMultiplier: " + multiVal;
    }

    // Update is called once per frame
    public void startLevelUI()
    {        
        screenWidth = root.Q<VisualElement>("ScreenContainer").resolvedStyle.width;
        screenHeight = root.Q<VisualElement>("ScreenContainer").resolvedStyle.height;
        float tag1Center = playerTag1.resolvedStyle.width / 2;
        float tag2Center = playerTag2.resolvedStyle.width / 2;
        float tag3Center = playerTag3.resolvedStyle.width / 2;

        if(playerNo > 3 || playerNo < 1) {
            Debug.Log("invalid player count");
        }
        else {
            for (int j = 0; j < 3; j++) {
                for (int i = 0; i < playerNo; i++) {
                    tags[j][i].style.display = DisplayStyle.Flex;
                    tags[j][i].style.left = screenWidth*(i+1) /(playerNo+1) - (tags[j][i].resolvedStyle.width/2);
                    tags[j][i].style.top = (screenHeight * (j+2) /(tags[j][i].resolvedStyle.height)) + j*10;
                }
            }
        }
    }

    void Update() {
        updateScore(1, test, ctest, mtest);
        test++;
        if(mtest == 5) {
            ctest++;
        }
        else {
            ctest++;
            mtest = Mathf.FloorToInt(ctest/100);
        }
    }
}
