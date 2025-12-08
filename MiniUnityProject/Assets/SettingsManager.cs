using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource; // Drag your BackgroundMusic AudioSource here

    [Header("UI Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    void Start()
    {
        // Load saved settings
        LoadSettings();

        // Set initial slider values
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;

        // Apply loaded volumes
        ApplyMusicVolume();
    }

    // Called when Master Volume slider changes
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        ApplyMusicVolume();
        SaveSettings();
    }

    // Called when Music Volume slider changes
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        ApplyMusicVolume();
        SaveSettings();
    }

    // Called when SFX Volume slider changes
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        SaveSettings();
    }

    private void ApplyMusicVolume()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = masterVolume * musicVolume;
        }
    }

    // Save settings to PlayerPrefs
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    // Load settings from PlayerPrefs
    private void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
}