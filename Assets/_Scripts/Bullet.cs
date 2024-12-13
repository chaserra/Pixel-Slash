using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour, IAttackable
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _bulletLifespan = 10f;

    private Rigidbody2D rb;
    private Collider2D bulletCollider;
    private SpriteRenderer sprite;
    private TrailRenderer trailRenderer;

    private float initMoveSpeed;
    private SourceType _sourceType = SourceType.Enemy;
    private Vector2 movement;
    private float lifespan = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        initMoveSpeed = MoveSpeed;
    }

    private void OnEnable()
    {
        // Wake up rigidbody
        rb.WakeUp();
        // Reenable sprite
        sprite.enabled = true;
        // Reenable trail
        trailRenderer.emitting = true;
        // Reset lifespan
        lifespan = 0f;
        // Reenable collisions
        bulletCollider.enabled = true;
        // Reset movespeed to original value
        MoveSpeed = initMoveSpeed;
        // Clear trail
        trailRenderer.Clear();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        GetMoveDirection();
        Expire();
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
        movement = transform.TransformDirection(Vector3.up);
    }

    [Tooltip("Disable object after x amount of time.")]
    private void Expire()
    {
        if (lifespan >= BulletLifespan)
        {
            Deactivate();
        }
        lifespan += Time.deltaTime;
    }

    [Tooltip("Move the bullet towards it's up direction.")]
    private void Move()
    {
        rb.MovePosition(rb.position + movement * (MoveSpeed * Time.deltaTime));
    }

    public void IsAttacked(GameObject source)
    {
        // Check if bullet is perpendicular to the slash
        Vector3 slashDirection = (source.transform.TransformDirection(Vector3.up)).normalized;
        Vector3 bulletDirection = (transform.TransformDirection(Vector3.up)).normalized;

        //Debug.Log(Vector3.Dot(slashDirection, bulletDirection));

        // If perpendicular or same direction as slash object
        // Deflect to direction of slash object
        if (Vector3.Dot(slashDirection, bulletDirection) > -0.75f)
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
        // TODO: Trigger hitstop. Maybe through an event?
        // Change source type depending on who attacked it
        gameObject.tag = source.tag;
        // Make bullet move faster and reset lifespan
        MoveSpeed *= 3f;
        lifespan = 0f;
    }

    [Tooltip("Soft disable. Allows trail to finish before full deactivation.")]
    public void Deactivate()
    {
        // Stop all movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.Sleep();

        // Deactivate sprite, trail, and collisions
        sprite.enabled = false;
        trailRenderer.emitting = false;
        bulletCollider.enabled = false;

        // Wait for trail renderer to finish before fully deactivating the object
        StartCoroutine(FullyDeactivateObject());
    }

    [Tooltip("Wait for trail renderer to end before fully disabling the object.")]
    private IEnumerator FullyDeactivateObject()
    {
        // Wait for trail emission to end
        yield return new WaitForSeconds(trailRenderer.time);
        // Then deactivate the object
        gameObject.SetActive(false);
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
