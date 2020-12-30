using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class SettingsManager : MonoBehaviour
    {
        public Toggle fullscreenToggle;
        public Dropdown resolutionDropdown;
        public Slider volumeSlider;

        public AudioSource gameAudio;
        public Resolution[] resolutions;

        void OnEnable()
        {
            fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
            resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
            volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });

            resolutions = Screen.resolutions;
            foreach (Resolution resolution in resolutions)
            {
                resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
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

        public void OnVolumeChange()
        {
            gameAudio.volume = volumeSlider.value;
        }
    }
}
