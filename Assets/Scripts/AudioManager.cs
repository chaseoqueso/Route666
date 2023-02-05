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
    [FMODUnity.EventRef] public string greezeyGulchMusicTrack;

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

    private bool isInCharacterSelect;
    private bool pauseMuffle;

    void Start()
    {
        instance = this;
        isInCharacterSelect = false;

        masterVCA = FMODUnity.RuntimeManager.GetVCA(masterVCAPath);
        masterVCA.setVolume(masterVolume);

        musicVCA = FMODUnity.RuntimeManager.GetVCA(musicVCAPath);
        musicVCA.setVolume(musicVolume);

        sfxVCA = FMODUnity.RuntimeManager.GetVCA(sfxVCAPath);
        sfxVCA.setVolume(sfxVolume);
    }

    public void playMusic(MusicType musicTrack, int zone = -1)
    {
        StopAllCoroutines();
        switch(musicTrack)
        {
            case MusicType.MainMenu:
                musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(mainMenuMusicTrack);
                musicInstance.start();
                isInCharacterSelect = false;
                break;
        }
    }

    public void stopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        StopAllCoroutines();
    }

    public void playSound(SFXType soundEffect)
    {
        switch(soundEffect)
        {
            case SFXType.MenuSelect:
                soundEffectHelper(menuSelectEvent);
                break;
        }
    }

    public void setPauseMuffle(bool shouldMuffle)
    {
        if(pauseMuffle != shouldMuffle)
        {
            pauseMuffle = shouldMuffle;
            MusicVolume = MusicVolume * (shouldMuffle ? 0.5f : 2);
        }
    }

    private void soundEffectHelper(string eventPath)
    {
        sfxInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        sfxInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        sfxInstance.start();
        musicInstance.setParameterByName("NoiseVolume", 0);
    }
}
