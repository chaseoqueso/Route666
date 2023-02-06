using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public enum MusicType
    {
        None,
        MainMenu,
        Level1
    }

    public enum SFXType
    {
        None,
        MenuSelect
    }

    [Header("Music Tracks")]
    [FMODUnity.EventRef] public string mainMenuMusicTrack;
    [FMODUnity.EventRef] public string level1MusicTrack;

    [Header("Sound Effects")]
    [FMODUnity.EventRef] public string menuSelectEvent;

    [Header("Volume Levels")]
    [Range(0, 1)] [SerializeField] private float masterVolume;
    [Range(0, 1)] [SerializeField] private float musicVolume;
    [Range(0, 1)] [SerializeField] private float sfxVolume;

    public float MasterVolume
    {
        set
        {
            masterVolume = value;
            masterVCA.setVolume(masterVolume);
        }

        get { return masterVolume; }
    }

    public float MusicVolume
    {
        set
        {
            musicVolume = value;
            musicVCA.setVolume(musicVolume);
        }

        get { return musicVolume; }
    }

    public float SfxVolume
    {
        set
        {
            sfxVolume = value;
            sfxVCA.setVolume(sfxVolume);
        }

        get { return sfxVolume; }
    }

    private const string masterVCAPath = "vca:/Master";
    private const string musicVCAPath = "vca:/Music";
    private const string sfxVCAPath = "vca:/SFX";
    
    private FMOD.Studio.VCA masterVCA;
    private FMOD.Studio.VCA musicVCA;
    private FMOD.Studio.VCA sfxVCA;
    
    private FMOD.Studio.EventInstance musicInstance;
    private FMOD.Studio.EventInstance sfxInstance;

    private bool pauseMuffle;

    void Awake()
    {
        instance = this;

        masterVCA = FMODUnity.RuntimeManager.GetVCA(masterVCAPath);
        masterVCA.setVolume(masterVolume);

        musicVCA = FMODUnity.RuntimeManager.GetVCA(musicVCAPath);
        musicVCA.setVolume(musicVolume);

        sfxVCA = FMODUnity.RuntimeManager.GetVCA(sfxVCAPath);
        sfxVCA.setVolume(sfxVolume);
    }

    public void PlayMusic(MusicType musicTrack, int zone = -1)
    {
        StopAllCoroutines();
        switch(musicTrack)
        {
            case MusicType.MainMenu:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(mainMenuMusicTrack);
                musicInstance.start();
                break;
                
            case MusicType.Level1:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(level1MusicTrack);
                musicInstance.start();
                break;
        }
    }

    public void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        StopAllCoroutines();
    }

    public void PlaySound(SFXType soundEffect)
    {
        switch(soundEffect)
        {
            case SFXType.MenuSelect:
                SoundEffectHelper(menuSelectEvent);
                break;
        }
    }

    public void SetPauseMuffle(bool shouldMuffle)
    {
        if(pauseMuffle != shouldMuffle)
        {
            pauseMuffle = shouldMuffle;
            MusicVolume = MusicVolume * (shouldMuffle ? 0.5f : 2);
        }
    }

    private void SoundEffectHelper(string eventPath)
    {
        sfxInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        sfxInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        sfxInstance.start();
        musicInstance.setParameterByName("NoiseVolume", 0);
    }
}
