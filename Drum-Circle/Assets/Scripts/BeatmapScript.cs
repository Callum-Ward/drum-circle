using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.SceneManagement;

public class BeatmapScript : MonoBehaviour
{
    public RhythmSpawner spawner;
    [HideInInspector] public float timer = 0.0f;
    [HideInInspector] public float window = 0f;
    public float windowtime = 0.4f;
    public float delay = 2.0f;
    public float inputDelay = 0f;
    [HideInInspector] public float introTimer = 0f;
    public float introDelay = 8f;
    public float beatTargetLocation = 0.3f;
    private int noteNumberOffset = 21;
    private int[] drumInputStrengths;
    private float[] midiInputVelocities;
    private bool hitL = false;
    private bool hitR = false;
    private float[] shakeTimer = new float[3] {0f, 0f, 0f};

    [HideInInspector] public int terrainBeatStage = 1;
    public float glowPower = 5.0f;
    private float glowRate = 1.1f;
    private int treeStage = 0;
    private int treeScoreRatio = 1500;

    private float windIncreaseRate = 0.05f;
    private float windDecreaseRate = 0.001f;
    public float windFactor = 0.1f;

    private FreestyleHandler freestyleHandler;

    [HideInInspector] public ScoreManager scoreManager;
    [HideInInspector] public AudioAnalyser audioAnalyser;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public BeatManager beatManager;
    [HideInInspector] public FireballSpawner fireballSpawner;
    [HideInInspector] public RhythmSpawner beatSpawner;
    [HideInInspector] public TreeSpawning treeSpawner;
    [HideInInspector] public MidiHandler midiHandler;
    [HideInInspector] public Terrain terrain;
    [HideInInspector] public BeatUI beatUI;
    [HideInInspector] public TreeManager treeManager;
    [HideInInspector] public WaypointMover waypointMover;
    [HideInInspector] public string[] sections;
    [HideInInspector] public string receivedString;
    main main;
    private const int beatmapWidth = 10;


    public bool tutorial = false;
    private bool running = false;

    private float beatShareDuration = 10f;
    private float beatShareOnset = 30f;

    public int playerCount = 3;
    public int sceneNumber;

    private bool firstHit = false;

    private LoadScreen loadScreen;
    Dictionary<string, float> scenelength = new Dictionary<string, float>();


    // Assigns and configures key gameplay components upon level initialisation
    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatSpawner = GameObject.Find("BeatSpawner").GetComponent<RhythmSpawner>();
        fireballSpawner = GameObject.Find("FireballSpawner").GetComponent<FireballSpawner>();
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        midiHandler = GameObject.Find("MidiHandler").GetComponent<MidiHandler>();
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>();
        waypointMover = GameObject.Find("platform").GetComponent<WaypointMover>();
        main = GameObject.Find("mainTracker").GetComponent<main>();


        treeManager = GameObject.Find("TreeManager").GetComponent<TreeManager>();
        treeSpawner = GameObject.Find("TreeSpawner").GetComponent<TreeSpawning>();

        this.freestyleHandler = new FreestyleHandler(this.playerCount);

        audioAnalyser.setPlayerCount(this.playerCount);
        beatManager.setPlayerCount(this.playerCount);
        beatSpawner.setPlayerCount(this.playerCount);
        beatUI.setPlayerCount(this.playerCount);

        drumInputStrengths = new int[this.playerCount*2];
        midiInputVelocities = new float[this.playerCount*2];

          terrain.terrainData.wavingGrassSpeed = 0.5f;
        terrain.terrainData.wavingGrassStrength = 0.5f;
        terrain.terrainData.wavingGrassAmount = 0.5f;

        this.freestyleHandler.setScene(this.sceneNumber);

