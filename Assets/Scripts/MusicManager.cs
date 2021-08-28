using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;
    private string sceneName;
    private void Start()
    {
        OnLvlLoad(0);
    }

    void OnLvlLoad(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
    if (newSceneName==sceneName)
    {
        sceneName = newSceneName;
        Invoke("PlayMusic",.2f);
    }
}

    void PlayMusic()
    {
        AudioClip clipToPlay = null;

        if (sceneName == "Menu")
        {
            clipToPlay = menuTheme;
        }
        else if(sceneName=="Shootah")
        {
            clipToPlay = mainTheme;
        }

        if (clipToPlay!=null)
        {
            AudioManager.instance.PlayMusic(clipToPlay,2f);
            Invoke("PlayMusic",clipToPlay.length);
        }
    }
}
