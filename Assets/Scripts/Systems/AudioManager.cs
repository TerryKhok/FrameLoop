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
    private bool world1ThemeFlag = false;
    private bool world23ThemeFlag = false;
    private bool world45ThemeFlag = false;

    private void Start()
    {
        menuThemeFlag = false;
        gameThemeFlag = false;
        world1ThemeFlag = false;
        world23ThemeFlag = false;
        world45ThemeFlag = false;
}
    private void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        
        if(!menuThemeFlag) 
        {
            if (sceneName == "MainMenu" || sceneName == "World1" || sceneName == "World2" ||
                sceneName == "World3" || sceneName == "World4" || 
                sceneName == "WorldSelect" || sceneName == "AppreciateScene")
            {
                Play("TitleTheme");
                Stop("BGM Calm");
                Stop("Main BGM");
                Stop("BGM Intense");

                menuThemeFlag       = true;
                gameThemeFlag       = false;

                world1ThemeFlag     = false;
                world23ThemeFlag    = false;
                world45ThemeFlag    = false;
            }
        }
        //1 - 7     = world 1   "BGM Calm"
        //8 - 14    = world 2   "Main BGM"
        //15 - 21   = world 3   "Main BGM"
        //22 - 27   = world 4   "BGM Intense"
        if(!world1ThemeFlag)
        {
            if(sceneName == "lvl 1" || sceneName == "lvl 2" || sceneName == "lvl 3" || 
                sceneName == "lvl 4" || sceneName == "lvl 5" || sceneName == "lvl 6" || 
                sceneName == "lvl 7")
            {
                Stop("TitleTheme");
                Play("BGM Calm");
                Stop("Main BGM");
                Stop("BGM Intense");

                menuThemeFlag = false;
                world1ThemeFlag = true;

                world1ThemeFlag = true;
                world23ThemeFlag = false;
                world45ThemeFlag = false;
            }
        }
        if (!world23ThemeFlag)
        {
            if (sceneName == "lvl 8" || sceneName == "lvl 9" || sceneName == "lvl 10" ||
                sceneName == "lvl 11" || sceneName == "lvl 12" || sceneName == "lvl 13" ||
                sceneName == "lvl 14" ||
                sceneName == "lvl 15" || sceneName == "lvl 16" || sceneName == "lvl 17" ||
                sceneName == "lvl 18" || sceneName == "lvl 19" || sceneName == "lvl 20" ||
                sceneName == "lvl 21")
            {
                Stop("TitleTheme");
                Stop("BGM Calm");
                Play("Main BGM");
                Stop("BGM Intense");

                menuThemeFlag = false;
                world1ThemeFlag = true;

                world1ThemeFlag = false;
                world23ThemeFlag = true;
                world45ThemeFlag = false;
            }
        }
        if (!world45ThemeFlag)
        {
            if (sceneName == "lvl 22" || sceneName == "lvl 23" || sceneName == "lvl 24" ||
                sceneName == "lvl 25" || sceneName == "lvl 26" || sceneName == "lvl 27")
            {
                Stop("TitleTheme");
                Stop("BGM Calm");
                Stop("Main BGM");
                Play("BGM Intense");

                menuThemeFlag = false;
                world1ThemeFlag = true;

                world1ThemeFlag = false;
                world23ThemeFlag = false;
                world45ThemeFlag = true;
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