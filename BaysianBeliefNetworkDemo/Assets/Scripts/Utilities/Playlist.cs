using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playlist : MonoBehaviour
{
	[SerializeField] private List<string> trackList;
	[SerializeField] private float fadeTime;
	[SerializeField] private bool shuffle;
	[SerializeField] private bool loop = true;
	[SerializeField] private bool playOnAwake = true;
	[SerializeField] private bool autoPlay = true;
	private Coroutine waitForNextSong;
	private float timeElapsedWhenPaused = 0f;
	private int trackNumber = 0;
	private AudioManager audioManager;
	private bool isPlaying = false;

	private void Start()
	{
		audioManager = AudioManager.instance;
		if (playOnAwake) Reset();
	}

	public void Play()
	{
		if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
		isPlaying = true;
		if (trackNumber > 0) audioManager.FadeOutMusic(fadeTime);
		if (trackNumber < trackList.Count)
		{
			if (waitForNextSong != null) StopCoroutine(waitForNextSong);
			audioManager.FadeInSong(trackList[trackNumber], fadeTime);
			trackNumber++;
			if (autoPlay) waitForNextSong = StartCoroutine(WaitForNextSong(audioManager.GetSongLength(trackList[trackNumber-1])));
		}
		else if (loop) Reset();
	}

	public void Reset()
	{
		Pause();
		trackNumber = 0;
		if (shuffle) Shuffle(trackList);
		Play();
	}

	static void Shuffle(List<string> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

	private IEnumerator WaitForNextSong(float duration)
	{
		yield return new WaitForSeconds(duration);
		Play();
	}

	public void Pause()
	{
		if (!isPlaying) return;
		if (waitForNextSong != null) StopCoroutine(waitForNextSong);
		audioManager.PauseMusic();
		timeElapsedWhenPaused = audioManager.GetSongProgress(trackList[trackNumber-1]);
		StopAllCoroutines();
		isPlaying = false;
	}

	public void Resume()
	{
		if (waitForNextSong != null) StopCoroutine(waitForNextSong);
		isPlaying = true;
		audioManager.PlayMusic(trackList[trackNumber-1]);
		if (autoPlay) StartCoroutine(WaitForNextSong(audioManager.GetSongLength(trackList[trackNumber-1]) - timeElapsedWhenPaused));
	}

	public void OnDestroy()
	{
		Pause();
	}
}