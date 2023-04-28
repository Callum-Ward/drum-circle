using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UIElements;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.Experimental;
using System.Linq;

public class BeatUI : MonoBehaviour
{
    [HideInInspector] public bool gameStart = true;
    public int playerNo = 3;
    [HideInInspector] public int count = 0;
    VisualElement root;
    Label playerTag1, playerTag2, playerTag3;
    Label scoreTag1, scoreTag2, scoreTag3;
    Label comboTag1, comboTag2, comboTag3;
    Label introTimer, freestyleNotice;
    VisualElement Lane1L, Lane1R, Lane2L, Lane2R, Lane3L, Lane3R, Lane1, Lane2, Lane3, lane1Container, lane2Container, lane3Container;
    [HideInInspector] public VisualElement[] lanes, playerLanes, laneContainers;
    VisualElement[] container = new VisualElement[6];
    Label[] playerTags, scoreTags, comboTags;
    Label[][] tags;
    [HideInInspector] public float screenWidth;
    [HideInInspector] public float screenHeight;
    [HideInInspector] public float textWidth;
    [HideInInspector] public float textHeight;
    private int start = 0;
    private bool startFade = false;

    private float[] score;
    private int[] combo;
    private int[] comboMulti;

    private float beatHeight;
    private float time;
    private float beatTargetLocation;

    BeatmapScript beatmapScript;

    TemplateContainer[] beatSpawnContainer = new TemplateContainer[6];

    public VisualTreeAsset beatSpawnTemplate;
    UIDocument beatSpawnUI;

    float test = 0;
    int ctest = 0;
    int mtest = 0;

    int counter = 0;

    private GUIStyle guiStyle = new GUIStyle();

    private class TargetEffect {
        public int swellStage;
        public float swellEffect;
    }

    private TargetEffect[] beatTargetEffects = new TargetEffect[6];
    private int[] drumIndexUIMap = new int[6]{0, 3, 1, 4, 2, 5};

    
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
        lane1Container = root.Q<VisualElement>("Lane1Container");
        lane2Container = root.Q<VisualElement>("Lane2Container");
        lane3Container = root.Q<VisualElement>("Lane3Container");
        introTimer = root.Q<Label>("IntroTimer");
        freestyleNotice = root.Q<Label>("FreestyleNotice");
   
        guiStyle.fontSize = 60;

        beatmapScript = GameObject.Find("Rhythm Logic").GetComponent<BeatmapScript>();
        beatTargetLocation = beatmapScript.beatTargetLocation;

        playerTags = new Label[] {playerTag1, playerTag2, playerTag3};
        scoreTags = new Label[] {scoreTag1, scoreTag2, scoreTag3};
        comboTags = new Label[] {comboTag1, comboTag2, comboTag3};
        tags = new Label[][] {playerTags, scoreTags, comboTags};
        laneContainers = new VisualElement[] {lane1Container, lane2Container, lane3Container};


