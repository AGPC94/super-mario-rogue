using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    string currentMusic;

    Sound[] sounds;

    public static AudioManager instance;

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

        Sound[] effects = Resources.LoadAll("Sounds/Effects", typeof(Sound)).Cast<Sound>().ToArray();
        Sound[] music = Resources.LoadAll("Sounds/Music", typeof(Sound)).Cast<Sound>().ToArray();

        sounds = effects.Concat(music).ToArray();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    Sound FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            Debug.LogWarning("Sound: " + name + " not found!");
        else
            Debug.Log("Sound: " + name + " is finded!");

        return s;
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " is sounding!");

        s.source.Play();
    }
    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " is sounding!");

        if (!s.source.isPlaying)
            s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " stop!");

        s.source.Stop();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " stop!");

        s.source.Pause();
    }

    public void UnPause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " stop!");

        s.source.UnPause();
    }

    public void PlayMusic(string name)
    {
        if (currentMusic != name)
            Stop(currentMusic);

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " is sounding!");

        if (!s.source.isPlaying)
        {
            currentMusic = name;
            s.source.Play();
        }
    }

    public void StopMusic()
    {
        Stop(currentMusic);
    }

    public void PauseMusic()
    {
        Pause(currentMusic);
    }

    public void UnPauseMusic()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "Star");
        if (!s.source.isPlaying)
            UnPause(currentMusic);
    }

    public void AdjustVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Sound: " + name + " is sounding!");

        volume = Mathf.Clamp(volume, 0, 1);

        s.volume = volume;
        s.source.volume = volume;
    }
}
