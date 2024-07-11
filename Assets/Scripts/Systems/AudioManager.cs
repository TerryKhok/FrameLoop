using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.Rendering.Universal;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioMixerGroup SEMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

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

    private bool menuThemeFlag = false;
    private bool gameThemeFlag = false;

    private void Start()
    {
        menuThemeFlag = false;
        gameThemeFlag = false;
}
    private void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        
        if(!menuThemeFlag) 
        {
            if (sceneName == "MainMenu" || sceneName == "World1" || sceneName == "World2" ||
                sceneName == "World3" || sceneName == "WorldSelect" || sceneName == "AppreciateScene")
            {
                Play("TitleTheme");
                Stop("Main BGM");

                menuThemeFlag = true;
                gameThemeFlag = false;
            }
        }
        if(!gameThemeFlag)
        {
            if(sceneName == "lvl 1" || sceneName == "lvl 2" || sceneName == "lvl 3" || sceneName == "lvl 4" || sceneName == "lvl 5" ||
                sceneName == "lvl 6" || sceneName == "lvl 7" || sceneName == "lvl 8" || sceneName == "lvl 9" || sceneName == "lvl 10" ||
                sceneName == "lvl 11" || sceneName == "lvl 12" || sceneName == "lvl 13" || sceneName == "lvl 14" || sceneName == "lvl 15" ||
                sceneName == "lvl 16" || sceneName == "lvl 17" || sceneName == "lvl 18")
            {
                Stop("TitleTheme");
                Play("Main BGM");

                menuThemeFlag = false;
                gameThemeFlag = true;
            }
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