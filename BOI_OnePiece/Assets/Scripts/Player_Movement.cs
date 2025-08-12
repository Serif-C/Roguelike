using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] Animator anim;

    private Vector2 movementInput;
    private InputSystem_Actions inputSystem;

    private void Awake()
    {
        inputSystem = new InputSystem_Actions(); // instantiate the input system

        // Subscribe to movement action
        inputSystem.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputSystem.Player.Move.canceled += ctx => movementInput = Vector2.zero;
    }

    private void OnEnable()
    {
        inputSystem.Enable();
    }

    private void OnDisable()
    {
        inputSystem.Disable();
    }

    private void Update()
    {
        anim.SetFloat("xInput", movementInput.x);
        anim.SetFloat("yInput", movementInput.y);

        Vector2 movement = movementInput * movementSpeed * Time.deltaTime ;

        if(movement != Vector2.zero)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        transform.position += (Vector3) movement;
    }
}
