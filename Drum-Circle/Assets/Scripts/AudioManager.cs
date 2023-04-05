using UnityEngine.Audio;
using System;
using UnityEngine;


public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    public static AudioManager instance;
    private string fadeIn = null;
    private string fadeOut = null;
    private string fadeSpeed = null;

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

    //Update is called every frame.
    void update()
    {
        if (fadeIn != null)
        {
            FadeIn(fadeIn, fadeSpeed);
        }
        if (fadeOut != null)
        {
            FadeOut(fadeOut);
        }
    }

    //Plays the passed track.
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

    //Allows changing of volume for selected track
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

    //Starts a fade in based on passed speed (Fast/Slow)
    public void FadeIn(string name, string speed)
    {
        Sound s = Array.Find(sounds, sounds => sounds.Name == name);
        fadeOut = null;

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
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
            fadeIn = name;
        }
    }

    //Fades out music.
    public void FadeOut(string name)
    {
        Sound s = Array.Find(sounds, sounds => sounds.Name == name);
        fadeIn = null;

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = s.source.volume - 0.33f;
        if (s.source.volume == 0f)
        {
            fadeOut = null;
        }
        else
        {
            fadeOut = name;
        }
    }

    //Sets passed sound track as active in the source
    public void SetActive(string name)
    {
        Sound s = Array.Find(sounds, sounds => sounds.Name == name);


        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        this.activeSource = s.source;
    }
}
