using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[System.Serializable]
public class FishTypeConfig
{
    public Fish.MovementType movementType;
    public int count;
    public float swimSpeed = 2f;
    public float fleeRadius = 2f;
    public float fleeSpeed = 3.5f;
    public float clickRadius = 1f; // add this
}

[System.Serializable]
public class LevelConfig
{
    public int level;
    public Fish.MovementType fishOfTheDay;
    public FishTypeConfig[] fishTypes;
}

public class FishSpawner : MonoBehaviour
{
    [Header("Fish")]
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private LevelConfig[] levels;

    [Header("World Bounds")]
    [SerializeField] private Vector2 worldMin = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 worldMax = new Vector2(10f, 5f);

    [Header("Ocean UI")]
    [SerializeField] private GameObject fullPanel;
    [SerializeField] private GameObject cookedPanel;
    [SerializeField] private Image fishImage;
    [SerializeField] private GameObject minimizedView;
    [SerializeField] private Image miniFishImage;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private GameObject backToLighthouse;
    [SerializeField] private GameObject cookedMessage;
    [SerializeField] private GameObject slider;

    [Header("Catch Panel")]
    [SerializeField] private GameObject catchPanel;
    [SerializeField] private Image catchFishImage;
    [SerializeField] private TextMeshProUGUI catchBioText;
    [SerializeField] private Button keepButton;

    [Header("Sprites")]
    [SerializeField] private Sprite wanderSprite;
    [SerializeField] private Sprite traverseSprite;
    [SerializeField] private Sprite crabSprite;
    [SerializeField] private Sprite octopusSprite;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite octopusAltSprite;
    
    [Header("Ending")]
    [SerializeField] private GameObject fadeInButton;
    [SerializeField] private GameObject endingText;

    [SerializeField] private RuntimeAnimatorController octopusAnimator;
    [SerializeField] private RuntimeAnimatorController octopusAnimatorAlt;
    
    [SerializeField] private Transform cameraTarget; // where you want the camera to move to
    [SerializeField] private float cameraMoveSpeed = 9f;
    
    private float padding = 0.5f;
    private int totalTarget;
    private int caughtTarget;
    private Fish.MovementType currentTargetType;
    private bool catchPanelOpen = false;
    private Fish pendingFish;
    private bool levelFinished = false;
    private bool fadeStarted = false;
    public static int currentLevel = 0;

    void Start()
    {
        catchPanel.SetActive(false);
        backToLighthouse.SetActive(false);
        cookedMessage.SetActive(false);
        cookedPanel.SetActive(false);
        fadeInButton.SetActive(false);
        endingText.SetActive(false);
        if (currentLevel < 5){
            SpawnFishForLevel(currentLevel);
        }
        else{
            LevelConfig config = System.Array.Find(levels, l => l.level == currentLevel);
            totalTarget = 1;
            caughtTarget = 0;

            Sprite targetSprite = GetSprite(config.fishOfTheDay);
            fishImage.sprite = targetSprite;
            miniFishImage.sprite = targetSprite;

            cookedPanel.SetActive(true);
            fullPanel.SetActive(false);
            minimizedView.SetActive(false);

            UpdateCounter();
        }
    }

    public void LoadLevel(int level){
        currentLevel = level;
    }

    public void LeaveLevel(){
        SceneManager.LoadScene("Lighthouse");
    }

    public void SpawnFishForLevel(int level)
    {
        LevelConfig config = System.Array.Find(levels, l => l.level == level);
        if (config == null)
        {
            Debug.LogWarning($"No config found for level {level}");
            return;
        }

        currentTargetType = config.fishOfTheDay;

        int targetCount = 0;
        foreach (FishTypeConfig ft in config.fishTypes)
            if (ft.movementType == config.fishOfTheDay)
                targetCount += ft.count;

        InitializeUI(config.fishOfTheDay, targetCount);

        foreach (FishTypeConfig fishType in config.fishTypes)
            for (int i = 0; i < fishType.count; i++)
                SpawnFish(fishType);
    }

    // --- Catch Panel ---

    public void OnFishClicked(Fish fish)
    {
        if (catchPanelOpen) return;
        if (fullPanel.activeSelf) return;

        pendingFish = fish;
        catchPanelOpen = true;
        catchFishImage.preserveAspect = true;
        if (fish.GetMovementType() == Fish.MovementType.Octopus && (currentLevel == 3 || currentLevel == 4))
            catchFishImage.sprite = octopusAltSprite;
        else
            catchFishImage.sprite = GetSprite(fish.GetMovementType());
        catchBioText.text = GetBio(fish.GetMovementType());
        keepButton.gameObject.SetActive(fish.GetMovementType() == currentTargetType);

        catchPanel.SetActive(true);
        fish.gameObject.SetActive(false);
    }

    public void OnRelease()
    {
        if (pendingFish != null)
            pendingFish.gameObject.SetActive(true);
        catchPanel.SetActive(false);
        catchPanelOpen = false;
        pendingFish = null;
    }

    public void OnKeep()
    {
        if (pendingFish != null)
            Destroy(pendingFish.gameObject);
        RegisterCatch(currentTargetType);
        catchPanel.SetActive(false);
        catchPanelOpen = false;
        pendingFish = null;
    }

