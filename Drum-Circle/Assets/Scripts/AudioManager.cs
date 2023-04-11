using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour {

    public Sound[] drumTracks;
    public Sound[] additiveLayers;
    public Sound[] oneShots;

    public Sound[] sounds;

    public static AudioManager instance;
    private Sound fadeIn = null;
    private Sound fadeOut = null;
    private string fadeSpeed = null;

    public AudioSource activeSource;

    public List<AudioSource> activeSources;

    private System.Random random;

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

        DontDestroyOnLoad(gameObject);

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

        activeSources = new List<AudioSource>();
        random = new System.Random();
    }

    //Update is called every frame.
    void update()
    {
        if (fadeIn != null)
        {
            FadeInTrack(fadeIn, fadeSpeed);
        }
        if (fadeOut != null)
        {
            FadeOutTrack(fadeOut);
        }
    }

    void PlayTrack(Sound s)
    {
        s.source.Play();
        activeSources.Add(s.source);
    }

    float VolumeTrack(Sound s, float volume)
    {
        s.source.volume = volume;
        return s.source.volume;
    }

     //Starts a fade in based on passed speed (Fast/Slow)
    void FadeInTrack(Sound s, string speed)
    {
        if (speed == "fast")
        {
            s.source.volume = s.source.volume + 0.5f;
            fadeSpeed = speed;
        }
        else if (speed == "slow")
        {
            s.source.volume = s.source.volume + 0.1f;
            fadeSpeed = speed;
        }
        
        if (s.source.volume == 1f)
        {
            fadeIn = null;
        }
        else
        {
            fadeIn = s;
        }
    }

    //Fades out music.
    void FadeOutTrack(Sound s)
    {
        fadeIn = null;
        s.source.volume = s.source.volume - 0.33f;
        if (s.source.volume == 0f)
        {
            fadeOut = null;
        }
        else
        {
            fadeOut = s;
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
        AudioSource source = oneShots[index].source;
        source.volume = velocity;
        source.PlayOneShot(source.clip, 1f);
    }

    public int PlayRandomOneShot(float velocity)
    {
        int index = random.Next(1, oneShots.Length - 1);
        this.PlayDrumOneShot(index, velocity);
        return index;
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
}
