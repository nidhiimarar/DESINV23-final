using UnityEngine;

public class revampedAudio : MonoBehaviour
{
    // Static instance allows other scripts to access this without a direct reference
    public static revampedAudio Instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip buttonClickClip;

    private void Awake()
    {
        // Singleton Pattern: Ensures only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object alive between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonSound()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
            sfxSource.PlayOneShot(buttonClickClip);
        }
        else
        {
            Debug.LogWarning("SoundManager: Missing AudioSource or Clip!");
        }
    }
}


/*using UnityEngine;

public class revampedAudio : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource2;
    public static revampedAudio Instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            
        }
        else
        {
            Destroy(gameObject); // prevent duplicates
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick()
    {
        Debug.Log("Button clicked!!!!!!!");
        audioSource.PlayOneShot(audioSource.clip);
    }
}*/