    string GetBio(Fish.MovementType type)
    {
        return type switch
        {
            Fish.MovementType.Wander   => "make me into a fish stick",
            Fish.MovementType.Traverse => "no thoughts, head full of bubbles",
            Fish.MovementType.Crab     => "i taste great in chowder",
            Fish.MovementType.Octopus  => (currentLevel == 3 || currentLevel == 4) 
                ? "it's staring."
                : "i definitely have more heart than ur job interviewer",
            
            _                          => "Unknown creature."
        };
    }

    // --- Fish of the Day UI ---

    void InitializeUI(Fish.MovementType targetType, int total)
    {
        totalTarget = total;
        caughtTarget = 0;

        Sprite targetSprite = GetSprite(targetType);
        fishImage.sprite = targetSprite;
        fishImage.preserveAspect = true;
        miniFishImage.sprite = targetSprite;
        miniFishImage.preserveAspect = true;

        fullPanel.SetActive(true);
        minimizedView.SetActive(false);

        UpdateCounter();
    }

    public void OnMinimizeClicked()
    {
        if (currentLevel < 5){
            fullPanel.SetActive(false);
            minimizedView.SetActive(true);
        }
        else{
            cookedPanel.SetActive(false);
            minimizedView.SetActive(true);
            if (!fadeStarted){
                fadeStarted = true;
                StartCoroutine(FadeInButton());
            }
        }
    }

    public void OnMaximizeClicked()
    {
        if (!levelFinished){
            if (currentLevel < 5){
                fullPanel.SetActive(true);
                minimizedView.SetActive(false);
            }
            else{
                cookedPanel.SetActive(true);
                minimizedView.SetActive(false);
            }
        }
    }

    IEnumerator FadeInButton()
    {
        yield return new WaitForSeconds(10f);
        fadeInButton.SetActive(true);
        CanvasGroup cg = fadeInButton.GetComponent<CanvasGroup>();
        if (cg == null) cg = fadeInButton.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        float duration = 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    public void OnEndingButtonClicked()
    {
        // hide everything
        fadeInButton.SetActive(false);
        minimizedView.SetActive(false);
        cookedPanel.SetActive(false);
        backToLighthouse.SetActive(false);
        slider.SetActive(false);

        // show ending text
        StartCoroutine(MoveCamera());
    }
    
    IEnumerator MoveCamera()
    {
        Transform cam = Camera.main.transform;
        Vector3 startPos = cam.position;
        Vector3 endPos = cameraTarget.position;
        float elapsed = 0f;
        float duration = 3f; // how long the move takes

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }
        cam.position = endPos;
        FindObjectOfType<OceanBgProgression>().uhOhBigBug();
    }

    void RegisterCatch(Fish.MovementType caughtType)
    {
        if (caughtType != currentTargetType) return;
        caughtTarget = Mathf.Min(caughtTarget + 1, totalTarget);
        UpdateCounter();
    }

    void UpdateCounter()
    {
        counterText.text = $"{caughtTarget}/{totalTarget}";
        if (caughtTarget==totalTarget){
            levelFinished = true;
            minimizedView.SetActive(false);
            backToLighthouse.SetActive(true);
            fullPanel.SetActive(false);
        }
    }

    Sprite GetSprite(Fish.MovementType type)
    {
        return type switch
        {
            Fish.MovementType.Wander   => wanderSprite,
            Fish.MovementType.Traverse => traverseSprite,
            Fish.MovementType.Crab     => crabSprite,
            Fish.MovementType.Octopus  => octopusSprite,
            Fish.MovementType.Empty  => emptySprite,
            _                          => null
        };
    }

    // --- Spawning ---

    Vector2 GetSpawnPosition(Fish.MovementType movementType)
    {
        Vector2 min = worldMin + Vector2.one * padding;
        Vector2 max = worldMax - Vector2.one * padding;
        float sliderOffset = 1.0f;

        if (movementType == Fish.MovementType.Crab)
        {
            float crabMaxY = Mathf.Lerp(min.y + sliderOffset, max.y, 0.1f);
            return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y + sliderOffset, crabMaxY));
        }

        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y + sliderOffset, max.y));
    }

    void SpawnFish(FishTypeConfig config)
    {
        Vector2 spawnPos = GetSpawnPosition(config.movementType);
        GameObject obj = Instantiate(fishPrefab, new Vector3(spawnPos.x, spawnPos.y, -1f), Quaternion.identity);
        Fish fish = obj.GetComponent<Fish>();
        fish.GetComponent<SpriteRenderer>().sortingOrder = 1;

        fish.SetMovementType(config.movementType);
        fish.SetStats(config.swimSpeed, config.fleeRadius, config.fleeSpeed);
        fish.SetWorldBounds(worldMin, worldMax);
        fish.SetSprite(GetSprite(config.movementType));
        fish.GetComponent<CircleCollider2D>().radius = config.clickRadius;
        fish.SetSpawner(this); // give each fish a reference back to spawner
        
        if (config.movementType == Fish.MovementType.Crab){
            obj.transform.localScale = new Vector3(0.11f, 0.11f, 0.11f);
        }
        if (config.movementType == Fish.MovementType.Traverse){
            obj.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
        }
        if (config.movementType == Fish.MovementType.Wander){
            obj.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
        }


        if (config.movementType == Fish.MovementType.Octopus)
        {
            if (currentLevel == 3 || currentLevel == 4)
                fish.SetAnimation(octopusAnimatorAlt);
            else
                fish.SetAnimation(octopusAnimator);
        }
        else
            fish.SetSprite(GetSprite(config.movementType));
    }
}