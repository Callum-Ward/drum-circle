using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BeatmapScript : MonoBehaviour
{
    public RhythmSpawner spawner;
    private float timer = 0.0f;

    private void spawnOnTime(float time)
    {
            int index = (int)(Math.Round(time, 2) * 100);
            List<AudioTimestamp> timestampedOnsets = FindObjectOfType<AudioAnalyser>().activeAnalysis.timestampedOnsets;

            if(index < timestampedOnsets.Count){
                int lb = index == 0 ? 0 : index - 1;
                int ub = index == timestampedOnsets.Count - 1 ? index : index + 1;
                for(int i = lb; i <= ub; i++){
                    if(timestampedOnsets[i].isOnset){
                        spawner.spawn(1, 0);
                        timestampedOnsets[i].isOnset = false;
                    }
                    if(timestampedOnsets[i].isBeat){
                        spawner.spawn(1, 1);
                        timestampedOnsets[i].isBeat = false;
                    }
                }
            } 
    }

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<RhythmSpawner>();
        FindObjectOfType<AudioAnalyser>().loadTrackAnalysis("drakkar");
    }

    // Update is called once per frame
    // void Update()
    // {

    //     if(timer <= 4.0f && FindObjectOfType<AudioManager>().activeSource == null)
    //     {
    //         spawnOnTime(timer);
    //         timer += Time.deltaTime;

    //     }
    //     else if(FindObjectOfType<AudioManager>().activeSource == null)
    //     {
    //         FindObjectOfType<AudioManager>().Play("drakkar");
    //     }
    //     else
    //     {
    //         spawnOnTime(FindObjectOfType<AudioManager>().activeSource.time + 4.0f);
    //     }
    // }
}
