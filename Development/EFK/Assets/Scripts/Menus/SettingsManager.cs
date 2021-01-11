using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menus
{
    public class SettingsManager : MonoBehaviour
    {
        public Toggle fullscreenToggle;
        public Dropdown resolutionDropdown;
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider SFXVolumeSlider;
        public Slider voiceVolumeSlider;

        public AudioMixer audioMixer;
        public List<Resolution> resolutions;

        void Start()
        {
            fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
            resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
            masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
            musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
            SFXVolumeSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });
            voiceVolumeSlider.onValueChanged.AddListener(delegate { OnVoiceVolumeChange(); });

            resolutions = new List<Resolution>();
            int currentResolution = 0;

            foreach(Resolution res in Screen.resolutions) {
                if (res.refreshRate == Screen.currentResolution.refreshRate) {
                    resolutions.Add(res);
                    resolutionDropdown.options.Add(
                        new Dropdown.OptionData(res.width + "x" + res.height));
                    if (res.width == Screen.currentResolution.width &&
                        res.height == Screen.currentResolution.height) currentResolution = resolutions.Count - 1; // current resolution index
                }
            }

            resolutionDropdown.value = currentResolution;
            resolutionDropdown.RefreshShownValue();

            if (PlayerPrefs.HasKey("MasterVolume"))
            {
                float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
                audioMixer.SetFloat("Master", Mathf.Log10(masterVolume) * 20);
                masterVolumeSlider.value = masterVolume;
            }
            if (PlayerPrefs.HasKey("MusicVolume"))
            {
                float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
                audioMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
                musicVolumeSlider.value = musicVolume;
            }
            if (PlayerPrefs.HasKey("SFXVolume"))
            {
                float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
                audioMixer.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
                SFXVolumeSlider.value = sfxVolume;
            }
            if (PlayerPrefs.HasKey("VoiceVolume"))
            {
                float voiceVolume = PlayerPrefs.GetFloat("VoiceVolume");
                audioMixer.SetFloat("Voice", Mathf.Log10(voiceVolume) * 20);
                voiceVolumeSlider.value = voiceVolume;
            }
        }

        public void OnFullscreenToggle()
        {
            Screen.fullScreen = fullscreenToggle.isOn;
        }

        public void OnResolutionChange()
        {
            Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        }

        public void OnMasterVolumeChange()
        {
            audioMixer.SetFloat("Master", Mathf.Log10(masterVolumeSlider.value) * 20);
            PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        }
        
        public void OnMusicVolumeChange()
        {
            audioMixer.SetFloat("Music", Mathf.Log10(musicVolumeSlider.value) * 20);
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        }
        
        public void OnSFXVolumeChange()
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(SFXVolumeSlider.value) * 20);
            PlayerPrefs.SetFloat("SFXVolume", SFXVolumeSlider.value);
        }
        
        public void OnVoiceVolumeChange()
        {
            audioMixer.SetFloat("Voice", Mathf.Log10(voiceVolumeSlider.value) * 20);
            PlayerPrefs.SetFloat("VoiceVolume", voiceVolumeSlider.value);
        }
    }
}
