using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNoise : MonoBehaviour
{
	[SerializeField] private string clipId;
	[SerializeField] private bool sfx = true;
	private AudioManager audioManager;

	private void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
		if (sfx) audioManager.PlayEffect(clipId);
		else audioManager.PlayMusic(clipId);
	}
}