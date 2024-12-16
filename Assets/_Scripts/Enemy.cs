using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health = 1;
    [SerializeField] private float _shootBaseCooldown = 5f;
    [SerializeField] private float _rotateSpeed = 5f;

    private Player targetPlayer;
    private ObjectPooler_Bullet bulletPool;
    private SpriteRenderer spriteRenderer;
    private float shootCooldown;
    private float shootTimer = 0f;

    private void Awake()
    {
        // Get bullet object pooling instance
        bulletPool = ObjectPooler_Bullet.Instance;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        targetPlayer = FindFirstObjectByType<Player>();
        GenerateShootCooldown();
    }

    private void Update()
    {
        LookAtTarget();
        Shoot();
    }

    [Tooltip("Look at player.")]
    private void LookAtTarget()
    {
        // Get direction to target player
        Vector2 dir = (targetPlayer.transform.position - transform.position).normalized;
        // Get angle to direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float offset = 90f;
        // Get rotation
        Quaternion rotation = Quaternion.AngleAxis(angle - offset, Vector3.forward);
        // Rotate the player
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotateSpeed * Time.deltaTime * GameManager.Instance.InGameTimeScale);
    }

    [Tooltip("Shoot bullets every set time with random offset.")]
    private void Shoot()
    {
        if (shootTimer >= shootCooldown)
        {
            // Flash sprite then shoot
            StartCoroutine(ShootCoroutine());
            // Reset cooldown and generate new cooldown value
            shootTimer = 0f;
            GenerateShootCooldown();
        }
        // Increment timer
        shootTimer += Time.deltaTime * GameManager.Instance.InGameTimeScale;
    }

    private IEnumerator ShootCoroutine()
    {
        // Wait for flashing to finish before actually shooting
        yield return SpriteEffects.FlashSprite(spriteRenderer, 0.5f);
        // Find a pooled bullet object
        GameObject bullet = bulletPool.GetPooledObject();
        // Spawn in front of this unit and copy rotation
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        // Set tag and layer
        bullet.gameObject.tag = gameObject.tag;
        bullet.gameObject.layer = LayerMask.NameToLayer("Trigger");
        // Activate the bullet
        bullet.SetActive(true);
        yield return null;
    }

    [Tooltip("Generates a new shoot cooldown with a random offset for variety.")]
    private void GenerateShootCooldown()
    {
        shootCooldown = Random.Range(-2f, 5f) + ShootBaseCooldown;
    }

    [Tooltip("Take damage then check if health is zero.")]
    public void TakeDamage(GameObject source)
    {
        // Ignore self
        if (source.tag == "Enemy") { return; }

        Debug.Log("Took damage from " + source.name + " from " + source.gameObject.tag);
        Health--;

        if (Health <= 0)
        {
            // TODO: Destroy/Pool enemy
            Debug.Log(gameObject.name + " has died.");
        }
    }

    public int Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    public float ShootBaseCooldown
    {
        get { return _shootBaseCooldown; }
        private set { _shootBaseCooldown = value; }
    }

    public float RotateSpeed
    {
        get { return _rotateSpeed; }
        private set { _rotateSpeed = value; }
    }
}
