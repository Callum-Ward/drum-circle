using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using UnityEngine;

public class BeatmapScript : MonoBehaviour
{
    public RhythmSpawner spawner;
    public float timer = 0.0f;
    public float window = 0f;
    public float windowtime = 0.3f;
    public float delay = 2.0f;
    public float inputDelay = 0f;
    private int[] drumInputStrengths;
    private bool hitL = false;
    private bool hitR = false;

    public int glowStage = 2;
    public float glowPower = 5.0f;
    private int treeStage = 0;

    public ScoreManager scoreManager;
    public AudioAnalyser audioAnalyser;
    public AudioManager audioManager;
    public BeatManager beatManager;
    public RhythmSpawner beatSpawner;
    public TreeSpawnerForest treeSpawner;
    public MessageListener messageListener;
    public TutorialScript tutorialScript;
    public BeatUI beatUI;
    public string[] sections;
    public string receivedString;
    private const int beatmapWidth = 10;
    public bool tutorial = false;

    SerialPort data_stream = new SerialPort("COM3", 9600);

    public int playerCount = 3;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatSpawner = GameObject.Find("BeatSpawner").GetComponent<RhythmSpawner>();
        treeSpawner = GameObject.Find("TreeSpawner").GetComponent<TreeSpawnerForest>();
        messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
        tutorialScript = GameObject.Find("TutorialLogic").GetComponent<TutorialScript>();
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>();

        beatManager.setPlayerCount(this.playerCount);
        beatSpawner.setPlayerCount(this.playerCount);
        beatUI.setPlayerCount(this.playerCount);
        treeSpawner.setPlayers(this.playerCount);

        drumInputStrengths = new int[this.playerCount*2];
        
    }

    private void registerHit(int queueIndex, MoveBeat beat)
    {
        scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beat.windowScore));
        beatManager.BeatDelete(queueIndex, true);
        audioManager.FadeIn("drums", "fast");
        beat.dontDelete = true;
    }

    private void registerMiss(int queueIndex, MoveBeat beat)
    {
        scoreManager.Miss();
        audioManager.Play("tapFail", null);
        audioManager.SetActive("drums");
        audioManager.Volume("drums", 0f);
        if (beat.timer >= (delay * 0.85))
        {
            beatManager.BeatDelete(queueIndex, false);
        }
    }

    //Coroutine function for delaying hit-window
    IEnumerator WindowDelay(float time)
    {
        yield return new WaitForSeconds(time);

        window = windowtime;
    }

    public void StartTutorial(bool start) {
        tutorial = start;
    }

    private void handleGlow()
    {
         /*GameObject enaTree = GameObject.Find("tree_afsTREE_xao_xlprl");
            MeshRenderer renderer = enaTree.GetComponent<MeshRenderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.c
            renderer.SetPropertyBlock(block);*/
        GameObject glowingLayer = GameObject.Find("glowingLayer");
        MeshRenderer renderer = glowingLayer.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Shader Graphs/glowing shader"));
        renderer.material.SetFloat("_Power", glowPower);
        
        if(glowStage == 3)
        {
            glowPower += 0.05f;
            if(glowPower >= 5.0f){
                glowStage = 0;
            }
        }
        else if(glowStage == 2)
        {
            glowPower -= 0.05f;
            if(glowPower <= 1.0f){
                glowStage += 1;
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
            if (sections[0] == "on")
            {
                drumInputStrengths[Int32.Parse(sections[1])] = Int32.Parse(sections[3]);
            }
            messageListener.message = null;
        }
    }

    private void checkCorrectDrumHit()
    {
        for(int i = 0; i < playerCount; i++)
            {
                //Register left drum hit and perform code
                if ((drumInputStrengths[i*2] > 0 || Input.GetKeyDown(KeyCode.LeftArrow)))
                    if (beatManager.beatQueues[i * 2].Count > 0) {
                        {
                            var beatL = beatManager.beatQueues[i * 2].Peek().GetComponent<MoveBeat>();
                            beatHit((i*2), beatL);
                            
                            //Trigger start of glow process
                            if(i % 2 == 0 && glowStage <= 1){   
                                glowStage += 1;
                            }
                            
                            treeSpawner.spawnTree(i+1, 2);
                        }
                }

                //Register right drum hit and perform code
                if ((drumInputStrengths[i*2 + 1] > 0 || Input.GetKeyDown(KeyCode.RightArrow)))
                {
                    if (beatManager.beatQueues[i * 2 + 1].Count > 0)
                    {
                        var beatR = beatManager.beatQueues[i * 2 + 1].Peek().GetComponent<MoveBeat>();
                        beatHit((i*2+1), beatR);
                    }
                }
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<RhythmSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        //13
        handleGlow();
        handleDrumInput();

        //Start the timer
        if (timer <= delay && audioManager.activeSource == null && tutorialScript.tutorialComplete == true)
        {
            beatSpawner.spawnOnTime(timer);
            timer += Time.deltaTime;
            beatUI.startLevelUI();
        }
        //Play all layers of music simultaneously
        else if(audioManager.activeSource == null && tutorialScript.tutorialComplete == true)
        {
            audioManager.Play("drums", audioAnalyser);
            audioManager.Play("layer1", null);
            audioManager.Play("layer2", null);
            audioManager.Volume("layer2", 0f);
        }
        //Drum hit functionality
        else if(tutorialScript.tutorialComplete == true)
        {
            if(tutorial == false) {
                beatSpawner.spawnOnTime(audioManager.activeSource.time + delay + inputDelay);
            }

            checkCorrectDrumHit();
        }
    }

    void beatHit(int queueNo, MoveBeat beatSide) {
        if (beatSide.window == true)
            {
                registerHit(queueNo, beatSide);
            }
        else
            {
                registerMiss(queueNo, beatSide);
            }
    }
}

