using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class FishTypeConfig
{
    public Fish.MovementType movementType;
    public int count;
    public float swimSpeed = 2f;
    public float fleeRadius = 2f;
    public float fleeSpeed = 3.5f;
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
    [SerializeField] private int currentLevel = 1;

    [Header("World Bounds")]
    [SerializeField] private Vector2 worldMin = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 worldMax = new Vector2(10f, 5f);

    [Header("Fish of the Day UI")]
    [SerializeField] private GameObject fullPanel;
    [SerializeField] private Image fishImage;
    [SerializeField] private GameObject minimizedView;
    [SerializeField] private Image miniFishImage;
    [SerializeField] private TextMeshProUGUI counterText;

    [Header("Sprites")]
    [SerializeField] private Sprite wanderSprite;
    [SerializeField] private Sprite traverseSprite;
    [SerializeField] private Sprite crabSprite;
    [SerializeField] private Sprite octopusSprite;

    private float padding = 0.5f;
    private int totalTarget;
    private int caughtTarget;
    private Fish.MovementType currentTargetType;

    void Start()
    {
        SpawnFishForLevel(currentLevel);
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

    // --- UI ---

    void InitializeUI(Fish.MovementType targetType, int total)
    {
        totalTarget = total;
        caughtTarget = 0;

        Sprite targetSprite = GetSprite(targetType);
        fishImage.sprite = targetSprite;
        miniFishImage.sprite = targetSprite;

        fullPanel.SetActive(true);
        minimizedView.SetActive(false);

        UpdateCounter();
    }

    public void OnMinimizeClicked()
    {
        fullPanel.SetActive(false);
        minimizedView.SetActive(true);
    }

    public void OnMaximizeClicked()
    {
        fullPanel.SetActive(true);
        minimizedView.SetActive(false);
    }

    public void RegisterCatch(Fish.MovementType caughtType)
    {
        if (caughtType != currentTargetType) return;
        caughtTarget = Mathf.Min(caughtTarget + 1, totalTarget);
        UpdateCounter();
    }

    void UpdateCounter()
    {
        counterText.text = $"{caughtTarget}/{totalTarget}";
    }

    Sprite GetSprite(Fish.MovementType type)
    {
        return type switch
        {
            Fish.MovementType.Wander   => wanderSprite,
            Fish.MovementType.Traverse => traverseSprite,
            Fish.MovementType.Crab     => crabSprite,
            Fish.MovementType.Octopus  => octopusSprite,
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
        GameObject obj = Instantiate(fishPrefab, spawnPos, Quaternion.identity);
        Fish fish = obj.GetComponent<Fish>();

        fish.SetMovementType(config.movementType);
        fish.SetStats(config.swimSpeed, config.fleeRadius, config.fleeSpeed);
        fish.SetWorldBounds(worldMin, worldMax);
        fish.SetSprite(GetSprite(config.movementType));
    }
}