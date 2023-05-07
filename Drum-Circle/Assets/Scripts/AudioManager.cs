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

    // public float longestTime = 0;

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

        //DontDestroyOnLoad(gameObject);

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

    public float sourceDuration(int index) {
        return audioSource.time;
    }

    public void PlaySingle(string name) {
        Sound s = Array.Find(drumTracks, sound => sound.Name == name);
        s.source.Play();
    }
    public void StopSingle(string name) {
        Sound s = Array.Find(drumTracks, sound => sound.Name == name);
        s.source.Stop();
    }

    public void PlayTrack(Sound s)
    {
        s.source.Play();
        activeSources.Add(s.source);
        // if(s.clip.length > longestTime) {
        //     longestTime = s.clip.length;
        // }
    }

    public float VolumeTrack(Sound s, float volume)
    {
        s.source.volume = volume;
        return s.source.volume;
    }

     //Starts a fade in based on passed speed (Fast/Slow)
    public void FadeInTrack(Sound s, string speed, float limit = 1)
    {
        fadeOut = null;
        if (speed == "fast")
        {
            s.source.volume = s.source.volume + (limit*Time.deltaTime)*5;
            fadeSpeed = speed;
        }
        else if (speed == "slow")
        {
            s.source.volume = s.source.volume + (limit*Time.deltaTime);
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
        fadeIn = null;
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


    public void PlayDrumTrack(int index)
    {
        if(index >= drumTracks.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return;
        }

        PlayTrack(drumTracks[index]);
    }


    public void PlayAllDrumTracks()
    {
        foreach(Sound s in drumTracks)
        {
           PlayTrack(s);
        }
    }


    public void PlayLayerTrack(int index)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for additive layers");
            return;
        }
        PlayTrack(additiveLayers[index]);
    }

    public void PlayAllLayerTracks()
    {
        for(int i = 0; i < additiveLayers.Length; i++)
        {
            PlayLayerTrack(i);
        }
    }

    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(oneShots, sound => sound.Name == name);

        if(s == null)
        {
            Debug.LogWarning("No one-shot found with name " + name);
            return;
        }

        s.source.PlayOneShot(s.source.clip, 0.7F);
    }

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

    public int PlaySoloOneShot(int playerIndex, int drumIndex, float velocity)
    {
        int index = oneShotMap[playerIndex * 2 + drumIndex];
        this.PlayDrumOneShot(index, velocity);
        return index;
    }

    public void PlayBackgroundTrack()
    {
        PlayTrack(background);
    }


     public float VolumeDrumTrack(int index, float volume)
    {
        if(index >= drumTracks.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return 0f;
        }
        
        return VolumeTrack(drumTracks[index], volume);
    }


     public float VolumeLayerTrack(int index, float volume)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for additive layers");
            return 0f;
        }
        
        return VolumeTrack(additiveLayers[index], volume);
    }

    public void FadeInDrumTrack(int index, string speed)
    {
        if(index >= drumTracks.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return;
        }

        FadeInTrack(drumTracks[index], speed);
    }

    public void FadeOutDrumTrack(int index)
    {
        if(index >= drumTracks.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return;
        }

        FadeOutTrack(drumTracks[index]);
    }

     public void FadeInLayerTrack(int index, string speed)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for drum tracks");
            return;
        }

        FadeInTrack(additiveLayers[index], speed);
    }

    public void FadeOutLayerTrack(int index)
    {
        if(index >= additiveLayers.Length)
        {
            Debug.LogWarning("Invalid index for layer tracks");
            return;
        }

        FadeOutTrack(additiveLayers[index]);
    }

    public void ReduceToBackgroundLayer()
    {
        for(int i = 0; i < additiveLayers.Length; i++)
        {
            if(i != persistentLayerIndex)
            {
                FadeOutLayerTrack(i);
            }
        }
    }

    public void FadeInFromBackgroundLayer()
    {
        for(int i = 0; i < additiveLayers.Length; i++)
        {
            if(i != persistentLayerIndex)
            {
                FadeInLayerTrack(i, "slow");
            }
        }
    }

    public void AddLayer()
    {
        if(this.activeLayerIndices.Count == additiveLayers.Length)
        {
            return;
        }

        int index = random.Next() % additiveLayers.Length;
        while(this.activeLayerIndices.Contains(index))
        {
            if(index < additiveLayers.Length)
            {
                index += 1;
            }
            else
            {
                index = 0;
            }
        }

        this.activeLayerIndices.Add(index);
        FadeInLayerTrack(index, "slow");
        Debug.Log("Faded In Layer " + index.ToString());
    }

    public void RemoveLayer()
    {
        if(this.activeLayerIndices.Count == 0)
        {
            return;
        }

        int index = random.Next() % activeLayerIndices.Count;
        int value = this.activeLayerIndices[index];
        this.activeLayerIndices.Remove(value);
        FadeOutLayerTrack(value);
        Debug.Log("Faded Out Layer " + value.ToString());
    }

}
