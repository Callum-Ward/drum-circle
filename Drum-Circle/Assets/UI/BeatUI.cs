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
    ScoreManager scoreManager;
    VisualElement freestyleUI;

    TemplateContainer[] beatSpawnContainer = new TemplateContainer[6];

    public VisualTreeAsset beatSpawnTemplate;
    UIDocument beatSpawnUI;

    float test = 0;
    int ctest = 0;
    int mtest = 0;

    int counter = 0;

    private GUIStyle guiStyle = new GUIStyle();

    private Color[] targetColors = new Color[3]{new Color(0.84f, 0.34f, 0.09f), new Color(0.11f, 0.68f, 0.94f), new Color(0.02f, 0.21f, 0.02f)};

    private UITargetEffect[] beatTargetEffects = new UITargetEffect[6];
    private UILaneEffect[] laneEffects = new UILaneEffect[3];
    private int[] drumIndexUIMap = new int[6]{0, 3, 1, 4, 2, 5};

    
    private void OnEnable()
    {
        root = GameObject.Find("BeatSpawnUI").GetComponent<UIDocument>().rootVisualElement;
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        freestyleUI = GameObject.Find("FreestyleUI").GetComponent<UIDocument>().rootVisualElement;

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

        beatmapScript = GameObject.Find("RhythmLogic").GetComponent<BeatmapScript>();
        beatTargetLocation = beatmapScript.beatTargetLocation;

        playerTags = new Label[] {playerTag1, playerTag2, playerTag3};
        scoreTags = new Label[] {scoreTag1, scoreTag2, scoreTag3};
        comboTags = new Label[] {comboTag1, comboTag2, comboTag3};
        tags = new Label[][] {playerTags, scoreTags, comboTags};
        laneContainers = new VisualElement[] {lane1Container, lane2Container, lane3Container};


        playerLanes = new VisualElement[] {Lane1, Lane2, Lane3};
        lanes = new VisualElement[] {Lane1L, Lane2L, Lane3L, Lane1R, Lane2R, Lane3R};

        for(int i = 0; i < 6; i++){
            beatTargetEffects[i] = new UITargetEffect();
        }

        for(int i = 0; i < 3; i++){
            laneEffects[i] = new UILaneEffect();
        }


        beatSpawnUI = GetComponent<UIDocument>();

    }

    public void setPlayerCount(int number) {
        playerNo = number;
    }

    public void updateScore(int player, float scoreVal, int comboVal, int multiVal, int winningPlayer) {
        scoreTags[player].text = "Score: " + scoreVal;
        comboTags[player].text = "Combo: " + comboVal + "\nx" + multiVal;
        if(comboVal > 0)
        {
            laneEffects[player].SetMode("combo", (float)comboVal);
            if(comboVal <= 1)
            {   
                laneContainers[player].style.backgroundColor = laneEffects[player].getTransitionColor(laneContainers[player].style.backgroundColor.value);
            }
        }
       /* if(winningPlayer == player && scoreVal > 100f)
        {
            beatTargetEffects[player*2].SetMode("winning");
            beatTargetEffects[player*2 + 1].SetMode("winning");
        }
        else
        {
            beatTargetEffects[player*2].SetMode("none");
            beatTargetEffects[player*2 + 1].SetMode("none");
        }*/
    }

    public void hitSwell(int drumIndex) {
        beatTargetEffects[drumIndexUIMap[drumIndex]].SetMode("swell");
    }

    public void hitMiss(int playerIndex, float windowScore) {
        laneEffects[playerIndex].SetMode("miss", 4f);
        laneContainers[playerIndex].style.backgroundColor = laneEffects[playerIndex].getTransitionColor(laneContainers[playerIndex].style.backgroundColor.value);
    }

    public void toggleFreestyle(int playerIndex, bool freestyle)
    {
        if(freestyle)
        {
            laneEffects[playerIndex].SetMode("freestyle", 4f);
            laneContainers[playerIndex].style.backgroundColor = laneEffects[playerIndex].getTransitionColor(laneContainers[playerIndex].style.backgroundColor.value);
        }
        else
        {
            laneEffects[playerIndex].SetMode("none", 0f);
            laneContainers[playerIndex].style.backgroundColor = laneEffects[playerIndex].getTransitionColor(laneContainers[playerIndex].style.backgroundColor.value);
        }
    }

    // Update is called once per frame
    public void startLevelUI()
    {     
        
        float targetSize = Lane1L.resolvedStyle.width/4;
        if (start < 2)   
        {
            screenWidth = root.Q<VisualElement>("ScreenContainer").resolvedStyle.width;
            screenHeight = root.Q<VisualElement>("ScreenContainer").resolvedStyle.height;

            if(playerNo > 3 || playerNo < 1) {
                Debug.Log("invalid player count");
            }
            else {
                for (int i = 0; i < playerNo; i++) {
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
                    laneContainers[i].style.backgroundColor = new Color(0f, 0f, 0f, 0.3f);
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

    public void FreestyleUIStart(int PlayerNo) {
        string player = "P" + (PlayerNo + 1).ToString() + "Free";
        freestyleUI.Q<Label>(player).style.opacity = 1f;
    }

    public void FreestyleOtherPlayer(int PlayerNo) {
        string player = "P" + (PlayerNo + 1).ToString() + "Free";
        freestyleUI.Q<Label>(player).text = "Player " + PlayerNo.ToString() + "\n respond";
        freestyleUI.Q<Label>(player).style.opacity = 1f;
    }

    public void FreestyleTimerStart(int PlayerNo, float time) {
        string player = "P" + (PlayerNo + 1).ToString() + "Free";
        freestyleUI.Q<Label>(player).text = Mathf.Ceil(time).ToString();
    }

    public void FreestyleTimerStop(int PlayerNo) {
        string player = "P" + (PlayerNo + 1).ToString() + "Free";
        freestyleUI.Q<Label>(player).text = "Play!";
        StartCoroutine(FadeOutCoroutine(freestyleUI.Q<Label>(player)));
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

    private IEnumerator PlayerUIEffectCo(int playerIndex)
    {
        Color nextLaneColor = laneEffects[playerIndex].getNextGlowValue(laneContainers[playerIndex].style.backgroundColor.value);
        float nextShakeValue = laneEffects[playerIndex].getNextShakeValue();

        laneContainers[playerIndex].style.backgroundColor = new StyleColor(nextLaneColor);
        laneContainers[playerIndex].style.left = nextShakeValue;

        for(int i = 0; i < 2; i++)
        {
            Color nextBeatTargetColor = beatTargetEffects[drumIndexUIMap[playerIndex * 2 + i]].getNextGlowValue(beatSpawnContainer[playerIndex * 2 + i].style.backgroundColor.value);
            float nextScaleValue = beatTargetEffects[drumIndexUIMap[playerIndex * 2 + i]].getNextScaleEffect();

            //beatSpawnContainer[drumIndexUIMap[playerIndex * 2 + i]].style.backgroundColor = new StyleColor(nextBeatTargetColor);
            beatSpawnContainer[drumIndexUIMap[playerIndex * 2 + i]].style.scale = new Scale(new Vector2(nextScaleValue, nextScaleValue));
        }
        yield return null;
    }


    void Update() 
    {
        
        if(Input.GetKey(KeyCode.Alpha1)) {
            FreestyleUIStart(0);
        }
        if(Input.GetKey(KeyCode.Alpha2)) {
            FreestyleTimerStart(0, 16);
        }
        if(Input.GetKey(KeyCode.Alpha3)) {
            FreestyleTimerStop(0);
        }

        if(start == 2)
        {
            for(int i = 0; i < 6; i++)
            {
                if(i < 3)
                {
                    StartCoroutine(PlayerUIEffectCo(i));
                }
            //Do we need to wait for the coroutines to finish?
            }
        }

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
