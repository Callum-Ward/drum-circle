using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    public GameObject[] popups;
    private float[] popupDurations = {4.0f, 4.0f, 4.0f};
    private GameObject activePopup;

    private GameObject activeFeedbackMessage;

    private int activePopupIndex = -1;

    private bool spawnFlag;
    private int stage = 0;
    private int subStage = 0;
    private float currentTimeLimit = 2.0f;

    private String[] feedback = {"atrocious", "bad", "good", "excellent"};

    public bool tutorialComplete;

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
    public string[] sections;
    public string receivedString;
    private const int beatmapWidth = 10;

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
        audioManager.Play("tapFail");
        audioManager.SetActive("drums");
        audioManager.Volume("drums", 0f);
        //audioManager.FadeOut("drums");
        if (beat.timer >= (delay * 0.75))
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

    private void handleFeedbackMessage(int n)
    {
        if(activeFeedbackMessage != null)
        {
            if(activeFeedbackMessage)
            {
                
            }
        }
    }

    private void handleFirstStage()
    {
        if(spawnFlag)
        {
            beatSpawner.spawn(subStage + 1, 1, 1);
            currentTimeLimit = popupDurations[subStage];
            spawnFlag = false;
            subStage++;
        }
        else
        {
            if(subStage < playerCount)
            {
                spawnFlag = true;
            }
            else
            {
                Destroy(activePopup);
                stage++;
                return;
            }

            if(activePopup != null)
            {
                Destroy(activePopup);
            }
            transform.position = Camera.main.transform.position + new Vector3(0f,2.5f,4f);
            Vector3 textPosition = transform.position + new Vector3(-1.5f, -0.5f, -0.11f);
            activePopup = Instantiate(popups[subStage], textPosition, transform.rotation);

            currentTimeLimit = 2.0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<RhythmSpawner>();
       
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

        stage = 2;
        tutorialComplete = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= currentTimeLimit)
        {
            timer += Time.deltaTime;
        }
        else if(stage == 0)
        {
            handleFirstStage();
            timer = 0.0f;
        }
        else
        {
            tutorialComplete = true;
        }
    }

}

