using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public class Combat : MonoBehaviour
{
    public enum ArmSide { Left = 0, Right = 1 }

    [Header("References")]
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform leftFirePoint;
    [SerializeField] private Transform rightFirePoint;
    //[SerializeField] Animator anim;
    [SerializeField] private Sprite[] punchSprites; // 0 = rightPunch, 1 = leftPunch

    [Header("Stats")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attacksPerSecond = 5f;


    //[Header("Cooldown")]
    private float cooldownTimer = 0f;
    private Vector2 attackInput;
    private InputSystem_Actions playerInput;

    // Active projectile + arm start for line renderer
    private Transform currentFist;
    public Transform CurrentFist => currentFist;
    private Transform currentArmStart;
    public Transform CurrentArmStart => currentArmStart;

    // Which arm to use on the *next* attack:
    private ArmSide nextArm = ArmSide.Right;
    private SpriteRenderer sr;
    private Sprite defaultSprite;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
        playerInput.Player.Attack.performed += ctx => attackInput = ctx.ReadValue<Vector2>();
        playerInput.Player.Attack.canceled += ctx => attackInput = Vector2.zero;

        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) defaultSprite = sr.sprite;
    }

    void OnEnable() => playerInput.Enable();
    void OnDisable() => playerInput.Disable();

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        bool wantAttack = attackInput != Vector2.zero;

        // Only fire if: player is holding a direction, cooldown ready, and no active fist
        if (wantAttack && cooldownTimer <= 0f && currentFist == null)
        {
            Vector2 dir = attackInput.normalized;
            Attack(dir);
            cooldownTimer = 1f / Mathf.Max(0.0001f, attacksPerSecond);
        }

        // Only revert to default when we're truly idle (no fist AND no attack input)
        if (currentFist == null && !wantAttack && sr != null)
        {
            sr.sprite = defaultSprite;
        }
    }

    void Attack(Vector2 direction)
    {
        /////////////////////////////////////////////////////////////////////////
        ///choose which arm fires *this* time (flip after the fist finishes)
        Transform armStart;

        if (nextArm == ArmSide.Right)
        {
            armStart = rightFirePoint;
            sr.sprite = punchSprites[0];
        }
        else
        {
            armStart = leftFirePoint;
            sr.sprite = punchSprites[1];
        }

        if (armStart == null)
        {
            if (rightFirePoint != null)
            {
                armStart = rightFirePoint;
                sr.sprite = punchSprites[0];
            }
            else
            {
                armStart = leftFirePoint;
                sr.sprite = punchSprites[1];
            }
        }
        /////////////////////////////////////////////////////////////////////////

        //anim.SetInteger("PunchSide", (int)nextArm);
        //anim.SetTrigger("Punch");
        //anim.SetBool("HoldPunch", true);

        // spawn fist
        GameObject fistGO = Instantiate(attackPrefab, armStart.position, Quaternion.identity);
        currentFist = fistGO.transform;
        currentArmStart = armStart;

        // init projectile so it can fly & report back on destroy
        var proj = fistGO.GetComponent<FistProjectile>();
        if (proj) proj.Init(this, armStart, attackRange, direction);
    }

    // called by the projectile when it despawns/destroys
    public void ClearFist(Transform fist)
    {
        if (currentFist == fist)
        {
            currentFist = null;
            currentArmStart = null;

            // release hold pose
            //anim.SetBool("HoldPunch", false);

            // restore default sprite when attack truly ends
            if (sr) sr.sprite = defaultSprite;

            // flip to the other arm for the *next* attack
            nextArm = (nextArm == ArmSide.Right) ? ArmSide.Left : ArmSide.Right;
        }
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
