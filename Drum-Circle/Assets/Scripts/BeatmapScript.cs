using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BeatmapScript : MonoBehaviour
{
    public RhythmSpawner spawner;
    public float timer = 0.0f;
    public float window = 0f;
    private float windowtime = 0.3f;

    public ScoreManager scoreManager;
    public AudioAnalyser audioAnalyser;
    public AudioManager audioManager;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    private void spawnOnTime(float time)
    {
            int index = (int)(Math.Round(time, 2) * 100);
            List<AudioTimestamp> timestampedOnsets = audioAnalyser.activeAnalysis.timestampedOnsets;

            if(index < timestampedOnsets.Count){
                int lb = index == 0 ? 0 : index - 1;
                int ub = index == timestampedOnsets.Count - 1 ? index : index + 1;
                for(int i = lb; i <= ub; i++){
                    if(timestampedOnsets[i].isBeat){
                        spawner.spawn(1, 1);
                    }
                    if(timestampedOnsets[i].isOnset){
                        spawner.spawn(1, 0);
                        timestampedOnsets[i].isOnset = false;
                    }
                }
            } 
    }

    private void hitWindow(float time)
    {
        int index = (int)(Math.Round(time, 2) * 100);
        List<AudioTimestamp> timestampedOnsets = audioAnalyser.activeAnalysis.timestampedOnsets;

        if (index < timestampedOnsets.Count)
        {
            int lb = index == 0 ? 0 : index - 1;
            int ub = index == timestampedOnsets.Count - 1 ? index : index + 1;
            for (int i = lb; i <= ub; i++)
            {
                if (timestampedOnsets[i].isOnset)
                {
                    window = windowtime;
                }
                if (timestampedOnsets[i].isBeat)
                {
                    window = windowtime;
                }
            }
        }
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
        Debug.Log("Window: " + window);
        if (timer <= 4.0f && audioManager.activeSource == null)
        {
            spawnOnTime(timer);
            timer += Time.deltaTime;
        }
        else if(audioManager.activeSource == null)
        {
<<<<<<< HEAD
            FindObjectOfType<AudioManager>().Play("drums");
=======
            audioManager.Play("drums");
>>>>>>> music-testing
        }
        else
        {
            spawnOnTime(audioManager.activeSource.time + 4.0f + (windowtime / 2));
            hitWindow(audioManager.activeSource.time + windowtime);

            if (window <= 0f && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                audioManager.Play("tapFail");
                audioManager.SetActive("drums");
                scoreManager.Miss();
            }
            else if (window > 0f)
            {
                window -= Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    scoreManager.Hit(windowtime / 2 - Mathf.Abs((windowtime / 2) - window));
                }
            }
        }
    }
}
