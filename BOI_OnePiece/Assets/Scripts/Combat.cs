using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public class Combat : MonoBehaviour
{
    [Header("References & Stats")]
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float handSpeed;
    [SerializeField] Animator anim;
    [SerializeField] private float attackRange = 5f;

    [Header("Cooldown")]
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackStartTime = 0f;


    private Vector2 attackInput;
    private InputSystem_Actions playerInput;

    // NEW: reference to active fist
    private Transform currentFist;
    public Transform CurrentFist => currentFist;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();

        playerInput.Player.Attack.performed += ctx => attackInput = ctx.ReadValue<Vector2>();
        playerInput.Player.Attack.canceled += ctx => attackInput = Vector2.zero;
    }

    void OnEnable() => playerInput.Enable();
    void OnDisable() => playerInput.Disable();

    void Update()
    {
        // attackSpeed = 1 is 1 Second, 2 = 0.5 Second, etc....
        float attackCooldown = 1f / Mathf.Max(attackSpeed, 0.0001f);
        anim.speed = attackSpeed;

        if(attackStartTime > 0f)
        {
            attackStartTime -= Time.deltaTime;
        }

        if(attackStartTime <= 0f && attackInput != Vector2.zero)
        {
            attackStartTime += Time.deltaTime;

            anim.SetFloat("xAtk_Input", attackInput.x);
            anim.SetFloat("yAtk_Input", attackInput.y);
            anim.SetBool("isAttacking", true);

            Attack(attackInput.normalized);

            attackStartTime = attackCooldown;
        }
        else
        {
            anim.SetBool("isAttacking", false);
        }
    }

    void Attack(Vector2 direction)
    {
        var spawnPos = firePoint ? firePoint.position : transform.position;
        GameObject fistGO = Instantiate(attackPrefab, spawnPos, Quaternion.identity);

        // keep reference
        currentFist = fistGO.transform;

        var proj = fistGO.GetComponent<FistProjectile>();
        if (proj) proj.Init(this, firePoint ? firePoint : transform, attackRange, direction);
    }

    // called by the projectile when it despawns/destroys
    public void ClearFist(Transform fist)
    {
        if (currentFist == fist) currentFist = null;
    }

    public Vector2 GetAttackDirection()
    {
        return attackInput.normalized;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }
}
