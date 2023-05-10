using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour {

    public Sound[] drumTracks;
    public Sound[] additiveLayers;
    public Sound[] oneShots;
    public Sound background;

    public int[] oneShotMap;
    public int persistentLayerIndex;

    public static AudioManager instance;
    private Sound fadeIn = null;
    private Sound fadeOut = null;
    private string fadeSpeed = null;
    private float volLimit = 1f;

    public AudioSource activeSource;

    public List<AudioSource> activeSources;
    private List<int> activeLayerIndices;

    private System.Random random;

    public AudioSource audioSource;


    //Initialises sound from given audio clip
    void initialiseSound(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;

        s.source.loop = s.loop;
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

        foreach(Sound s in drumTracks)
        {
            initialiseSound(s);
        }

        foreach(Sound s in additiveLayers)
        {
            initialiseSound(s);
        }

        foreach(Sound s in oneShots)
        {
            initialiseSound(s);
        }

        initialiseSound(background);

        activeSources = new List<AudioSource>();
        activeLayerIndices = new List<int>();
        random = new System.Random();

        audioSource = GetComponent<AudioSource>();
    }

    //Update is called every frame.
    void update()
    {
        if (fadeIn != null)
        {
            FadeInTrack(fadeIn, fadeSpeed, volLimit);
        }
        if (fadeOut != null)
        {
            FadeOutTrack(fadeOut, fadeSpeed, volLimit);
        }
    }


    //Plays specified track by name
    public void PlaySingle(string name) {
        Sound s = Array.Find(drumTracks, sound => sound.Name == name);
        s.source.Play();
    }


    //Stops specified track by name
    public void StopSingle(string name) {
        Sound s = Array.Find(drumTracks, sound => sound.Name == name);
        s.source.Stop();
    }


    //Plays given sound
    public void PlayTrack(Sound s)
    {
        s.source.Play();
        activeSources.Add(s.source);
    }


    //Sets volume of given sound
    public float VolumeTrack(Sound s, float volume)
    {
        s.source.volume = volume;
        return s.source.volume;
    }


     //Starts a fade in based on passed speed (Fast/Slow)
    public void FadeInTrack(Sound s, string speed, float limit)
    {
         if(fadeOut != null)
        {
            return;
        }

        if (speed == "fast")
        {
            s.source.volume = s.source.volume + (limit*Time.deltaTime)*5;
            fadeSpeed = speed;
        }
        else if (speed == "slow")
        {
            s.source.volume = s.source.volume + (limit*Time.deltaTime) * 0.5f;
            fadeSpeed = speed;
        }
        
        if (s.source.volume >= limit)
        {
            s.source.volume = limit;
            fadeIn = null;
        }
        else
        {
            volLimit = limit;
            fadeIn = s;
        }
    }


    //Fades out music.
    public void FadeOutTrack(Sound s, string speed ="fast", float limit = 1)
    {
        if(fadeIn != null)
        {
            return;
        }

        if(speed == "fast") {
            s.source.volume = s.source.volume - (limit*Time.deltaTime)*3;
            fadeSpeed = speed;
        }
        if(speed == "slow") {
            s.source.volume = s.source.volume - (limit*Time.deltaTime);
            fadeSpeed = speed;
        }

        if (s.source.volume <= 0f)
        {
            s.source.volume = 0f;
            fadeOut = null;
        }
        else
        {
            fadeOut = s;
            volLimit = limit;
        }
    }

    //Plays indexed drumt track
    public void PlayDrumTrack(int index)
    {
        if(index >= drumTracks.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return;
        }

        PlayTrack(drumTracks[index]);
    }


    //Plays all drum tracks
    public void PlayAllDrumTracks()
    {
        foreach(Sound s in drumTracks)
        {
           PlayTrack(s);
        }
    }


    //Plays indexed layer track at given volume
    public void PlayLayerTrack(int index, float volume)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for additive layers");
            return;
        }
        Sound s = additiveLayers[index];
        s.source.volume = volume;
        PlayTrack(s);
    }


    //Plays all layer tracks at given volume
    public void PlayAllLayerTracks(float volume)
    {
        for(int i = 0; i < additiveLayers.Length; i++)
        {
            PlayLayerTrack(i, i == 0 ? volume : volume);
        }
        this.activeLayerIndices.Add(0);

    }


    //Plays indexed drum one shot with given velocity
    public void PlayDrumOneShot(int index, float velocity)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for drum one shots");
            return;
        }

        AudioSource source = oneShots[oneShotMap[index]].source;
        source.volume = velocity;
        source.PlayOneShot(source.clip, 1f);
    }


    //Plays mapped one shot for solo section
    public int PlaySoloOneShot(int playerIndex, int drumIndex, float velocity)
    {
        int index = oneShotMap[playerIndex * 2 + drumIndex];
        this.PlayDrumOneShot(index, velocity);
        return index;
    }


    //Plays ambience
    public void PlayBackgroundTrack()
    {
        PlayTrack(background);
    }

    
    //Sets volume of indexed drum track
    public float VolumeDrumTrack(int index, float volume)
    {
        if(index >= drumTracks.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return 0f;
        }
        
        return VolumeTrack(drumTracks[index], volume);
    }


    //Sets volume of indexed layer track
    public float VolumeLayerTrack(int index, float volume)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for additive layers");
            return 0f;
        }
        
        return VolumeTrack(additiveLayers[index], volume);
    }


    //Fades in indexed drum track at given speed
    public void FadeInDrumTrack(int index, string speed)
    {
        if(index >= drumTracks.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return;
        }

        FadeInTrack(drumTracks[index], speed, 1.0f);
    }


    //Fades out indexed drum track
    public void FadeOutDrumTrack(int index)
    {
        if(index >= drumTracks.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return;
        }

        FadeOutTrack(drumTracks[index]);
    }


    //Fades in indexed layer track at given speed
    public void FadeInLayerTrack(int index, string speed)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return;
        }

        FadeInTrack(additiveLayers[index], speed, 0.5f);
    }


    //Fades out indexed layer track
    public void FadeOutLayerTrack(int index)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for layer tracks");
            return;
        }

        FadeOutTrack(additiveLayers[index]);
    }

    //Fades in next additive music layer
    public void AddLayer()
    {
        if(this.activeLayerIndices.Count == additiveLayers.Length)
        {
            return;
        }

        int index = this.activeLayerIndices.Count;

        this.activeLayerIndices.Add(index);
        FadeInLayerTrack(index, "slow");
        Debug.Log("Faded In Layer " + index.ToString());
    }


    //Fades out most recently added additive music layer
    public void RemoveLayer()
    {
        if(this.activeLayerIndices.Count <= 1)
        {
            return;
        }

        int value = this.activeLayerIndices[this.activeLayerIndices.Count - 1];
        this.activeLayerIndices.Remove(value);
        FadeOutLayerTrack(value);
        Debug.Log("Faded Out Layer " + value.ToString());
    }


    //Returns number of additive layers currently audible
    public int addedLayersCount()
    {
        return this.activeLayerIndices.Count;
    }

}
