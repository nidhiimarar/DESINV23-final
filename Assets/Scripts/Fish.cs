using UnityEngine;

public class Fish : MonoBehaviour
{
    public enum MovementType { Wander, Traverse }

    [SerializeField] private Sprite customFishImage;
    [SerializeField] private MovementType movementType = MovementType.Wander;

    [Header("Movement")]
    [SerializeField] private float swimSpeed = 2f;
    [SerializeField] private float wanderChangeInterval = 25f;

    [Header("Mouse Avoidance")]
    [SerializeField] private float fleeRadius = 2f;
    [SerializeField] private float fleeSpeed = 3.5f;

    private Vector2 wanderTarget;
    private float wanderTimer;
    private Camera mainCamera;
    private Vector2 screenMin;
    private Vector2 screenMax;
    private float padding = 0.5f;

    // Traverse state
    private float traverseY;
    private bool isFleeing;

    void Awake()
    {
        if (customFishImage != null)
            GetComponent<SpriteRenderer>().sprite = customFishImage;
    }

    void Start()
    {
        mainCamera = Camera.main;
        CalculateBounds();

        if (movementType == MovementType.Traverse)
            InitTraverse();
        else
            PickNewWanderTarget();
    }

    void Update()
    {
        CalculateBounds();

        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float distToMouse = Vector2.Distance(transform.position, mouseWorld);

        if (movementType == MovementType.Traverse)
        {
            if (distToMouse < fleeRadius)
            {
                isFleeing = true;
                Flee(mouseWorld);
            }
            else
            {
                isFleeing = false;
                Traverse();
            }

            // Respawn once fully off the left edge
            if (transform.position.x < screenMin.x - padding - 1f)
                InitTraverse();
        }
        else
        {
            if (distToMouse < fleeRadius)
                Flee(mouseWorld);
            else
                Wander();

            ClampToBounds();
        }
    }

    // --- Wander ---

    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f || Vector2.Distance(transform.position, wanderTarget) < 0.2f)
            PickNewWanderTarget();

        MoveTowards(wanderTarget, swimSpeed);
    }

    void Flee(Vector2 mouseWorld)
    {
        Vector2 fleeDirection = ((Vector2)transform.position - mouseWorld).normalized;
        Vector2 fleeTarget;

        if (movementType == MovementType.Traverse)
        {
            // Only flee left (forward), lock Y to current traverse lane
            fleeTarget = new Vector2(transform.position.x - Mathf.Abs(fleeDirection.x) * 2f, traverseY);
        }
        else
        {
            fleeTarget = (Vector2)transform.position + fleeDirection * 2f;
            fleeTarget = ClampPoint(fleeTarget);
        }

        MoveTowards(fleeTarget, fleeSpeed);
    }

    void PickNewWanderTarget()
    {
        wanderTarget = new Vector2(
            Random.Range(screenMin.x, screenMax.x),
            Random.Range(screenMin.y, screenMax.y)
        );
        wanderTimer = wanderChangeInterval;
    }

    // --- Traverse ---

    void InitTraverse()
    {
        CalculateBounds();

        float spawnX = screenMax.x + padding + 0.5f;
        traverseY = Random.Range(screenMin.y, screenMax.y);
        transform.position = new Vector3(spawnX, traverseY, transform.position.z);

        // Face left
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void Traverse()
    {
        // Smoothly return to the traverse Y lane after fleeing
        float targetY = Mathf.MoveTowards(transform.position.y, traverseY, swimSpeed * Time.deltaTime);
        Vector2 target = new Vector2(transform.position.x - swimSpeed * Time.deltaTime, targetY);
        transform.position = new Vector3(target.x, target.y, transform.position.z);

        // Face left
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // --- Shared ---

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 newPos = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        float moveDir = target.x - transform.position.x;
        if (Mathf.Abs(moveDir) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = moveDir > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        transform.position = newPos;
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