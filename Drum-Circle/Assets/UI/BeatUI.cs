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
    VisualElement Lane1L, Lane1R, Lane2L, Lane2R, Lane3L, Lane3R, Lane1, Lane2, Lane3;
    VisualElement[] lanes, playerLanes;
    VisualElement container1, container2;
    Label[] playerTags, scoreTags, comboTags;
    Label[][] tags;
    public float screenWidth;
    public float screenHeight;
    public float textWidth;
    public float textHeight;

    private float[] score;
    private int[] combo;
    private int[] comboMulti;

    private float beatHeight;
    private float time;
    private float beatTargetLocation;

    BeatmapScript beatmapScript;

    TemplateContainer beatSpawnContainer1, beatSpawnContainer2;

    public VisualTreeAsset beatSpawnTemplate;
    UIDocument beatSpawnUI;

    float test = 0;
    int ctest = 0;
    int mtest = 0;

    int counter = 0;
    

    
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
        Lane1L = root.Q<VisualElement>("Lane1L");
        Lane1R = root.Q<VisualElement>("Lane1R");
        Lane2L = root.Q<VisualElement>("Lane2L");
        Lane2R = root.Q<VisualElement>("Lane2R");
        Lane3L = root.Q<VisualElement>("Lane3L");
        Lane3R = root.Q<VisualElement>("Lane3R");
        Lane1 = root.Q<VisualElement>("Lane1");
        Lane2 = root.Q<VisualElement>("Lane2");
        Lane3 = root.Q<VisualElement>("Lane3");

        beatmapScript = GameObject.Find("Rhythm Logic").GetComponent<BeatmapScript>();
        beatTargetLocation = beatmapScript.beatTargetLocation;

        playerTags = new Label[] {playerTag1, playerTag2, playerTag3};
        scoreTags = new Label[] {scoreTag1, scoreTag2, scoreTag3};
        comboTags = new Label[] {comboTag1, comboTag2, comboTag3};
        tags = new Label[][] {playerTags, scoreTags, comboTags};

        playerLanes = new VisualElement[] {Lane1, Lane2, Lane3};
        lanes = new VisualElement[] {Lane1L, Lane2L, Lane3L, Lane1R, Lane2R, Lane3R};

        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < 3; j++) {
                tags[i][j].visible = false;
            }
            playerLanes[i].style.display = DisplayStyle.None;
        }

        beatSpawnUI = GetComponent<UIDocument>();

    }

    public void setPlayerCount(int number) {
        playerNo = number;
    }

    public void updateScore(int player, float scoreVal, int comboVal, int multiVal) {
        scoreTags[player-1].text = "Score: " + scoreVal;
        comboTags[player-1].text = "Combo: " + comboVal + "\nMultiplier: " + multiVal;
    }

    public void createBeat() {

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
            for (int i = 0; i < playerNo; i++) {
                for (int j = 0; j < 3; j++) {
                    tags[j][i].visible = true;
                    tags[j][i].style.left = screenWidth*(i+1) /(playerNo+1) - (tags[j][i].resolvedStyle.width/2);
                    tags[j][i].style.top = (screenHeight * (j+2) /(tags[j][i].resolvedStyle.height)) + j*5;
                }
            playerLanes[i].style.display = DisplayStyle.Flex;
            } 
        }

        

        beatSpawnContainer1 = beatSpawnTemplate.Instantiate();
        beatSpawnContainer2 = beatSpawnTemplate.Instantiate();
        container1 = new VisualElement();  
        container2 = new VisualElement();  
        container1.Add(beatSpawnContainer1);
        container2.Add(beatSpawnContainer2);
        Lane1L.Add(container1);
        Lane1R.Add(container2);
        container1.style.position = Position.Absolute;
        container2.style.position = Position.Absolute;
        container1.style.top = new StyleLength(Mathf.RoundToInt(screenHeight*(1-beatTargetLocation)));
        container2.style.top = new StyleLength(Mathf.RoundToInt(screenHeight*(1-beatTargetLocation)));
    }

    void Update() {
        updateScore(1, test, ctest, mtest);
        time = time + Time.deltaTime;
        test++;
        if(mtest == 5) {
            ctest++;
        }
        else {
            ctest++;
            mtest = Mathf.FloorToInt(ctest/100);
        }
    
        if(counter == 50) {
            counter = 0;
        }
        else
            counter++;
        
    }
    
}
