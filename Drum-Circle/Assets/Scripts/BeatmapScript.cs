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
    public MessageListener messageListener;
    public string[] sections;
    public string receivedString;
    private const int beatmapWidth = 10;

    SerialPort data_stream = new SerialPort("COM8", 19200);

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
        // holdDownL = new Queue();
        // holdDownR = new Queue();
    }

    //Function for spawning beats based on passed variable
    private void spawnOnTime(float time)
    {
            int index = (int)(Math.Round(time, 2) * 100);
            List<AudioTimestamp> timestampedOnsets = audioAnalyser.activeAnalysis.timestampedOnsets;
    
            if(index < timestampedOnsets.Count){
                int lb = index  < beatmapWidth ? 0 : index - beatmapWidth;
                int ub = index  >= timestampedOnsets.Count - beatmapWidth ? timestampedOnsets.Count - 1 : index + beatmapWidth;
                for(int i = lb; i <= ub; i++)
                {
                    if(timestampedOnsets[i].isBeat)
                    {
                        int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;
                        StartCoroutine(WindowDelay(delay - windowtime/2));
                        spawner.spawn(1, 1, size);
                        timestampedOnsets[i].isBeat = false;
                        break;
                    }
                    if(timestampedOnsets[i].isOnset)
                        {
                        int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;    
                        StartCoroutine(WindowDelay(delay - windowtime/2));
                        spawner.spawn(1, 0, size); 
                        timestampedOnsets[i].isOnset = false;
                        break;
                    }
                }
            } 
    }

    //Coroutine function for delaying hit-window
    IEnumerator WindowDelay(float time)
    {
        yield return new WaitForSeconds(time);

        window = windowtime;
    }


    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<RhythmSpawner>();
        FindObjectOfType<AudioAnalyser>().loadTrackAnalysis("drums");
       
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
        if (timer <= delay && audioManager.activeSource == null)
        {
            spawnOnTime(timer);
            timer += Time.deltaTime;
        }
        //Play all layers of music simultaneously
        else if(audioManager.activeSource == null)
        {
            audioManager.Play("drums");
            audioManager.Play("layer1");
            audioManager.Play("layer2");
            audioManager.Volume("layer2", 0f);
        }
        //Drum hit functionality
        else
        {
            spawnOnTime(audioManager.activeSource.time + delay + inputDelay);

            //Register left drum hit and perform code
            // if ((hitL == true || Input.GetKeyDown(KeyCode.LeftArrow)) && holdDownL.Count == 0)
            if ((hitL == true || Input.GetKeyDown(KeyCode.LeftArrow)))
                if (beatManager.beatQueueL.Count > 0) {
                    {
                        var beatL = beatManager.beatQueueL.Peek().GetComponent<MoveBeat>();
                        if (beatL.window == true)
                        {
                            scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beatL.windowScore));
                            //audioManager.Volume("drums", 1f);
                            beatManager.BeatDelete("left", true);
                            audioManager.FadeIn("drums", "fast");
                            beatL.dontDelete = true;
                        }
                        else
                        {
                            scoreManager.Miss();
                            audioManager.Play("tapFail");
                            audioManager.SetActive("drums");
                            audioManager.Volume("drums", 0f);
                            //audioManager.FadeOut("drums");
                            if (beatL.timer >= (delay * 0.75))
                            {
                                beatManager.BeatDelete("left", false);
                            }
                        }
                    }
                // holdDownL.Enqueue("1");
                // holdDownL.Enqueue("1");
            }

            //Register right drum hit and perform code
            // if ((hitR == true || Input.GetKeyDown(KeyCode.RightArrow)) && holdDownR.Count == 0)
            if ((hitR == true || Input.GetKeyDown(KeyCode.RightArrow)))
            {
                if (beatManager.beatQueueR.Count > 0)
                {
                    var beatR = beatManager.beatQueueR.Peek().GetComponent<MoveBeat>();
                    if (beatR.window == true)
                    {
                        scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beatR.windowScore));
                        //audioManager.Volume("drums", 1f);
                        beatManager.BeatDelete("right", true);
                        audioManager.FadeIn("drums", "fast");
                        beatR.dontDelete = true;
                    }
                    else
                    {
                        scoreManager.Miss();
                        audioManager.Play("tapFail");
                        audioManager.SetActive("drums");
                        audioManager.Volume("drums", 0f);
                        //audioManager.FadeOut("drums");
                        if (beatR.timer >= (delay * 0.75))
                        {
                            beatManager.BeatDelete("right", false);
                        } 
                    }
                }
                // holdDownR.Enqueue("1");
                // holdDownR.Enqueue("1");
            }
        }
    }
}

