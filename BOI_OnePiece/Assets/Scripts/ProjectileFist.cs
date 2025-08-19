using UnityEngine;

public class ProjectileFist : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float thrust = 1f;
    private Vector2 projectileDirection;
    private float maxDistance;
    private Vector2 projStartPosition;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        projectileDirection = GameObject.FindGameObjectWithTag("Player").GetComponent<Combat>().GetAttackDirection();
        maxDistance = GameObject.FindGameObjectWithTag("Player").GetComponent<Combat>().GetAttackRange();
        projStartPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    private void FixedUpdate()
    {
        rb2d.AddForce(projectileDirection * thrust, ForceMode2D.Impulse);

        // Destroys the arm projectile when it travels a certain range
        if(Vector2.Distance(projStartPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
