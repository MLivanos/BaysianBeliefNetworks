using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundGroup music;
    [SerializeField] private SoundGroup sfx;
    [Range(0f, 1f)] private float masterVolume = 1f;
    [Range(0f, 1f)] private float musicVolume = 1f;
    [Range(0f, 1f)] private float sfxVolume = 1f;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private float lastMusicVolume = 1f;
    private float lastSFXVolume = 1f;
    private float epsilon = 0.01f;

    private bool muted = false;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadVolumeSettings();
        UpdateVolumeLevels();
    }

    public void AdjustMusicVolume(float volume)
    {
        if (Mathf.Abs(volume - lastMusicVolume) < epsilon) return;
        musicVolume = volume;
        lastMusicVolume = volume;
        UpdateVolumeLevels();
    }

    public void AdjustSFXVolume(float volume)
    {
        if (Mathf.Abs(volume - lastSFXVolume) < epsilon) return;
        sfxVolume = volume;
        lastSFXVolume = volume;
        UpdateVolumeLevels();
    }

    public void AdjustMasterVolume(float volume)
    {
        if (Mathf.Abs(volume - masterVolume) < epsilon) return;
        masterVolume = volume;
        UpdateVolumeLevels();
    }

    public void Mute()
    {
        if (muted) UpdateVolumeLevels(); // If already muted, unmute
        else // Otherwise, mute
        {
            music.UpdateVolume(0f);
            sfx.UpdateVolume(0f);
        }
        muted = !muted;
    }

    public bool IsMute()
    {
        return muted;
    }

    private void UpdateVolumeLevels()
    {
        music.UpdateVolume(masterVolume * musicVolume);
        sfx.UpdateVolume(masterVolume * sfxVolume);

        SaveVolumeSettings();
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
    }

    public void PlayMusic(string trackName)
    {
        music.Play(trackName);
    }

    public void PlayEffect(string trackName)
    {
        sfx.Play(trackName);
    }

    public void FadeOutMusic(float duration)
    {
        music.FadeOut(duration);
    }

    public void FadeOutSFX(float duration)
    {
        sfx.FadeOut(duration);
    }

    public void FadeInMusicAndAmbient(List<string> musicToFadeIn, float duration)
    {
        if (musicToFadeIn.Count == 0) return;
        music.FadeIn(duration, musicToFadeIn.GetRange(0, 1));
        sfx.FadeIn(duration, musicToFadeIn.GetRange(1, musicToFadeIn.Count-1));
    }

    public void FadeInSong(string song, float duration)
    {
        music.FadeIn(duration, new List<string>{song});
    }

    public void PauseMusic()
    {
        music.PauseAll();
    }

    public float GetSongLength(string song)
    {
        return music.GetSongLength(song);
    }

    public float GetSongProgress(string song)
    {
        return music.GetProgress(song);
    }

    public void SetMusicTime(string song, float time)
    {
        music.SetTime(song, time);
    }
}
