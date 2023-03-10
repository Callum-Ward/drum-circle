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
    private bool hitL = false;
    private bool hitR = false;
    // private Queue holdDownL;
    // private Queue holdDownR;

    public ScoreManager scoreManager;
    public AudioAnalyser audioAnalyser;
    public AudioManager audioManager;
    public BeatManager beatManager;
    public RhythmSpawner beatSpawner;
    public MessageListener messageListener;
    public TutorialScript tutorialScript;
    public string[] sections;
    public string receivedString;

    SerialPort data_stream = new SerialPort("COM3", 19200);

    public int playerCount = 3;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatSpawner = GameObject.Find("BeatSpawner").GetComponent<RhythmSpawner>();
        messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
        tutorialScript = GameObject.Find("TutorialLogic").GetComponent<TutorialScript>();

        beatManager.setPlayerCount(this.playerCount);
        beatSpawner.setPlayerCount(this.playerCount);

        // holdDownL = new Queue();
        // holdDownR = new Queue();
    }

    private void registerHit(int queueIndex, MoveBeat beat)
    {
        scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beat.windowScore));
        //audioManager.Volume("drums", 1f);
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
        //audioManager.FadeOut("drums");
        if (beat.timer >= (delay * 0.75))
        {
            beatManager.BeatDelete(queueIndex, false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {  
        //Opens the data stream for the connected drums
        try
        {
            data_stream.Open();
            data_stream.ReadTimeout = 10;
        }
        catch (System.Exception ex)
        {
            print(ex.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if there was an input in the data stream
        hitL = false;
        hitR = false;
        
        string message = messageListener.message;
        if (message != null)
        {
            sections = message.Split(":");
            if (sections[0] == "on")
            {
                if (sections[1] == "1")
                {
                    hitL = true;
                }
                else if (sections[1] == "0")
                {
                    hitR= true;
                }
                
            }
            messageListener.message = null;
        }
        
        // if(hitL == false && holdDownL.Count != 0) {
        //     holdDownL.Dequeue();
        // }
        // if(hitR == false && holdDownR.Count != 0) {
        //     holdDownR.Dequeue();
        // }

        //Start the timer
        if (timer <= delay && audioManager.activeSource == null && tutorialScript.tutorialComplete == true)
        {
            beatSpawner.spawnOnTime(timer);
            timer += Time.deltaTime;
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
            beatSpawner.spawnOnTime(audioManager.activeSource.time + delay + inputDelay);

            for(int i = 0; i < playerCount; i++)
            {
                //Register left drum hit and perform code
                // if ((hitL == true || Input.GetKeyDown(KeyCode.LeftArrow)) && holdDownL.Count == 0)
                if ((hitL == true || Input.GetKeyDown(KeyCode.LeftArrow)))
                    if (beatManager.beatQueues[i * 2].Count > 0) {
                        {
                            var beatL = beatManager.beatQueues[i * 2].Peek().GetComponent<MoveBeat>();
                            if (beatL.window == true)
                            {
                                registerHit(i * 2, beatL);
                            }
                            else
                            {
                                registerMiss(i * 2, beatL);
                            }
                        }
                    // holdDownL.Enqueue("1");
                    // holdDownL.Enqueue("1");
                }

                //Register right drum hit and perform code
                // if ((hitR == true || Input.GetKeyDown(KeyCode.RightArrow)) && holdDownR.Count == 0)
                if ((hitR == true || Input.GetKeyDown(KeyCode.RightArrow)))
                {
                    if (beatManager.beatQueues[i * 2 + 1].Count > 0)
                    {
                        var beatR = beatManager.beatQueues[i * 2 + 1].Peek().GetComponent<MoveBeat>();
                        if (beatR.window == true)
                        {
                            registerHit(i * 2 + 1, beatR);
                        }
                        else
                        {
                            registerMiss(i * 2 + 1, beatR);
                        }
                    }
                    // holdDownR.Enqueue("1");
                    // holdDownR.Enqueue("1");
                }
            }
        }
    }
}

