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
    public float inputDelay = 0.1f;
<<<<<<< HEAD
    private bool hitL = false;
    private bool hitR = false;
=======
>>>>>>> 8f529fa0 (Added new song and input delay compensation)

    public ScoreManager scoreManager;
    public AudioAnalyser audioAnalyser;
    public AudioManager audioManager;
    public BeatManager beatManager;
    public string[] sections;
    public string receivedString;

    SerialPort data_stream = new SerialPort("COM8", 9600);

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
    }

    //Function for spawning beats based on passed variable
    private void spawnOnTime(float time)
    {
            int index = (int)(Math.Round(time, 2) * 100);
            List<AudioTimestamp> timestampedOnsets = audioAnalyser.activeAnalysis.timestampedOnsets;
    
            if(index < timestampedOnsets.Count){
                int lb = index == 0 ? 0 : index - 1;
                int ub = index == timestampedOnsets.Count - 1 ? index : index + 1;
                for(int i = lb; i <= ub; i++){
                    if(timestampedOnsets[i].isBeat)
                    {
<<<<<<< HEAD
                        int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;
                        StartCoroutine(WindowDelay(delay - windowtime/2));
                        spawner.spawn(1, 1, size);
=======
                        StartCoroutine(WindowDelay(delay + inputDelay - windowtime/2));
                        spawner.spawn(1, 1);
>>>>>>> 8f529fa0 (Added new song and input delay compensation)
                        timestampedOnsets[i].isBeat = false;
                        break;
                    }
                    if(timestampedOnsets[i].isOnset)
                        {
<<<<<<< HEAD
                        int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;    
                        StartCoroutine(WindowDelay(delay - windowtime/2));
                        spawner.spawn(1, 0, size); 
=======
                        StartCoroutine(WindowDelay(delay + inputDelay - windowtime/2));
                        spawner.spawn(1, 0); 
>>>>>>> 8f529fa0 (Added new song and input delay compensation)
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
        try
        {
            receivedString = data_stream.ReadLine();
            sections = receivedString.Split(":");
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
        }
        catch (System.Exception e)
        {
            //print(e.ToString());

        }

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
<<<<<<< HEAD

            //Register left drum hit and perform code
            if ((hitL == true || Input.GetKeyDown(KeyCode.LeftArrow)))
=======
            
            //if (Input.GetKeyDown(KeyCode.LeftArrow))
            if (hitL == true || Input.GetKeyDown(KeyCode.LeftArrow))
>>>>>>> 8f529fa0 (Added new song and input delay compensation)
                if (beatManager.beatQueueL.Count > 0) {
                    {
                        var beatL = beatManager.beatQueueL.Peek().GetComponent<MoveBeat>();
                        if (beatL.window == true)
                        {
                            scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beatL.windowScore));
                            //audioManager.Volume("drums", 1f);
                            beatManager.BeatDelete("left", true);
                            audioManager.FadeIn("drums", "fast");
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
            }

            //Register right drum hit and perform code
            if ((hitR == true || Input.GetKeyDown(KeyCode.RightArrow)))
            {
                if (beatManager.beatQueueR.Count > 0)
                {
                    var beatR = beatManager.beatQueueR.Peek().GetComponent<MoveBeat>();
                    if (beatR.window == true)
                    {
                        scoreManager.Hit((windowtime / 2) - beatR.windowScore);
                        //audioManager.Volume("drums", 1f);
                        beatManager.BeatDelete("right", true);
                        audioManager.FadeIn("drums", "fast");
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
            }
        }
    }
}

