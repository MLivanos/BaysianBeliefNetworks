using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderController : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject volumeImage;
    [SerializeField] private GameObject muteImage;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SetMuteImage();

        masterSlider.onValueChanged.AddListener(value => audioManager.AdjustMasterVolume(value));
        musicSlider.onValueChanged.AddListener(value => audioManager.AdjustMusicVolume(value));
        sfxSlider.onValueChanged.AddListener(value => audioManager.AdjustSFXVolume(value));
    }

    public void ToggleMute()
    {
        audioManager.Mute();
        SetMuteImage();
    }

    private void SetMuteImage()
    {
        if (audioManager.IsMuted())
        {
            volumeImage.SetActive(false);
            muteImage.SetActive(true);
        }
        else
        {
            volumeImage.SetActive(true);
            muteImage.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
    }
}
