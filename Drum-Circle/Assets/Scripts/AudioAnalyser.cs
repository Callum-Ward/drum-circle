using UnityEngine.Audio;
using System;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AudioTimestamp {
    public bool isOnset;
    public bool isBeat;
    public string strength;
}

public class TrackAnalysis {
    public string name;
    public string path;
    public List<AudioTimestamp> timestampedOnsets;
}

public class AudioAnalyser : MonoBehaviour {

    public static AudioAnalyser instance;

    public string analysisFile;
    public TrackAnalysis activeAnalysis;

    public void loadTrackAnalysis(string name)
    {
        string path = "../Audio/" + name + ".json";
        List<AudioTimestamp> timestamps;
        using (StreamReader r = new StreamReader(path))  
        {  
            string json = r.ReadToEnd();  
            timestamps = JsonConvert.DeserializeObject<List<AudioTimestamp>>(json);  
        }  

        this.activeAnalysis = new TrackAnalysis();
        this.activeAnalysis.name = name;
        this.activeAnalysis.path = path;
        this.activeAnalysis.timestampedOnsets = timestamps;
    }

    // Awake is called before the Start method
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        loadTrackAnalysis(this.analysisFile);
    }
}