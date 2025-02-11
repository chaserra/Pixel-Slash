using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    public delegate void OnShiftHeld();
    public event OnShiftHeld e_ShiftHeld;
    public delegate void OnShiftRelease();
    public event OnShiftRelease e_ShiftReleased;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 5f;

    private Player player;
    private Rigidbody2D rb;
    private Vector2 _movement;
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
        timeShiftAction.canceled += OnTimeShiftCancel;
        player.e_TimeSlash += OnTimeSlash;
    }

    private void Start()
    {
       
    }

    private void Update()
    {
        // Do not get input if game is paused
        if (GameManager.Instance.IsGamePaused) { return; }

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
        timeShiftAction.canceled -= OnTimeShiftCancel;
        player.e_TimeSlash -= OnTimeSlash;
    }

    [Tooltip("Gets player input.")]
    private void GetInput()
    {
        // Read rotation input
        rotationVector = rotateAction.ReadValue<Vector2>();

        // Check if timeshift button is held down
        if (timeShiftAction.ReadValue<float>() > 0f && player.CanUseTimeShift)
        {
            // Stop movement
            _movement = Vector2.zero;

            // Trigger time dilate event
            e_ShiftHeld?.Invoke();
        }
        else
        {
            // Get movement values from Input System
            _movement = moveAction.ReadValue<Vector2>();;

            // Trigger end of time dilate event
            e_ShiftReleased?.Invoke();
        }
    }

    [Tooltip("Move player using Rigidbody 2D.")]
    private void Move()
    {
        // Move via Rigidbody2D
        rb.MovePosition(rb.position + _movement * (MoveSpeed * Time.deltaTime) * GameManager.Instance.InGameTimeScale);
    }

    [Tooltip("Call the correct attack type depending on player state.")]
    private void OnAttack(InputAction.CallbackContext context)
    {
        // Ignore all when game is paused
        if (GameManager.Instance.IsGamePaused) { return; }

        if (!player.TimeShiftActive)
        {
            player.Attack();
        }
        else
        {
            player.DashAttack();
        }
    }

    [Tooltip("Set recently time shifted flag on button release.")]
    private void OnTimeShiftCancel(InputAction.CallbackContext context)
    {
        // Ignore all when game is paused
        if (GameManager.Instance.IsGamePaused) { return; }

        player.RecentlyTimeShifted = true;
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

    public void OnTimeSlash()
    {
        // Ignore all when game is paused
        if (GameManager.Instance.IsGamePaused) { return; }

        Vector3 direction = player.transform.up;
        Vector2 xyDir = direction;
        Vector3 teleportSpot = rb.position + player.TimeSliceDistance * xyDir;
        
        // Check for wall collisions
        int playerLayer = LayerMask.NameToLayer("Player"); // Get Player layer index
        LayerMask filteredLayer = ~(1 << playerLayer); // Remove player layer from layer mask
        RaycastHit2D[] raycastToNewPos = Physics2D.LinecastAll(transform.position, teleportSpot, filteredLayer);
        for (int i = 0; i < raycastToNewPos.Length; i++)
        {
            // If a wall is in the path, teleport player to collision point
            if (raycastToNewPos[i].collider.gameObject.tag == "Wall")
            {
                transform.position = raycastToNewPos[i].collider.ClosestPoint(raycastToNewPos[i].point);
                return;
            }
        }
        // If no wall is found, do a complete dash
        transform.position = teleportSpot;

    }

    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }

    public float RotateSpeed
    {
        get { return _rotateSpeed; }
    }

    public Vector2 MovementVector
    {
        get { return _movement; }
    }
}
