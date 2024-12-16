using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 5f;

    private Player player;
    private Rigidbody2D rb;
    private Vector2 movement;

    // Input System
    private PlayerInput playerInput;
    // Store Control Actions
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction timeShiftAction;
    private InputAction rotateAction;
    private InputAction dashAttackAction;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("Movement");

    }

    private void Start()
    {
        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
        timeShiftAction = playerInput.actions["Timeshift"];
        rotateAction = playerInput.actions["Rotate"];
        dashAttackAction = playerInput.actions["Dash Attack"];
    }

    private void Update()
    {
        GetInput();
        Rotate();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnDisable()
    {
        //playerControls.Disable();
        //playerControls.Movement.Attack.performed -= OnAttack;
        //playerControls.TimeDilation.DashAttack.performed -= OnDashAttack;
    }

    [Tooltip("Gets player input.")]
    private void GetInput()
    {
        // TODO: Change this so action maps are switched dynamically. Attack should be the same as DashAttack
        // Check if timeshift button is held down
        if (timeShiftAction.ReadValue<float>() > 0f)
        {
            playerInput.actions.FindActionMap("Time Dilation").Enable();
            // Stop movement
            movement = Vector2.zero;
            // TODO: Just rotate the player
        }
        else
        {
            playerInput.actions.FindActionMap("Movement").Enable();
            // Get movement values from Input System
            movement = moveAction.ReadValue<Vector2>();
        }

        if (attackAction.IsPressed() || dashAttackAction.IsPressed())
        {
            OnAttack();
        }
    }

    [Tooltip("Move player using Rigidbody 2D.")]
    private void Move()
    {
        // Move via Rigidbody2D
        rb.MovePosition(rb.position + movement * (MoveSpeed * Time.deltaTime) * GameManager.Instance.InGameTimeScale);
    }

    [Tooltip("Input System Attack.")]
    private void OnAttack()
    {
        player.Attack();
    }

    [Tooltip("Input System Dash Attack.")]
    private void OnDashAttack(InputAction.CallbackContext context)
    {
        // TODO: Perform a dash attack
        Debug.Log("Dash attack!");
    }

    [Tooltip("Rotate player towards movement direction.")]
    private void Rotate()
    {
        // Only rotate if movement vector has a value.
        if (movement.magnitude > 0f)
        {
            // Get angle to movement vector
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            // Offset angle to make X horizontal and Y vertical
            float offset = 90f;
            // Get rotation
            Quaternion rotation = Quaternion.AngleAxis(angle - offset, Vector3.forward);
            // Rotate the player
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotateSpeed * Time.deltaTime * GameManager.Instance.InGameTimeScale);
        }
    }

    [Tooltip("Rotate player towards movement direction while in Time Dilation.")]
    private void SpotRotate()
    {
        // TODO: Rotate in place. Or maybe put this inside the Rotate method above instead
    }

    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }

    public float RotateSpeed
    {
        get { return _rotateSpeed; }
    }
}
