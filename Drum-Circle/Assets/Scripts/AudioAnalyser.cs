using UnityEngine.Audio;
using System;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

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

public class TrackMidi {
    public MidiFile midiFile;
    public TempoMap tempoMap;
}

public class AudioAnalyser : MonoBehaviour {

    public static AudioAnalyser instance;

    public string analysisFile;
    public TrackAnalysis activeAnalysis;
    public TrackMidi activeMidi;

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

    public void loadMidiFile(string name)
    {
        string path = "../Audio/" + name + ".mid";
        this.activeMidi = new TrackMidi();
        this.activeMidi.midiFile = MidiFile.Read(path);
        this.activeMidi.tempoMap = this.activeMidi.midiFile.GetTempoMap();
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
        loadMidiFile("ddc-oriental-taiko-midi");
    }
}