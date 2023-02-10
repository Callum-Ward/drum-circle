using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BeatmapScript : MonoBehaviour
{
    public RhythmSpawner spawner;
    public float timer = 0.0f;
    public float window = 0f;
    public float windowtime = 0.3f;
    public float delay = 2.0f;

    public ScoreManager scoreManager;
    public AudioAnalyser audioAnalyser;
    public AudioManager audioManager;
    public BeatManager beatManager;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
    }

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
                        StartCoroutine(WindowDelay(delay - windowtime/2));
                        spawner.spawn(1, 1);
                        timestampedOnsets[i].isBeat = false;
                        break;
                    }
                    if(timestampedOnsets[i].isOnset)
                        {
                        StartCoroutine(WindowDelay(delay - windowtime/2));
                        spawner.spawn(1, 0); 
                        timestampedOnsets[i].isOnset = false;
                        break;
                    }
                }
            } 
    }

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
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= delay && audioManager.activeSource == null)
        {
            spawnOnTime(timer);
            timer += Time.deltaTime;
        }
        else if(audioManager.activeSource == null)
        {
            audioManager.Play("drums");
            //audioManager.Play("layer1");
        }
        else
        {
            spawnOnTime(audioManager.activeSource.time + delay);
            
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                if (beatManager.beatQueueL.Count > 0) {
                    {
                        var beatL = beatManager.beatQueueL.Peek().GetComponent<MoveBeat>();
                        if (beatL.window == true)
                        {
                            scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beatL.windowScore));
                            beatManager.BeatDelete("left");
                            audioManager.Volume("drums", 1f);
                        }
                        else
                        {
                            scoreManager.Miss();
                            //audioManager.Play("tapFail");
                            audioManager.SetActive("drums");
                            audioManager.Volume("drums", 0f);
                        }
                    }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (beatManager.beatQueueR.Count > 0)
                {
                    var beatR = beatManager.beatQueueR.Peek().GetComponent<MoveBeat>();
                    if (beatR.window == true)
                    {
                        scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beatR.windowScore));
                        beatManager.BeatDelete("right");
                        audioManager.Volume("drums", 1f);
                    }
                    else
                    {
                        scoreManager.Miss();
                        audioManager.Play("tapFail");
                        audioManager.SetActive("drums");
                        audioManager.Volume("drums", 0f);
                    }
                }
            }
        }
    }
}

