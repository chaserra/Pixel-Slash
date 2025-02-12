using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health = 1;
    [SerializeField] private float _range = 10f;
    [SerializeField] private float _shootBaseCooldown = 5f;
    [SerializeField] private float _rotateSpeed = 5f;
    [SerializeField] private GameObject spriteObject;

    private Player targetPlayer;
    private ObjectPooler_Bullet bulletPool;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float shootCooldown;
    private float shootTimer = 0f;
    private bool isAttacking = false;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    private void Awake()
    {
        // Get bullet object pooling instance
        bulletPool = ObjectPooler_Bullet.Instance;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        targetPlayer = FindFirstObjectByType<Player>();
        GenerateShootCooldown();
        // Shoot on first sight <3
        shootTimer = shootCooldown;
    }

    private void Update()
    {
        if (!AcquireTarget()) { return; }
        LookAtTarget();
        Shoot();
        spriteObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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

    private bool AcquireTarget()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        LayerMask filteredLayer = (1 << playerLayer);

        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, _range, filteredLayer);

        for (int i = 0; i < col.Length; i++)
        {
            if (col[i].gameObject.tag == "Player")
            {
                filteredLayer |= (1 << LayerMask.NameToLayer("Wall"));
                RaycastHit2D hit = Physics2D.Linecast(transform.position, col[i].transform.position, filteredLayer);

                if (hit)
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    [Tooltip("Shoot bullets every set time with random offset.")]
    private void Shoot()
    {
        if (shootTimer >= shootCooldown && !isAttacking)
        {
            // Flash sprite then shoot
            //StartCoroutine(ShootCoroutine());
            // Reset cooldown and generate new cooldown value
            //shootTimer = 0f;
            //GenerateShootCooldown();
            isAttacking = true;
            animator.SetTrigger("StartAttack");
        }
        // Increment timer
        shootTimer += Time.deltaTime * GameManager.Instance.InGameTimeScale;
    }

    [Tooltip("Triggered by animation event.")]
    public void OnAttack()
    {

    }

    [Tooltip("Triggered by animation event. Shoots a bullet object.")]
    public void OnShoot()
    {
        GameObject bullet = bulletPool.GetPooledObject();
        // Spawn in front of this unit and copy rotation
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        // Set tag and layer
        bullet.gameObject.tag = gameObject.tag;
        bullet.gameObject.layer = LayerMask.NameToLayer("Trigger");
        // Activate the bullet
        bullet.SetActive(true);

        // Reset cooldown and generate new cooldown value
        shootTimer = 0f;
        GenerateShootCooldown();
        isAttacking = false;
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
        shootCooldown = Random.Range(-2f, 2f) + ShootBaseCooldown;
    }

    [Tooltip("Take damage then check if health is zero.")]
    public void TakeDamage(GameObject source)
    {
        // Ignore self
        if (source.tag == "Enemy") { return; }

        //Debug.Log("Took damage from " + source.name + " from " + source.gameObject.tag);
        Health--;

        // Invoke OnHit events
        GameManager.Instance.SetHitStopDuration(0.3f);
        GameManager.Instance.InvokeOnHitEvents();

        // Die
        if (Health <= 0)
        {
            GameManager.Instance.InvokeOnEnemyDeathEvents();
            Destroy(gameObject);
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
