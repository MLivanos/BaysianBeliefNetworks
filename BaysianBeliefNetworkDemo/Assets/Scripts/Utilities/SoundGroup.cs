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
	[Range(0f,1f)] public float volumeMultiplier = 1f;
	[Range(0f,1f)] public float pitchMultiplier = 1f;
	[HideInInspector] public AudioSource source;
	[HideInInspector] public float duration;

	public void AttachAudioSource(float volume, float pitch)
	{
		source.clip = clip;
		source.volume = volume;
		source.pitch = pitch;
		source.loop = loop;
		source.spatialBlend = 0f;
		source.spatialize = false;
		duration = clip.length;
	}

	public void UpdateVolume(float newVolume)
	{
		source.volume = volumeMultiplier*newVolume;
	}

	public void UpdatePitch(float newPitch)
	{
		source.pitch = pitchMultiplier*newPitch;
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

	private void Awake()
	{
		foreach(Sound sound in sounds)
		{
			soundsByName.Add(sound.name, sound);
			sound.source = gameObject.AddComponent<AudioSource>();
			sound.AttachAudioSource(volume, pitch);
		}
	}

	public void UpdateVolume(float newVolume)
	{
		volume = newVolume;
		foreach(Sound sound in soundsPlaying.Values)
		{
			sound.UpdateVolume(volume);
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
		foreach(Sound sound in GetPlayingSounds())
		{
			Stop(sound.name);
		}
	}

	public void PauseAll()
	{
		foreach(Sound sound in GetPlayingSounds())
		{
			Pause(sound.name);
		}
	}

	private void CheckForSound(string soundName, bool isPlaying, System.Action<AudioSource> action)
	{
	    if (!soundsByName.TryGetValue(soundName, out Sound sound))
	    {
	        Debug.LogWarning("Sound " + soundName + " not found!");
	        return;
	    }

	    if (isPlaying)
	    {
	    	soundsPlaying[soundName] = sound;
	    	soundsPlaying[soundName].UpdateVolume(volume);
	    	soundsPlaying[soundName].UpdatePitch(pitch);
	    }
	    else if (soundsPlaying.ContainsKey(soundName)) soundsPlaying.Remove(soundName);
	    action?.Invoke(sound.source);
	}

	public void FadeAllSounds(float duration, bool fadeIn)
	{
		foreach (Sound sound in GetPlayingSounds())
		{
		    StartCoroutine(FadeSound(sound, duration, fadeIn));
		}
		if (!fadeIn) soundsPlaying.Clear();
	}

	public IEnumerator FadeSound(Sound sound, float duration, bool fadeIn)
	{
	    float startVolume = fadeIn ? 0f : volume;
	    float targetVolume = fadeIn ? volume : 0f;
	    float timer = 0f;
	    while (timer < duration)
	    {
	        sound.UpdateVolume(Mathf.Lerp(startVolume, targetVolume, timer / duration));
	        timer += Time.deltaTime;
	        yield return null;
	    }
	    sound.UpdateVolume(targetVolume);
	    if (!fadeIn) Stop(sound.name);
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

	public void FadeOut(float duration, List<string> soundNames)
	{
		foreach(string soundName in soundNames)
		{
			StartCoroutine(FadeSound(soundsByName[soundName], duration, false));
		}
	}

	private List<Sound> GetPlayingSounds()
	{
		return new List<Sound>(soundsPlaying.Values);
	}

	public float GetSongLength(string song)
	{
		return soundsByName[song].duration;
	}

	public float GetProgress(string song)
	{
		return soundsByName[song].source.time;
	}

	public void SetTime(string song, float time)
	{
		soundsByName[song].source.time = time;
	}
}