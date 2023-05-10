using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
    public int noteSize;
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
    public int[] midiOffsets;

    public void setPlayerCount(int playerCount)
    {
        this.playerCount = playerCount;
    }


    //Loads track onset map from JSON file
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


    //Loads track onset map from JSON file for each player
    public void loadAnalysisJson()
    {
        this.playerJson = new TrackJson[this.playerCount];
        for(int i = 0; i < this.playerCount; i++)
        {
            TrackJson j = loadTrackJson(this.playerJsonFiles[i]);
            this.playerJson[i] = j;
        }
    }


    //Finds average velocity, average note number and creates left/right drum map from series of notes
    private Tuple<int, int, IDictionary<int, bool>> getNotesInformation(IEnumerable<Note> notes)
    {
        List<int> noteNumbers = new List<int>();
        List<int> noteVelocities = new List<int>();
        int minVelocity = 127;
        int maxVelocity = 0;

        foreach(Note note in notes)
        {
            noteNumbers.Add((int)(note.NoteNumber));
            noteVelocities.Add((int)note.Velocity);

            int noteVelocity = (int)note.Velocity;
            if(noteVelocity < minVelocity)
            {
                minVelocity = noteVelocity;
            }
            if(noteVelocity > maxVelocity)
            {
                maxVelocity = noteVelocity;
            }
        }

        noteNumbers.Sort();

        int medianNoteNumber = noteNumbers[noteNumbers.Count / 2];
        int medianVelocity = noteVelocities[noteVelocities.Count / 2];

        List<int> noteNumbersDistinct = noteNumbers.Distinct().ToList();
        IDictionary<int, bool> drumSideMap = new Dictionary<int, bool>();
        for(int i = 0; i < noteNumbersDistinct.Count; i++ )
        {
            drumSideMap.Add(noteNumbersDistinct[i], i % 3 == 0 || i % 5 == 0);
        }

        return Tuple.Create(medianNoteNumber, medianVelocity, drumSideMap);
    }


    //Loads and analysises track MIDI file, with pre-determined offset to align with timescale of song
    public TrackMidi loadTrackMidi(string name, int midiOffset)
    {
        string path = "./Assets/Music/PlayerMidis/" + name + ".mid";

        TrackMidi midi = new TrackMidi();

        MidiFile midiFile = MidiFile.Read(path);
        TempoMap tempoMap = midiFile.GetTempoMap();
        TimeSpan midiFileDuration = midiFile.GetDuration<MetricTimeSpan>();
        int durationInMills = midiFileDuration.Minutes * 60000 + midiFileDuration.Seconds * 1000 + midiFileDuration.Milliseconds + midiOffset;

        midi.midiFileDuration = midiFileDuration;
        midi.timestampedNotes = new TimestampedNote[durationInMills];

        IEnumerable<Note> notes = midiFile.GetNotes();

        Tuple<int, int, IDictionary<int, bool>> notesInformation = getNotesInformation(notes);
        int medianNoteNumber = notesInformation.Item1;
        int medianVelocity = notesInformation.Item2;
        IDictionary<int, bool> drumSideMap = notesInformation.Item3;

        
        StringBuilder sb = new StringBuilder();

        foreach(Note note in notes)
        {
            TimeSpan time = note.TimeAs<MetricTimeSpan>(tempoMap);
            int timeInMills = time.Minutes * 60000 + time.Seconds * 1000 + time.Milliseconds + midiOffset;

            sb.Append(timeInMills);
            sb.Append(", ");

            midi.timestampedNotes[timeInMills] = new TimestampedNote();
            midi.timestampedNotes[timeInMills].noteNumber = (int)(note.NoteNumber);
            midi.timestampedNotes[timeInMills].left = drumSideMap[(int)(note.NoteNumber)] ? 1 : 0;
            midi.timestampedNotes[timeInMills].noteSize = (int)(note.Velocity) >= medianVelocity ? 2 : 1;
        }

        //Debug.Log("MIDI: " + sb.ToString());

        return midi;
    }


    //Loads and analysises track MIDI file for each players track
    public void loadAnalysisMidi()
    {
        this.playerMidis = new TrackMidi[this.playerCount];
        for(int i = 0; i < this.playerCount; i++)
        {
            TrackMidi m = loadTrackMidi(this.playerMidiFiles[i], this.midiOffsets[i]);
            this.playerMidis[i] = m;
        }
    }

 
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