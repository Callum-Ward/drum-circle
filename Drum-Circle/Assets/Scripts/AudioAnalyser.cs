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

public class TrackAnalysis {
    public string name;
    public string path;
    public List<AudioTimestamp> timestampedOnsets;
}

public class AudioAnalyser : MonoBehaviour {

    public TrackAnalysis[] tracks;
    public static AudioAnalyser instance;

    public TrackAnalysis activeAnalysis;

    public void loadTrackAnalysis(string name)
    {
        TrackAnalysis t = Array.Find(tracks, track => track.name == name);
        if (t == null)
        {
            Debug.LogWarning("Track analysis for " + name + " not found!");
            return;
        }
        this.activeAnalysis = t;
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

        /******Test tracks**********/

        TrackAnalysis TEST_TRACK = new TrackAnalysis();
        TEST_TRACK.name = "drums";
        TEST_TRACK.path = "../Audio/BiBDrumsBass.json";
        tracks = new TrackAnalysis[] {TEST_TRACK};

        /**************************/

        foreach(TrackAnalysis t in tracks)
        {

            List<AudioTimestamp> timestamps;
            using (StreamReader r = new StreamReader(t.path))  
            {  
                string json = r.ReadToEnd();  
                timestamps = JsonConvert.DeserializeObject<List<AudioTimestamp>>(json);  
            }  
            t.timestampedOnsets = timestamps;
        }
    }
}