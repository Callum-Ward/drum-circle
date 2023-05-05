using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.SceneManagement;

public class BeatmapScript : MonoBehaviour
{
    public RhythmSpawner spawner;
    [HideInInspector] public float timer = 0.0f;
    [HideInInspector] public float window = 0f;
    public float windowtime = 0.3f;
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
    
    public bool useMidiFile;

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
    [HideInInspector] public MessageListener messageListener;
    [HideInInspector] public Terrain terrain;
    [HideInInspector] public TutorialScript tutorialScript;
    [HideInInspector] public BeatUI beatUI;
    [HideInInspector] public TreeManager treeManager;
    [HideInInspector] public WaypointMover waypointMover;
    [HideInInspector] public string[] sections;
    [HideInInspector] public string receivedString;
    private const int beatmapWidth = 10;


    public bool tutorial = false;
    private bool running = false;

    private float beatShareDuration = 10f;
    private float beatShareOnset = 30f;

    public int playerCount = 3;
    public int sceneNumber;

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
        messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
        tutorialScript = GameObject.Find("TutorialLogic").GetComponent<TutorialScript>();
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>();
        waypointMover = GameObject.Find("platform").GetComponent<WaypointMover>();

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
    }

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
            //treeSpawner.spawnTree(playerIndex, 2, new Color(0, 0, 0), true);
        }

    }

    private void handleTerrainBeatResponse()
    {
         /*GameObject enaTree = GameObject.Find("tree_afsTREE_xao_xlprl");
            MeshRenderer renderer = enaTree.GetComponent<MeshRenderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.c
            renderer.SetPropertyBlock(block);*/
        try{
            MeshRenderer renderer;

            //Material newMaterial = new Material(Shader.Find("Shader Graphs/glowing shader"));
            //newMaterial.SetFloat("_Power", glowPower);

           /* IEnumerable<GameObject> glowingLayers = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "branch");
            foreach(GameObject obj in glowingLayers)
            {
                renderer = obj.GetComponent<MeshRenderer>();
                renderer.material.SetFloat("_Power", glowPower);
            }*/

            IEnumerable<GameObject> glowingLayers = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "glowingLayer");
            foreach(GameObject obj in glowingLayers)
            {
                renderer = obj.GetComponent<MeshRenderer>();
                renderer.material.SetFloat("_Power", glowPower);
            }
        } catch {

        }

        terrain.terrainData.wavingGrassStrength = windFactor;
        terrain.terrainData.wavingGrassSpeed = windFactor;
        //terrain.terrainData.wavingGrassAmount = windFactor;
        
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

    private void handleDrumInput()
    {
        
        //Checks if there was an input in the data stream
        for(int i = 0; i < playerCount*2; i++)
        {
            drumInputStrengths[i] = 0;
        }
        
        string message = messageListener.message;
        if (message != null)
        {
            sections = message.Split(":");
           //Debug.Log(message);
            if (sections.Length > 1)
            {
                drumInputStrengths[Int32.Parse(sections[0])] = Int32.Parse(sections[1]);
            }
            messageListener.message = null;
        }
    }

    private bool checkCorrectDrumHit(int drumIndex, float velocity)
    {
        if (beatManager.beatQueues[drumIndex].Count > 0) 
        {
            try{
                var beat = beatManager.beatQueues[drumIndex].Peek();
                beatHit((drumIndex), beat.obj.GetComponent<MoveBeatUI>(), beat.oneShotIndex, velocity);
                return true;
            } catch {
            }
        }
        return false;
    }

    private void checkDrumHit()
    {
        for(int i = 0; i < this.playerCount; i++)
            {
                //Register left drum hit and perform code
                if ((drumInputStrengths[i*2] > 0 || midiHandler.midiInputVelocities[i*2] > 0.0f || Input.GetKeyDown(KeyCode.LeftArrow)))
                {
                    if (checkCorrectDrumHit(i*2, midiHandler.midiInputVelocities[i*2]))
                    {
                        fireballSpawner.spawn(i);
                        setEnvironmentTriggers(i*2);
                    }
                    beatUI.hitSwell(i*2);

                    freestyleHandler.handleDrumHitFreestyle(beatSpawner, audioManager, audioAnalyser, i, 0, midiInputVelocities[i*2], 1.0f);
                    midiHandler.clearMidiInputVelocities(i * 2);
                }

                //Register right drum hit and perform code
                if ((drumInputStrengths[i*2 + 1] > 0 || midiHandler.midiInputVelocities[i*2 + 1] > 0.0f || Input.GetKeyDown(KeyCode.RightArrow)))
                {
                    if (checkCorrectDrumHit(i*2 + 1, midiHandler.midiInputVelocities[i*2 + 1]))
                    {
                        fireballSpawner.spawn(i);
                        // Enviroment triggers etc. right drum hit on target
                    }
                    beatUI.hitSwell(i*2 + 1);
                    freestyleHandler.handleDrumHitFreestyle(beatSpawner, audioManager, audioAnalyser, i, 1, midiInputVelocities[i*2 + 1], 1.0f);
                    midiHandler.clearMidiInputVelocities(i * 2 + 1);
                }
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<RhythmSpawner>();
        audioManager.PlayBackgroundTrack();
    }

    // Update is called once per frame
    void Update()
    {
        //13
        //handleTerrainBeatResponse();
        handleDrumInput();       

        beatUI.startLevelUI();

        float countdown = introDelay - introTimer;

        if(timer > audioManager.longestTime+8 && audioManager.longestTime != 0 ) {                  
            SceneManager.LoadScene("2MissionSelect");
        }

        if (introTimer <= introDelay && !running)
        {
            introTimer += Time.deltaTime;
        }
        else if(!running)
        {
            running = true;
            waypointMover.startMove();
        }

        if(running)
        {
            if(timer < delay)
            {
                int queueIndex  = beatSpawner.spawnOnTime(timer + inputDelay, useMidiFile);
                checkDrumHit();
            }
            //Play all layers of music simultaneously
            else if(audioManager.activeSources.Count <= 1)
            {
                audioManager.PlayDrumTrack(0);
                //audioManager.PlayAllLayerTracks();
            }
            //Drum hit functionality
            else
            {
                int queueIndex  = beatSpawner.spawnOnTime(timer + delay + inputDelay, useMidiFile);
                
                checkDrumHit();
                freestyleHandler.handleFreestyle(beatSpawner, beatUI, audioManager, audioManager.activeSources[1].time);
            
            }
        }

        if(countdown <= 5 && countdown > 4) {
            beatUI.IntroTimerStart();
        }
        else if (introDelay - introTimer <= 4 && countdown > 0) {
            beatUI.IntroTimerUpdate(countdown);
        }
        else if (countdown <= 0) {
            beatUI.IntroTimerStop();
            introTimer = 10f;
        }  
        timer += Time.deltaTime;          
    }


    void beatHit(int queueNo, MoveBeatUI beatSide, int oneShotIndex, float velocity) {
        if (beatSide.window == true)
        {
            registerHit(queueNo, beatSide, oneShotIndex, velocity);
        }
        else if (freestyleHandler.checkMiss(queueNo))
        {
            registerMiss(queueNo, beatSide);
        }
        else
        {
            registerMiss(queueNo, beatSide);
        }
    }
}

