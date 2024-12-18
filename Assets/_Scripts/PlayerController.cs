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
    private Vector2 rotationVector;

    // Input System
    private PlayerInput playerInput;
    // Store Control Actions
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction timeShiftAction;
    private InputAction rotateAction;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
        timeShiftAction = playerInput.actions["Timeshift"];
        rotateAction = playerInput.actions["Rotate"];
    }

    private void OnEnable()
    {
        playerInput.actions.FindActionMap("Movement").Enable();
        playerInput.actions.FindActionMap("Time Dilation").Enable();
        attackAction.performed += OnAttack;
    }

    private void Start()
    {
       
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
        playerInput.actions.FindActionMap("Movement").Disable();
        playerInput.actions.FindActionMap("Time Dilation").Disable();
        attackAction.performed -= OnAttack;
    }

    [Tooltip("Gets player input.")]
    private void GetInput()
    {
        // Read rotation input
        rotationVector = rotateAction.ReadValue<Vector2>();

        // Check if timeshift button is held down
        if (timeShiftAction.ReadValue<float>() > 0f)
        {
            // Stop movement
            movement = Vector2.zero;

            // Slow down time via time dilation
            // TODO: Change below logic to be handled by TimeDilation. Controller should just notify if Shift is held down
            GameManager.Instance.SlowDownTime();
            player.TimeShiftActive = true;
        }
        else
        {
            // Get movement values from Input System
            movement = moveAction.ReadValue<Vector2>();

            // Cancel time dilation
            // TODO: Change below logic to be handled by TimeDilation. Controller should just notify if Shift is no longer held down
            GameManager.Instance.ResumeGame();
            GameManager.Instance.IsTimeSlowed = false;
            player.TimeShiftActive = false;
        }
    }

    [Tooltip("Move player using Rigidbody 2D.")]
    private void Move()
    {
        // Move via Rigidbody2D
        rb.MovePosition(rb.position + movement * (MoveSpeed * Time.deltaTime) * GameManager.Instance.InGameTimeScale);
    }

    [Tooltip("Call the correct attack type depending on player state.")]
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!player.TimeShiftActive)
        {
            player.Attack();
        }
        else
        {
            player.DashAttack();
        }
    }

    [Tooltip("Rotate player towards movement direction.")]
    private void Rotate()
    {
        // Only rotate if movement vector has a value.
        if (rotationVector.magnitude > 0f)
        {
            // Get angle to movement vector
            float angle = Mathf.Atan2(rotationVector.y, rotationVector.x) * Mathf.Rad2Deg;
            // Offset angle to make X horizontal and Y vertical
            float offset = 90f;
            // Get rotation
            Quaternion rotation = Quaternion.AngleAxis(angle - offset, Vector3.forward);
            // Rotate the player
            // If time is slowed down, rotate speed is slightly faster than special game time
            if (GameManager.Instance.IsTimeSlowed)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotateSpeed * Time.deltaTime * (GameManager.Instance.InGameTimeScale * (.25f / GameManager.Instance.SlowTimeScale)));
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotateSpeed * Time.deltaTime * GameManager.Instance.InGameTimeScale);
            }
        }
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