        playerLanes = new VisualElement[] {Lane1, Lane2, Lane3};
        lanes = new VisualElement[] {Lane1L, Lane2L, Lane3L, Lane1R, Lane2R, Lane3R};

        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < 3; j++) {
                tags[i][j].visible = false;
            }
            // playerLanes[i].style.display = DisplayStyle.None;
        }

        for(int i = 0; i < 6; i++){
            beatTargetEffects[i] = new TargetEffect();
            beatTargetEffects[i].swellStage = 0;
            beatTargetEffects[i].swellEffect = 1f;
        }


        beatSpawnUI = GetComponent<UIDocument>();

    }

    public void setPlayerCount(int number) {
        playerNo = number;
    }

    public void updateScore(int player, float scoreVal, int comboVal, int multiVal) {
        scoreTags[player].text = "Score: " + scoreVal;
        comboTags[player].text = "Combo: " + comboVal + "\nMultiplier: " + multiVal;
    }

    public void hitSwell(int drumIndex) {
        beatTargetEffects[drumIndexUIMap[drumIndex]].swellStage = 1;
    }

    public void failShake(int player, int strength) {
        // laneContainers[player].style.position = Position.Absolute;
        float x = (UnityEngine.Random.Range(-1f, 1f) * strength);
        laneContainers[player].style.left = new StyleLength(Mathf.RoundToInt(x));
        Debug.Log("current position: " + laneContainers[player].style.translate);
        // laneContainers[player].style.translate = new Translate(0, 0, 0);
    }

    // Update is called once per frame
    public void startLevelUI()
    {     
        
        float targetSize = Lane1L.resolvedStyle.width/4;
        if (start < 2)   
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
                beatSpawnContainer[i] = beatSpawnTemplate.Instantiate();
                beatSpawnContainer[i+3] = beatSpawnTemplate.Instantiate();
                container[i] = new VisualElement();
                container[i+3] = new VisualElement();
                container[i].Add(beatSpawnContainer[i]);
                container[i+3].Add(beatSpawnContainer[i+3]);
                lanes[i].Add(container[i]);
                lanes[i+3].Add(container[i+3]);
                container[i].style.position = Position.Absolute;
                container[i+3].style.position = Position.Absolute;
                container[i].style.top = new StyleLength(Mathf.RoundToInt((screenHeight*(1-beatTargetLocation))+targetSize));
                container[i+3].style.top = new StyleLength(Mathf.RoundToInt((screenHeight*(1-beatTargetLocation))+targetSize));
                } 
             }
        
        int targetOffset = Mathf.RoundToInt(screenHeight*beatmapScript.inputDelay);

        

            
            // Lane1L.Add(container1);
            // Lane1R.Add(container2);
                

            // container1.style.position = Position.Absolute;
            // container2.style.position = Position.Absolute;
            // container2.style.top = new StyleLength(Mathf.RoundToInt((screenHeight*(1-beatTargetLocation))+targetSize));
            // container1.style.top = new StyleLength(Mathf.RoundToInt((screenHeight*(1-beatTargetLocation))+targetSize));
            start++;
        }
    }

    public void IntroTimerStart() 
    {
        introTimer.visible = true;
        introTimer.text = "5";
    }

    public void IntroTimerUpdate(float time) 
    {
        introTimer.text = Mathf.Ceil(time).ToString();
    }

    public void IntroTimerStop() 
    {
        introTimer.text = "GO!";
        StartCoroutine(FadeOutCoroutine(introTimer));
    }

    public void FreestyleNoticeStart(int playerIndex)
    {
        Debug.Log("FNS");
        freestyleNotice.visible = true;
        freestyleNotice.text = "Freestyle Player " + (playerIndex + 1).ToString();
    }

    public void FreestyleNoticeStop() 
    {
        StartCoroutine(FadeOutCoroutine(freestyleNotice));
    }

    IEnumerator beatSwellCo(int drumIndex)
    {
        if(beatTargetEffects[drumIndex].swellStage == 1)
        {
            if(beatTargetEffects[drumIndex].swellEffect < 1.15f)
            {
                beatTargetEffects[drumIndex].swellEffect += 0.05f;
                beatSpawnContainer[drumIndex].style.scale = new Scale(new Vector2(beatTargetEffects[drumIndex].swellEffect, beatTargetEffects[drumIndex].swellEffect));
            }
            else
            {
                beatTargetEffects[drumIndex].swellStage = 2;
            }
        }
        else if(beatTargetEffects[drumIndex].swellStage == 2)
        {
            if(beatTargetEffects[drumIndex].swellEffect > 1f)
            {
                beatTargetEffects[drumIndex].swellEffect -= 0.05f;
                beatSpawnContainer[drumIndex].style.scale = new Scale(new Vector2(beatTargetEffects[drumIndex].swellEffect, beatTargetEffects[drumIndex].swellEffect));
            }
            else
            {
                beatTargetEffects[drumIndex].swellStage = 0;
            }
        }
        yield return null;
    }

    private void handleBeatTargetSwell()
    {
        for(int i = 0; i < 6; i++)
        {
            StartCoroutine(beatSwellCo(i));

            //Do we need to wait for the coroutines to finish?
        }
    }



    void Update() 
    {
        
        if(Input.GetKey(KeyCode.UpArrow)) {
            Lane1L.AddToClassList("glow-class:glow");
        }

        handleBeatTargetSwell();

    }

        IEnumerator FadeOutCoroutine(Label label) {
            float startF = 1f;
            float endF = 0f;
            float startW = label.resolvedStyle.width;
            float startH = label.resolvedStyle.height;
            float endW = startW*2;
            float endH = startH*2;
            float cTimer = 0f;
            float incrementF = (endF - startF);
            float incrementW = (endW - startW);
            float incrementH = (endH - startH);
            long intervalInTicks = (long)(0.05 * TimeSpan.TicksPerSecond);

            while (cTimer < 1f) {
                cTimer += Time.deltaTime;
                float newOpacity = startF+(incrementF*cTimer);
                float newWidth = startW+(incrementW*cTimer);
                float newHeight = startH+(incrementH*cTimer);
                float newFontSize = label.resolvedStyle.fontSize+1;

                
                label.style.opacity = newOpacity;
                label.style.width = new StyleLength(newWidth);
                label.style.height = new StyleLength(newHeight);
                label.style.fontSize = new StyleLength(newFontSize);
                yield return null;
            }
    }
}
