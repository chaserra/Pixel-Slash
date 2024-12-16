using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 5f;

    private Player player;
    private PlayerControls playerControls;
    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Movement.Attack.performed += OnAttack;
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
        playerControls.Disable();
        playerControls.Movement.Attack.performed -= OnAttack;
    }

    [Tooltip("Gets player input.")]
    private void GetInput()
    {
        // Get movement values from Input System
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
    }

    [Tooltip("Move player using Rigidbody 2D.")]
    private void Move()
    {
        // Move via Rigidbody2D
        rb.MovePosition(rb.position + movement * (MoveSpeed * Time.deltaTime) * GameManager.Instance.InGameTimeScale);
    }

    [Tooltip("Input System Attack Callback.")]
    private void OnAttack(InputAction.CallbackContext context)
    {
        player.Attack();
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

    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }

    public float RotateSpeed
    {
        get { return _rotateSpeed; }
    }
}
