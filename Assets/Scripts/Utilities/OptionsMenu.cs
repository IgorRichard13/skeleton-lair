using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

namespace R2 {
    public class OptionsMenu : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public TMP_Dropdown resolutionDropdown;
        Resolution[] resolutions;

        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        [SerializeField] private Toggle _fullscreenToggle;

        [SerializeField] private TMP_Dropdown _qualityDropdown;

        public void Start()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            foreach (Resolution resolution in resolutions)
            {
                string option = resolution.width + " x " + resolution.height + " @ " + resolution.refreshRate + "hz"; ;
                options.Add(option);

                if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetActual()
        {
            audioMixer.GetFloat("Volume", out float Volume);
            audioMixer.GetFloat("MusicVolume", out float musicVolume);
            audioMixer.GetFloat("SFXVolume", out float sfxVolume);

            _masterVolumeSlider.value = Volume;
            _musicVolumeSlider.value = musicVolume;
            _sfxVolumeSlider.value = sfxVolume;

            _qualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", volume);
        }
        
        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFXVolume", volume);
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}