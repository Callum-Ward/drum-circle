using UnityEngine.Audio;
using System;
using UnityEngine;


public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    public static AudioManager instance;

    public AudioSource activeSource;

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
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sounds => sounds.Name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        this.activeSource = s.source;
        this.activeSource.Play();
    }

    public float Volume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sounds => sounds.Name == name);


        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return 0f;
        }
        s.source.volume = volume;

        return s.source.volume;
    }

    public void FadeIn(string name)
    {
        Sound s = Array.Find(sounds, sounds => sounds.Name == name);


        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = s.source.volume + 0.01f;
    }
}
