using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlaylistUIBinder : MonoBehaviour
{
    [SerializeField] private Button skipButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text trackNameDisplay;

    private Playlist currentPlaylist;

    private void Start()
    {
        StartCoroutine(InitializePlaylistReference());
    }

    private IEnumerator InitializePlaylistReference()
    {
        yield return null;

        currentPlaylist = FindObjectOfType<Playlist>();

        if (currentPlaylist == null)
        {
            Debug.LogError("No Playlist found in the scene after initialization.");
            yield break;
        }

        skipButton.onClick.AddListener(currentPlaylist.Skip);
        backButton.onClick.AddListener(currentPlaylist.Back);
        UpdateTitle();
        currentPlaylist.AddListener(this);
    }

    public void UpdateTitle()
    {
        trackNameDisplay.text = currentPlaylist.GetTrackTitle();
    }
}