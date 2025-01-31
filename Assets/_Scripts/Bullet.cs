using UnityEngine;

public class Bullet : MonoBehaviour, IAttackable
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _bulletLifespan = 10f;
    [SerializeField] private GameObject spriteObject;

    private Rigidbody2D rb;
    private Collider2D bulletCollider;
    private SpriteRenderer sprite;
    private ParticleSystem particles;

    private float initMoveSpeed;
    private SourceType _sourceType = SourceType.Enemy;
    private Vector2 movement;
    private float lifespan = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        particles = GetComponentInChildren<ParticleSystem>();
        initMoveSpeed = MoveSpeed;
    }

    private void OnEnable()
    {
        // Wake up rigidbody
        rb.WakeUp();
        // Reenable sprite
        sprite.enabled = true;
        // Reset lifespan
        lifespan = 0f;
        // Reenable collisions
        bulletCollider.enabled = true;
        // Reset movespeed to original value
        MoveSpeed = initMoveSpeed;
        // Clear particles
        particles.Clear();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        GetMoveDirection();
        Expire();
        CheckForParticles();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnDisable()
    {

    }

    [Tooltip("Gets the bullet's local up direction for movement.")]
    private void GetMoveDirection()
    {
        movement = transform.up;
    }

    [Tooltip("Disable object after x amount of time.")]
    private void Expire()
    {
        if (lifespan >= BulletLifespan)
        {
            Deactivate();
        }
        lifespan += Time.deltaTime * GameManager.Instance.InGameTimeScale;
    }

    [Tooltip("Completely disables the object when particle effects end.")]
    private void CheckForParticles()
    {
        if (!particles.IsAlive())
        {
            gameObject.SetActive(false);
        }
    }

    [Tooltip("Move the bullet towards it's up direction.")]
    private void Move()
    {
        rb.MovePosition(rb.position + movement * (MoveSpeed * Time.deltaTime) * GameManager.Instance.InGameTimeScale);
    }

    public void IsAttacked(GameObject source)
    {
        // Check if bullet is perpendicular to the slash
        Vector3 slashDirection = source.transform.up;
        Vector3 bulletDirection = transform.up;

        // If source is directly from a player (Time Slash)
        if (source.TryGetComponent<Player>(out var P))
        {
            // Flip to opposite direction (back to source of bullet)
            transform.Rotate(new Vector3(0f, 0f, 180f));
        }
        // If perpendicular or same direction as slash object
        // Deflect to direction of slash object
        else if (Vector3.Dot(slashDirection, bulletDirection) > -0.75f)
        {
            // Get angle between slash and bullet directions
            float angle = Vector2.Angle(slashDirection, bulletDirection);
            // Offset angle to make X horizontal and Y vertical
            float offset = 90f;
            // Get rotation
            Quaternion rotation = Quaternion.AngleAxis(angle - offset, Vector3.forward);
            // Rotate the bullet
            transform.rotation = rotation;
        }
        // If angle is not steep
        else
        {
            // Flip to opposite direction (back to source of bullet)
            transform.Rotate(new Vector3(0f, 0f, 180f));
        }
        // Change source type depending on who attacked it
        gameObject.tag = source.tag;
        // Make bullet move faster and reset lifespan
        MoveSpeed *= 5f;
        lifespan = 0f;
        // Invoke events
        GameManager.Instance.InvokeOnHitEvents();
    }

    [Tooltip("Soft disable. Allows particle effects to finish before full deactivation.")]
    public void Deactivate()
    {
        // Stop all movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.Sleep();

        // Deactivate sprite, trail, and collisions
        sprite.enabled = false;
        particles.Stop();
        bulletCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore objects without rigidbodies
        if (collision.attachedRigidbody == null) { return; }
        // Ignore objects with the same tag
        if (collision.tag == gameObject.tag) { return; }

        Transform collisionParent = collision.attachedRigidbody.transform;

        // Call interface method
        if (collisionParent.TryGetComponent<IDamageable>(out IDamageable damageCol))
        {
            damageCol.TakeDamage(gameObject);
            Deactivate();
        }
        // Wall hit
        if (collisionParent.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Deactivate();
        }
    }

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        private set { _moveSpeed = value; }
    }

    public float BulletLifespan
    {
        get { return _bulletLifespan; }
    }

    public SourceType SourceType
    {
        get { return _sourceType; }
    }
}
