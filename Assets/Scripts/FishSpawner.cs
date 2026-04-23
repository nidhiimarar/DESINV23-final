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

    [Header("World Bounds (independent of camera)")]
    [SerializeField] private Vector2 worldMin = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 worldMax = new Vector2(10f, 5f);

    private float padding = 0.5f;

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

        foreach (FishTypeConfig fishType in config.fishTypes)
            for (int i = 0; i < fishType.count; i++)
                SpawnFish(fishType);
    }

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
        fish.SetWorldBounds(worldMin, worldMax); // pass world bounds in
    }
}