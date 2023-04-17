using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine.Audio;
using UnityEngine;

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

using Newtonsoft.Json;

public class AudioTimestamp {
    public bool isOnset;
    public bool isBeat;
    public string strength;
}

public class TrackJson {
    public string name;
    public string path;
    public List<AudioTimestamp> timestampedOnsets;
}

public class TimestampedNote {
    public int left;
    public int noteNumber;
}

public class TrackMidi {
    public TimeSpan midiFileDuration;
    public TimestampedNote[] timestampedNotes;
}

public class AudioAnalyser : MonoBehaviour {

    public static AudioAnalyser instance;

    private int playerCount;

    public string analysisFile;
    public TrackJson activeAnalysis;

    public TrackMidi[] playerMidis;
    public TrackJson[] playerJson;

    public string[] playerJsonFiles;
    public string[] playerMidiFiles;

    private long ticks = 123;


    public void setPlayerCount(int playerCount)
    {
        this.playerCount = playerCount;
    }

    private TrackJson loadTrackJson(string name)
    {
        string path = "./Assets/Music/PlayerJson/" + name + ".json";
        List<AudioTimestamp> timestamps;
        using (StreamReader r = new StreamReader(path))  
        {  
            string json = r.ReadToEnd();  
            timestamps = JsonConvert.DeserializeObject<List<AudioTimestamp>>(json);  
        }  

        TrackJson trackJson = new TrackJson();
        trackJson.name = name;
        trackJson.path = path;
        trackJson.timestampedOnsets = timestamps;
        return trackJson;
    }

    public void loadAnalysisJson()
    {
        this.playerJson = new TrackJson[this.playerCount];
        for(int i = 0; i < this.playerCount; i++)
        {
            TrackJson j = loadTrackJson(this.playerJsonFiles[i]);
            this.playerJson[i] = j;
        }
    }

    private int getMedianNoteNumber(IEnumerable<Note> notes)
    {
        List<int> noteNumbers = new List<int>();
        foreach(Note note in notes)
        {
            noteNumbers.Add((int)(note.NoteNumber));
        }

        noteNumbers.Sort();
        return noteNumbers[noteNumbers.Count / 2];
    }

    public TrackMidi loadTrackMidi(string name)
    {
        string path = "./Assets/Music/PlayerMidis/" + name + ".mid";

        TrackMidi midi = new TrackMidi();

        MidiFile midiFile = MidiFile.Read(path);
        TempoMap tempoMap = midiFile.GetTempoMap();
        TimeSpan midiFileDuration = midiFile.GetDuration<MetricTimeSpan>();
        int durationInMills = midiFileDuration.Minutes * 60000 + midiFileDuration.Seconds * 1000 + midiFileDuration.Milliseconds;

        midi.midiFileDuration = midiFileDuration;
        midi.timestampedNotes = new TimestampedNote[durationInMills];

        IEnumerable<Note> notes = midiFile.GetNotes();

        int medianNoteNumber = getMedianNoteNumber(notes);

        foreach(Note note in notes)
        {
            TimeSpan time = note.TimeAs<MetricTimeSpan>(tempoMap);
            int timeInMills = time.Minutes * 60000 + time.Seconds * 1000 + time.Milliseconds;
            midi.timestampedNotes[timeInMills] = new TimestampedNote();
            midi.timestampedNotes[timeInMills].noteNumber = (int)(note.NoteNumber);
            midi.timestampedNotes[timeInMills].left = (int)(note.NoteNumber) >= medianNoteNumber ? 1 : 0;
        }

        return midi;
    }

    public void loadAnalysisMidi()
    {
        this.playerMidis = new TrackMidi[this.playerCount];
        for(int i = 0; i < this.playerCount; i++)
        {
            TrackMidi m = loadTrackMidi(this.playerMidiFiles[i]);
            this.playerMidis[i] = m;
        }
    }

    public int timeAtNearestNote(int playerIndex, int drumIndex, float time)
    {
        int buffer = 200;
        int timeInMills = (int)Math.Ceiling(time * 1000);
        for(int i = 0; i < buffer; i++)
        {
            TimestampedNote? note = this.playerMidis[playerIndex].timestampedNotes[timeInMills + i];
            if(note != null)
            {
                if(1 - drumIndex == note.left)
                {
                    return timeInMills + i;
                }
            }

            note = this.playerMidis[playerIndex].timestampedNotes[timeInMills - i];
            if(note != null)
            {
                if(1 - drumIndex == note.left)
                {
                    return timeInMills - i;
                }
            }
        }

        return -1;
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
    }

    void Start()
    {
        this.loadAnalysisJson();
        this.loadAnalysisMidi();
    }
}