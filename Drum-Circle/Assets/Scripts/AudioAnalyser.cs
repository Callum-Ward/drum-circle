using UnityEngine.Audio;
using System;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AudioTimestamp {
    public bool isOnset;
    public bool isBeat;
}

public class AudioAnalyser : MonoBehaviour {

    private List<AudioTimestamp> activeSourceTimestamps;

    // Awake is called before the Start method
    void Awake()
    {
        List<AudioTimestamp> timestamps;
        using (StreamReader r = new StreamReader("/Users/patcH/Documents/repos/drum-circle/Audio/Dataset.json"))  
        {  
            string json = r.ReadToEnd();  
            timestamps = JsonConvert.DeserializeObject<List<AudioTimestamp>>(json);  
        }  
        this.activeSourceTimestamps = timestamps;
    }
    public void Update()
    {
        int index = (int)(Math.Round(FindObjectOfType<AudioManager>().activeSource.time, 2) * 100);
        if(index < this.activeSourceTimestamps.Count){
            int lb = index == 0 ? 0 : index - 1;
            int ub = index == this.activeSourceTimestamps.Count - 1 ? index : index + 1;
            for(int i = lb; i <= ub; i++){
                if(this.activeSourceTimestamps[i].isOnset){
                    Debug.LogWarning("Onset");
                    this.activeSourceTimestamps[i].isOnset = false;
                }
                if(this.activeSourceTimestamps[i].isBeat){
                    Debug.LogWarning("Beat");
                    this.activeSourceTimestamps[i].isBeat = false;
                }
            }
        }
    }
}