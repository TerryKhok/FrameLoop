using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioMixerGroup SEMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.src = gameObject.AddComponent<AudioSource>();
            s.src.clip = s.clip;
            s.src.volume = s.volume;
            s.src.pitch = s.pitch;
            s.src.loop = s.loop;

            switch (s.audioType)
            {
                case Sound.AudioTypes.SE:
                    s.src.outputAudioMixerGroup = SEMixerGroup;
                    break;
                case Sound.AudioTypes.Music:
                    s.src.outputAudioMixerGroup = musicMixerGroup;
                    break;
            }

            s.src.spatialBlend = s.spatialBlend;
            s.src.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + "not found!");
            return;
        }
        s.src.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + "not found!");
            return;
        }
        s.src.Stop();
    }
}