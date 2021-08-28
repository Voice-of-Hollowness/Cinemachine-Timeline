using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;

    public Slider[] VolumeSliders;
    public Toggle[] ResolutionToggles;
    public Toggle FullscreenToggle;
    public int[] screenWidths;

    private int activeScreenResIndex;


    public void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1);
        //Debug.Log(gameObject.name);
        VolumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        VolumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        VolumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        for (int i = 0; i < ResolutionToggles.Length; i++)
        {
            ResolutionToggles[i].isOn = i == activeScreenResIndex;
        }

        FullscreenToggle.isOn = isFullscreen;
    }

    public void Play()
    {
        SceneManager.LoadScene("Shootah");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void SetScreenResolution(int i)
    {
        if (ResolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths [i], (int)(screenWidths [i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
        }
        
    }

    public void SetFullsreen(bool isFullscreen)
    {
        for (int i = 0; i < ResolutionToggles.Length; i++)
        {
            ResolutionToggles [i].interactable = !isFullscreen;
        }

        if (isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }

        PlayerPrefs.SetInt("fullscreen", ((isFullscreen)? 1:0));
        PlayerPrefs.Save();

    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }
    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);

    }
    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.SFX);

    }
}
