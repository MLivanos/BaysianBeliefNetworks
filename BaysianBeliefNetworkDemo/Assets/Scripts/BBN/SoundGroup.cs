using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
	public string name;
	public AudioClip clip;
	public bool loop;
	[HideInInspector] public AudioSource source;

	public void Initialize(float volume, float pitch)
	{
		source.clip = clip;
		source.volume = volume;
		source.pitch = pitch;
		source.loop = loop;
	}

	public void UpdateVolume(float newVolume)
	{
		source.volume = newVolume;
	}

	public void UpdatePitch(float newPitch)
	{
		source.pitch = newPitch;
	}
}

[System.Serializable]
public class SoundGroup : MonoBehaviour
{
	[SerializeField] private List<Sound> sounds;
	[Range(0f,1f)] public float volume;
	[Range(0.1f,3f)] public float pitch;
	private Dictionary<string, Sound> soundsByName = new Dictionary<string, Sound>();
	private Dictionary<string, Sound> soundsPlaying = new Dictionary<string, Sound>();

	private void Start()
	{
		foreach(Sound sound in sounds)
		{
			soundsByName.Add(sound.name, sound);
			sound.source = gameObject.AddComponent<AudioSource>();
			sound.Initialize(volume, pitch);
		}
	}

	public void UpdateVolume(float volume)
	{
		foreach(Sound sound in sounds)
		{
			sound.Initialize(volume, pitch);
		}
	}

	public void Play(string soundName)
	{
	    CheckForSound(soundName, true, sound => sound.Play());
	}

	public void Stop(string soundName)
	{
	    CheckForSound(soundName, false, sound => sound.Stop());
	}

	public void Pause(string soundName)
	{
	    CheckForSound(soundName, false, sound => sound.Pause());
	}

	public void StopAll()
	{
		foreach(string soundName in soundsPlaying.Keys)
		{
			Stop(soundName);
		}
	}

	public void PauseAll()
	{
		foreach(string soundName in soundsPlaying.Keys)
		{
			Pause(soundName);
		}
	}

	private void CheckForSound(string soundName, bool isPlaying, System.Action<AudioSource> action)
	{
	    if (!soundsByName.TryGetValue(soundName, out Sound sound))
	    {
	        Debug.LogWarning("Sound " + soundName + " not found!");
	        return;
	    }

	    if (isPlaying) soundsPlaying[soundName] = sound;
	    else if (soundsPlaying.ContainsKey(soundName)) soundsPlaying.Remove(soundName);
	    action?.Invoke(sound.source);
	}

	public void FadeAllSounds(float duration, bool fadeIn)
	{
		foreach (Sound sound in soundsPlaying.Values)
		{
		    StartCoroutine(FadeSound(sound, duration, fadeIn));
		}
	}

	public IEnumerator FadeSound(Sound sound, float duration, bool fadeIn)
	{
	    float startVolume = fadeIn ? 0f : sound.source.volume;
	    float targetVolume = fadeIn ? sound.source.volume : 0f;
	    float timer = 0f;
	    while (timer < duration)
	    {
	        sound.UpdateVolume(Mathf.Lerp(startVolume, targetVolume, timer / duration));
	        timer += Time.deltaTime;
	        yield return null;
	    }
	    sound.UpdateVolume(targetVolume);
	}

	public void FadeIn(float duration)
	{
		FadeAllSounds(duration, true);
	}

	public void FadeIn(float duration, List<string> soundNames)
	{
		foreach(string soundName in soundNames)
		{
			Play(soundName);
		}
		FadeAllSounds(duration, true);
	}

	public void FadeOut(float duration)
	{
		FadeAllSounds(duration, false);
	}
}