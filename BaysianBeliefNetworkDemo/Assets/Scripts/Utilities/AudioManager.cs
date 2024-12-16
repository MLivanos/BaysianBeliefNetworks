using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundGroup music;
    [SerializeField] private SoundGroup sfx;
    private float epsilon = 0.1f;
    private float lastMusicVolume = -1f;
    private float lastSFXVolume = -1f;

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

    public void AdjustMusicVolume(float volume)
    {
        if (Mathf.Abs(volume - lastMusicVolume) < epsilon) return;
        lastMusicVolume = volume;
        music.UpdateVolume(volume);
    }

    public void AdjustSFXVolume(float volume)
    {
        if (Mathf.Abs(volume - lastSFXVolume) < epsilon) return;
        lastSFXVolume = volume;
        sfx.UpdateVolume(volume);
    }

    public void AdjustMasterVolume(float volume)
    {
        AdjustMusicVolume(volume);
        AdjustSFXVolume(volume);
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
}
