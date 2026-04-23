using UnityEngine;

public class Fish : MonoBehaviour
{
    public enum MovementType { Wander, Traverse, Crab, Octopus, Empty}

    [Header("Sprites")]
    [SerializeField] private Sprite wanderFishImage;
    [SerializeField] private Sprite traverseFishImage;
    [SerializeField] private Sprite crabImage;
    [SerializeField] private Sprite octopusImage;

    [SerializeField] private MovementType movementType = MovementType.Wander;

    private float swimSpeed = 2f;
    private float wanderChangeInterval = 25f;

    private float fleeRadius = 2f;
    private float fleeSpeed = 3.5f;

    private Vector2 wanderTarget;
    private float wanderTimer;
    private Camera mainCamera;
    private Vector2 screenMin;
    private Vector2 screenMax;
    private float padding = 0.5f;

    // Traverse state
    private float traverseY;
    private bool isFleeing;
    private int traverseDirection;

    // Crab
    private float crabWalkTimer;
    private int crabWalkDirection = 1;

    void Start()
    {
        mainCamera = Camera.main;
        CalculateBounds();

        AssignSprite();
        crabWalkTimer = Random.Range(1f, 6f);

        if (movementType == MovementType.Traverse)
            ResetTraverse();
        else
            PickNewWanderTarget();
    }

    void AssignSprite()
    {
        Sprite chosen = movementType switch
        {
            MovementType.Wander   => wanderFishImage,
            MovementType.Traverse => traverseFishImage,
            MovementType.Crab     => crabImage,
            MovementType.Octopus  => octopusImage,
            _                     => null
        };

        if (chosen != null)
            GetComponent<SpriteRenderer>().sprite = chosen;
    }

    void Update()
    {
        CalculateBounds();

        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float distToMouse = Vector2.Distance(transform.position, mouseWorld);

        if (distToMouse < fleeRadius)
        {
            isFleeing = true;
            Flee(mouseWorld);
        }
        else {
            isFleeing = false;
            if (movementType == MovementType.Traverse){
                Traverse();
                bool exitedRight = traverseDirection == 1 && transform.position.x > screenMax.x + padding + 1f;
                bool exitedLeft = traverseDirection == -1 && transform.position.x < screenMin.x - padding - 1f;
                if (exitedRight || exitedLeft){
                    ResetTraverse();
                }
            }
            else if (movementType == MovementType.Crab){
                CrabWalk();
            }    
            else{
                Wander();
                ClampToBounds();
            }
        }
    }

    public void SetMovementType(MovementType type)
    {
        movementType = type;
    }

    public void SetStats(float speed, float fleeRad, float fleeSpd)
    {
        swimSpeed = speed;
        fleeRadius = fleeRad;
        fleeSpeed = fleeSpd;
    }

    void CrabWalk(){
        crabWalkTimer -= Time.deltaTime;

        bool hitWall = false;

        if (transform.position.x <= screenMin.x){
            crabWalkDirection = 1;
            transform.position = new Vector3(screenMin.x + 0.05f, transform.position.y, transform.position.z);
            hitWall = true;
        }
        else if (transform.position.x >= screenMax.x){
            crabWalkDirection = -1;
            transform.position = new Vector3(screenMax.x - 0.05f, transform.position.y, transform.position.z);
            hitWall = true;
        }

        // Reset timer on wall hit so random flip can't immediately re-reverse
        if (hitWall){
            crabWalkTimer = Random.Range(1f, 6f);
        }

        Vector2 target = new Vector2(transform.position.x + swimSpeed * Time.deltaTime * crabWalkDirection, transform.position.y);
        transform.position = new Vector3(target.x, target.y, transform.position.z);

        if (crabWalkTimer <= 0){
            crabWalkDirection = crabWalkDirection * -1;
            crabWalkTimer = Random.Range(1f, 6f);
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
        Vector2 fleeDirection = ((Vector2)transform.position - mouseWorld).normalized;
        Vector2 fleeTarget;

        if (movementType == MovementType.Traverse)
        {
            fleeTarget = new Vector2(transform.position.x + traverseDirection * Mathf.Abs(fleeDirection.x) * 2f, traverseY);
        }
        else if (movementType == MovementType.Crab){
            fleeTarget = new Vector2(transform.position.x + (fleeDirection.x) * 2f, transform.position.y);
            fleeTarget = ClampPoint(fleeTarget);
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

    void ResetTraverse()
    {
        CalculateBounds();

        traverseDirection = (Random.value > 0.5f) ? 1 : -1;
        traverseY = Random.Range(screenMin.y, screenMax.y);

        // Spawn on opposite side from direction of travel
        float spawnX = traverseDirection == 1 ? screenMin.x - padding - 0.5f : screenMax.x + padding + 0.5f;
        transform.position = new Vector3(spawnX, traverseY, transform.position.z);

        // Face direction of travel
        // Vector3 scale = transform.localScale;
        // scale.x = traverseDirection == 1 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        // transform.localScale = scale;
    }


    void Traverse()
    {
        Vector2 target = new Vector2(transform.position.x + swimSpeed * Time.deltaTime * traverseDirection, transform.position.y);
        transform.position = new Vector3(target.x, target.y, transform.position.z);

        // Face direction of travel
        // Vector3 scale = transform.localScale;
        // scale.x = traverseDirection == 1 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        // transform.localScale = scale;
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 newPos = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // float moveDir = target.x - transform.position.x;
        // if (Mathf.Abs(moveDir) > 0.01f)
        // {
        //     Vector3 scale = transform.localScale;
        //     scale.x = moveDir > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        //     transform.localScale = scale;
        // }

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