using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private Sprite customFishImage;

    [Header("Movement")]
    [SerializeField] private float swimSpeed = 2f;
    [SerializeField] private float wanderChangeInterval = 25f; // how often it picks a new direction

    [Header("Mouse Avoidance")]
    [SerializeField] private float fleeRadius = 2f;       // how close mouse must be to trigger flee
    [SerializeField] private float fleeSpeed = 3.5f;      // fast enough to escape casually, catchable if you try

    private Vector2 wanderTarget;
    private float wanderTimer;
    private Camera mainCamera;
    private Vector2 screenMin;
    private Vector2 screenMax;
    private float padding = 0.5f; // keeps fish fully inside screen edges

    void Awake()
    {
        if (customFishImage != null)
            GetComponent<SpriteRenderer>().sprite = customFishImage;
    }

    void Start()
    {
        mainCamera = Camera.main;
        CalculateBounds();
        PickNewWanderTarget();
    }

    void Update()
    {
        CalculateBounds(); // recalculate in case window resizes

        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float distToMouse = Vector2.Distance(transform.position, mouseWorld);

        if (distToMouse < fleeRadius)
        {
            Flee(mouseWorld);
        }
        else
        {
            Wander();
        }

        ClampToBounds();
    }

    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f || Vector2.Distance(transform.position, wanderTarget) < 0.2f)
            PickNewWanderTarget();

        MoveTowards(wanderTarget, swimSpeed);
    }

    void Flee(Vector2 mouseWorld)
    {
        // move in the opposite direction from the mouse
        Vector2 fleeDirection = ((Vector2)transform.position - mouseWorld).normalized;
        Vector2 fleeTarget = (Vector2)transform.position + fleeDirection * 2f;
        fleeTarget = ClampPoint(fleeTarget);

        MoveTowards(fleeTarget, fleeSpeed);
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 newPos = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // flip sprite to face movement direction
         float moveDir = target.x - transform.position.x;
        if (Mathf.Abs(moveDir) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = moveDir > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        transform.position = newPos;
    }

    void PickNewWanderTarget()
    {
        wanderTarget = new Vector2(
            Random.Range(screenMin.x, screenMax.x),
            Random.Range(screenMin.y, screenMax.y)
        );
        wanderTimer = wanderChangeInterval;
    }

    void CalculateBounds()
    {
        screenMin = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        screenMax = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        screenMin += Vector2.one * padding;
        screenMax -= Vector2.one * padding;
    }

    void ClampToBounds()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, screenMin.x, screenMax.x);
        pos.y = Mathf.Clamp(pos.y, screenMin.y, screenMax.y);
        transform.position = pos;
    }

    Vector2 ClampPoint(Vector2 point)
    {
        point.x = Mathf.Clamp(point.x, screenMin.x, screenMax.x);
        point.y = Mathf.Clamp(point.y, screenMin.y, screenMax.y);
        return point;
    }
}