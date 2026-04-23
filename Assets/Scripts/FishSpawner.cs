using UnityEngine;

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
    public FishTypeConfig[] fishTypes;
}

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private LevelConfig[] levels;
    [SerializeField] private int currentLevel = 1;

    private Camera mainCamera;
    private float padding = 0.5f;

    void Start()
    {
        mainCamera = Camera.main;
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

        foreach (FishTypeConfig fishType in config.fishTypes)
        {
            for (int i = 0; i < fishType.count; i++)
                SpawnFish(fishType);
        }
    }

    Vector2 GetSpawnPosition(Fish.MovementType movementType)
    {
        Vector2 screenMin = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 screenMax = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        screenMin += Vector2.one * padding;
        screenMax -= Vector2.one * padding;

        if (movementType == Fish.MovementType.Crab)
        {
            float crabMaxY = Mathf.Lerp(screenMin.y, screenMax.y, 0.2f);
            return new Vector2(
                Random.Range(screenMin.x, screenMax.x),
                Random.Range(screenMin.y, crabMaxY)
            );
        }

        return new Vector2(
            Random.Range(screenMin.x, screenMax.x),
            Random.Range(screenMin.y, screenMax.y)
        );
    }

    void SpawnFish(FishTypeConfig config)
    {
        Vector2 spawnPos = GetSpawnPosition(config.movementType);
        GameObject obj = Instantiate(fishPrefab, spawnPos, Quaternion.identity);
        Fish fish = obj.GetComponent<Fish>();

        fish.SetMovementType(config.movementType);
        fish.SetStats(config.swimSpeed, config.fleeRadius, config.fleeSpeed);
    }
}