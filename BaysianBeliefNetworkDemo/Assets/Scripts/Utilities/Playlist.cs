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
	private int trackNumber = 0;
	private AudioManager audioManager;

	private void Awake()
	{
		audioManager = FindObjectOfType<AudioManager>();
	}

	private void Start()
	{
		if (playOnAwake) Reset();
	}

	public void Play()
	{
		if (trackNumber > 0) audioManager.FadeOutMusic(fadeTime);
		if (trackNumber < trackList.Count)
		{
			audioManager.FadeInSong(trackList[trackNumber], fadeTime);
			trackNumber++;
			StartCoroutine(WaitForNextSong());
		}
		else if (loop) Reset();
	}

	public void Reset()
	{
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

	private IEnumerator WaitForNextSong()
	{
		yield return new WaitForSeconds(audioManager.GetSongLength(trackList[trackNumber-1]));
		Play();
	}
}