using UnityEngine;
using UnityEngine.SceneManagement;

public class revampedAudio : MonoBehaviour
{
    public static revampedAudio Instance;

    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClickClip;

    [Header("Music Clips")]
    [SerializeField] private AudioClip underwaterMusic;
    [SerializeField] private AudioClip underwaterQuietMusic;
    [SerializeField] private AudioClip theEndMusic;
    [SerializeField] private AudioClip uhOhSpaghettio;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "Ocean")
        {
            UpdateMusic();
        }
        else if (scene.name == "Lighthouse" && FishSpawner.currentLevel == 4)
        {
            PlayMusic(theEndMusic);
            // Do nothing — LighthouseManager handles music via UpdateMusic() in Awake
        }
        else
        {
            StopMusic();
        }
    }

    // --- Music ---
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic() => musicSource?.Stop();

    // --- SFX ---
    public void PlayButtonSound()
    {
        if (sfxSource != null && buttonClickClip != null)
            sfxSource.PlayOneShot(buttonClickClip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }
    
    public void PlayEnding()
    {
        if (sfxSource != null && uhOhSpaghettio != null)
        {
            sfxSource.PlayOneShot(uhOhSpaghettio); 
            Debug.Log("Button sound played!");
        }
            
    }
    public void UpdateMusic()
    {
        switch (FishSpawner.currentLevel)
        {
            case 0:
                break;
            case 1:
            case 2:
            case 3:
                PlayMusic(underwaterMusic);
                break;
            case 4:
                PlayMusic(underwaterQuietMusic);
                break;
            case 5:
                PlayMusic(theEndMusic);
                break;
        }
    }
}
