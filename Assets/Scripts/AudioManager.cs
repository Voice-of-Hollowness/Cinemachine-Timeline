using System.Collections;
using Assets.Scripts;
using UnityEngine;



public class AudioManager : MonoBehaviour
{
    public enum AudioChannel
    {
        Master,
        SFX,
        Music
    }

    public float masterVolumePercent  {get; private set;}
    public float sfxVolumePercent  {get; private set;}
    public float musicVolumePercent  {get; private set;}
    [SerializeField]
    Transform audioListener;
    [SerializeField]
    Transform playerT;

    SoundLib Library;

    private AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }else{
            instance = this;

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music Source" + (i + 1)); 
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
        }

        GameObject newSfx2DSource = new GameObject("2D sfx source");
        sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
        newSfx2DSource.transform.parent = transform;

        audioListener = FindObjectOfType<AudioListener>().transform;
        if (FindObjectOfType<Player>() != null)
        {
            playerT = FindObjectOfType<Player>().transform;
        }

        Library = GetComponent<SoundLib>();
        audioListener = FindObjectOfType<AudioListener>().transform;
        masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
        sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
        musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
    }

    private void Update()
    {
        if (playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }


    public void SetVolume(float volPercent, AudioChannel channel)
    {

        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volPercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volPercent;
                break;
            case AudioChannel.SFX:
                sfxVolumePercent = volPercent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();
        StartCoroutine(AnimateMusicCrossfade(fadeDuration));

    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(Library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(Library.GetClipFromName(soundName),sfxVolumePercent*masterVolumePercent);
    }


    IEnumerator AnimateMusicCrossfade(float duration)
    {
        var percent = 0f;
        var shift = Time.fixedDeltaTime * (1 / duration);
        
         while (percent < 1)
        {
            percent += shift;

            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * musicVolumePercent, percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * musicVolumePercent, 0, percent);
            yield return null;
        }


    }
}