        loadScreen = GameObject.Find("LoadScreen").GetComponent<LoadScreen>();
        loadScreen.LoadScreenFadeOut();
        scenelength.Add("Forest", 209);
        scenelength.Add("Mountains", 283);
        scenelength.Add("Beach", 271);
    }


    // Registers successfull drum hit on-beat
    private void registerHit(int queueIndex, MoveBeatUI beat, int oneShotIndex, float velocity)
    {
        scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beat.windowScore), Mathf.FloorToInt(queueIndex/2));
        beatManager.BeatDelete(queueIndex, true);

        if(freestyleHandler.active() && oneShotIndex > 0)
        {
            audioManager.PlayDrumOneShot(oneShotIndex, velocity);
        }
        else
        {
            audioManager.FadeInDrumTrack(queueIndex / 2, "fast");
        }

        beat.dontDelete = true;
        treeManager.SetHitStatus(true);
    }


    // Registers missed attempted drum hit
    private void registerMiss(int queueIndex, MoveBeatUI beat)
    {
        scoreManager.Miss(Mathf.FloorToInt(queueIndex/2));
        audioManager.VolumeDrumTrack(queueIndex / 2, 0f);
        if (beat.timer >= (delay * 0.85))
        {
            beatManager.BeatDelete(queueIndex, false);
        }
        beatUI.hitMiss((int)(queueIndex / 2), (windowtime / 2) - Mathf.Abs((windowtime / 2) - beat.windowScore));
        treeManager.SetHitStatus(false);
    }


    //Coroutine function for delaying hit-window
    IEnumerator WindowDelay(float time)
    {
        yield return new WaitForSeconds(time);

        window = windowtime;
    }


    // Sets triggers for on-beat enviroment effects
    private void setEnvironmentTriggers(int drumIndex)
    {
        int playerIndex = Mathf.FloorToInt(drumIndex / 2);

        if(drumIndex % 2 == 0 && terrainBeatStage <= 1)
        {   
            terrainBeatStage += 1;
        }
                            
        if(Mathf.Floor(scoreManager.playerScores[playerIndex] / treeScoreRatio) >= treeStage)
        {
            treeStage += 1;
        }

    }


    // Handles terrain manipultation such as glowing effects on beat
    private void handleTerrainBeatResponse()
    {
        try{

            Material newMaterial = new Material(Shader.Find("Shader Graphs/glowing shader"));
            newMaterial.SetFloat("_Power", glowPower);
        } catch {

        }

        terrain.terrainData.wavingGrassStrength = windFactor;
        terrain.terrainData.wavingGrassSpeed = windFactor;
        
        if(terrainBeatStage == 3)
        {
            glowPower += glowRate * 1.5f;
            windFactor -= windDecreaseRate;
            if(windFactor <= 0.1f){
                terrainBeatStage = 1;
            }
        }
        else if(terrainBeatStage == 2)
        {
            windFactor += windIncreaseRate;
            if(windFactor >= 0.8f){
                terrainBeatStage += 1;
            }
        }
    }


    // Checks for successfull drum hit on-beat
    private bool checkCorrectDrumHit(int drumIndex, float velocity)
    {
        if (beatManager.beatQueues[drumIndex].Count > 0) 
        {
            try{
                var beat = beatManager.beatQueues[drumIndex].Peek();
                return beatHit((drumIndex), beat.obj.GetComponent<MoveBeatUI>(), beat.oneShotIndex, velocity);
            } catch (Exception e) {
                Debug.Log(e);
            }
        }
        return false;
    }


    // Checks for drum input from each player from midi handler
    private bool checkDrumHit()
    {
        bool hit = false;
        for(int i = 0; i < this.playerCount; i++)
            {
                //Register left drum hit and perform code
                if ((drumInputStrengths[i*2] > 0 || midiHandler.midiInputVelocities[i*2] > 0.0f || Input.GetKeyDown(KeyCode.LeftArrow)))
                {
                    freestyleHandler.handleDrumHitFreestyle(beatSpawner, audioManager, audioAnalyser, i, 0, midiHandler.midiInputVelocities[i*2], 1.0f);
                    if (checkCorrectDrumHit(i*2, midiHandler.midiInputVelocities[i*2]))
                    {
                        fireballSpawner.spawn(i);
                        //treeSpawner.spawnTree(1, 2, new Color(0f,0f,0f), true );
                        setEnvironmentTriggers(i*2);
                        hit = true;
                    }
                    beatUI.hitSwell(i*2);

                    midiHandler.clearMidiInputVelocities(i * 2);
                }

                //Register right drum hit and perform code
                if ((drumInputStrengths[i*2 + 1] > 0 || midiHandler.midiInputVelocities[i*2 + 1] > 0.0f || Input.GetKeyDown(KeyCode.RightArrow)))
                {
                    freestyleHandler.handleDrumHitFreestyle(beatSpawner, audioManager, audioAnalyser, i, 1, midiHandler.midiInputVelocities[i*2 + 1], 1.0f);
                    if (checkCorrectDrumHit(i*2 + 1, midiHandler.midiInputVelocities[i*2 + 1]))
                    {
                        fireballSpawner.spawn(i);
                        //treeSpawner.spawnTree(1, 2, new Color(0f,0f,0f), true );
                        // Enviroment triggers etc. right drum hit on target
                        hit = true;
                    }
                    beatUI.hitSwell(i*2 + 1);
                    midiHandler.clearMidiInputVelocities(i * 2 + 1);
                }
            }
            return hit;
    }


    // Starts playing ambience track at start of level
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<RhythmSpawner>();
        audioManager.PlayBackgroundTrack();
    }


    // Resets in-game UI
    private void resetUI() {        
        VisualElement ending = GameObject.Find("EndScore").GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Canvas");
        VisualElement beatSpawnUI = GameObject.Find("BeatSpawnUI").GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ScreenContainer");      
        ending.style.opacity = new StyleFloat(0f);
        beatSpawnUI.style.opacity = new StyleFloat(1f);
    }

    // Main update loop, checking for beat hits
    void Update()
    {
        //13
        //handleTerrainBeatResponse();      

        beatUI.startLevelUI();

        //Manually exit level at any time
        if (Input.GetKey(KeyCode.Escape)) {            
            StartCoroutine(sceneSwitch("2MissionSelect"));
        }

        // if (Input.GetKey(KeyCode.Alpha9)) {             
        //     loadScreen.EndScreenFade();       
        // }
        if (Input.GetKey(KeyCode.Alpha8)) {
                resetUI();    
        }

             

        float countdown = introDelay - introTimer;

        //Get current scene, if end of scene's track has finished then display scores and go back to level select if drum is tapped
        Scene scene = SceneManager.GetActiveScene(); 
        if(timer > scenelength[scene.name]) {    
            loadScreen.EndScreenFade();          
            if(timer > scenelength[scene.name]+2) {
                if ((drumInputStrengths[0] > 0 || midiHandler.midiInputVelocities[0] > 0.0f || Input.GetKeyDown(KeyCode.LeftArrow)))
                {
                    loadScreen.endIn = false;
                    midiHandler.clearMidiInputVelocities(0);
                    StartCoroutine(sceneSwitch("2MissionSelect"));                
                }
                else if ((drumInputStrengths[1] > 0 || midiHandler.midiInputVelocities[1] > 0.0f || Input.GetKeyDown(KeyCode.RightArrow)))
                {
                    loadScreen.endIn = false;
                    midiHandler.clearMidiInputVelocities(1);
                    StartCoroutine(sceneSwitch("2MissionSelect"));                
                }
            }
        }    
        

        if (introTimer <= introDelay && !running)
        {
            introTimer += Time.deltaTime;
            resetUI();     
        }
        else if(!running)
        {
            running = true;
            timer = 0f;
        }

        if(running)
        {
            if(timer < delay)
            {
                int queueIndex  = beatSpawner.spawnOnTime(timer + inputDelay);
                checkDrumHit();
                resetUI();                
                main.getTracker();
            }
            //Play all layers of music simultaneously
            else if(audioManager.activeSources.Count <= 1)
            {
                audioManager.PlayAllDrumTracks();
                audioManager.PlayAllLayerTracks(0.75f);
            }
            //Drum hit functionality
            else
            {
                float averagedTime = ((audioManager.activeSources[1].time + delay + timer) / 2f) + inputDelay;
                int queueIndex  = beatSpawner.spawnOnTime(averagedTime);
                bool hit = checkDrumHit();
                if(hit && !firstHit)
                {
                    waypointMover.startMove();
                    firstHit = true;
                }
                freestyleHandler.handleFreestyle(beatSpawner, beatUI, audioManager, averagedTime);
            
            }
            timer += Time.deltaTime;  
        }

        //Intro timer functionality
        if(countdown <= 5 && countdown > 4) {
            
            resetUI();     
            beatUI.IntroTimerStart();
        }
        else if (introDelay - introTimer <= 4 && countdown > 0) {
            beatUI.IntroTimerUpdate(countdown);
            
                resetUI();     
        }
        else if (countdown <= 0) {
            beatUI.IntroTimerStop();
            introTimer = 10f;
        }      
    }


    // Switches game scene
    IEnumerator sceneSwitch(string mission) {
        loadScreen.LoadScreenFadeIn();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(mission);
    }


    // Handles beat hit, determining if its a hit or a miss
    bool beatHit(int queueNo, MoveBeatUI beatSide, int oneShotIndex, float velocity) {
        if (beatSide.window == true)
        {
            registerHit(queueNo, beatSide, oneShotIndex, velocity);
            return true;
        }
        else if (freestyleHandler.checkMiss(queueNo))
        {
            registerMiss(queueNo, beatSide);
            return false;
        }
        else
        {
            registerMiss(queueNo, beatSide);
            return false;
        }
    }
}

