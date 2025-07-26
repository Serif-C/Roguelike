using UnityEngine;
using UnityEngine.InputSystem;
public class Combat : MonoBehaviour
{
    /*
     * !!!Think about controller support!!!
     * 
     * Basic Attack:
     * - Gumpistol: Shoot out your hands alternately (base on aspd) to the direction you are looking
     *
     * Heavy Attack:
     * - Gumbazooka: Shoot out both your hands in the direction you are looking (2s cooldown)
     * 
     * Attacks should be physics based and should ricochet upon collision.
     * Since attacks stretches, it should feel snappy, and have a lot of weight on impact.
     */

    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float handSpeed;

    private Vector2 attackInput;
    private InputSystem_Actions playerInput;

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
        if (attackInput != Vector2.zero)
        {
            Attack(attackInput.normalized);
        }
    }

    void Attack(Vector2 direction)
    {
        GameObject hand = Instantiate(attackPrefab, firePoint ? firePoint.position : transform.position, Quaternion.identity);
        Rigidbody2D handRb = hand.GetComponent<Rigidbody2D>();
        if (handRb != null)
        {
            handRb.linearVelocity = direction * handSpeed;
        }
    }
}
