using UnityEngine;

public class FistProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 18f;

    private Rigidbody2D rb2d;
    private Combat owner;
    private Transform origin;   //firePoint at hand
    private float maxDistance;
    private Vector2 direction;
    private Vector2 startPos;

    public void Init(Combat owner, Transform origin, float maxDistance, Vector2 direction)
    {
        this.owner = owner;
        this.origin = origin;
        this.maxDistance = maxDistance;
        this.direction = direction.sqrMagnitude > 0 ? direction.normalized : Vector2.right;
    }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0f;
    }

    private void OnEnable()
    {
        // Cache starting position when it actually spawns
        startPos = origin ? (Vector2)origin.position : (Vector2)transform.position;

        // Set constant velocity once
        rb2d.linearVelocity = direction * speed;
    }

    private void FixedUpdate()
    {
        // Auto-despawn at max range
        float dist = Vector2.Distance(startPos, rb2d.position);
        if (dist >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hit something → despawn (or start retract logic)
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Let owner clear the active fist reference so the line hides
        //if (owner) owner.ClearFist(transform);
    }
}
