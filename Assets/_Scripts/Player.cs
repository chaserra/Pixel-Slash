using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public delegate void OnTimeSlash();
    public event OnTimeSlash e_TimeSlash;

    [SerializeField] private int _health = 1;
    [SerializeField] private float _timeSliceDistance = 10f;
    [SerializeField] private float _attackCooldown = .7f;
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private GameObject slashPoint;
    [SerializeField] private GameObject spriteObject;
    [SerializeField] private GameObject dashPrefab;
    private float raycastThickness = 3f;

    private ObjectPooler_Slash slashPool;
    private ObjectPooler_Dash dashPool;
    private SpriteRenderer sprite;
    private float attackCooldownTimer = 0f;
    private bool invincible = false;
    private bool _timeShiftActive = false;
    private bool _canUseTimeShift = true;
    private bool _recentlyTimeShifted = false;

    private HealthBar healthBar;
    private EnergyBar energyBar;

    private void Awake()
    {
        // Get object pooling instances
        slashPool = ObjectPooler_Slash.Instance;
        dashPool = ObjectPooler_Dash.Instance;
        // Get reference to sprite renderer
        sprite = GetComponentInChildren<SpriteRenderer>();
        // Get UI Stuff
        healthBar = FindFirstObjectByType<HealthBar>();
        energyBar = FindFirstObjectByType<EnergyBar>();
    }

    private void Start()
    {
        InitialiseObjectPooling();
    }

    private void Update()
    {
        TickAttackCooldowns();
        // Make sure sprite is upright
        spriteObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    [Tooltip("Only allow players to attack on a set cooldown or conditions.")]
    private void TickAttackCooldowns()
    {
        // Normal Attack
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime * GameManager.Instance.InGameTimeScale;
        }

        // Time Slice
        if (_recentlyTimeShifted)
        {
            // Wait for energy to fill up
            _canUseTimeShift = false;

            // If energy is full
            if (energyBar.EnergyRatio >= 1f)
            {
                // Remove flag
                _recentlyTimeShifted = false;
            }
        }
        // If energy is full (no longer recently TimeShifted)
        else
        {
            // Reenable TimeShift
            _canUseTimeShift = true;
        }

        // If all energy is used up
        if (energyBar.EnergyRatio <= 0f)
        {
            // Disallow TimeShift
            _recentlyTimeShifted = true;
        }
    }

    [Tooltip("Player attack. Spawns a pooled slash object.")]
    public void Attack()
    {
        // Can only attack if cooldown timer meets the required value.
        if (attackCooldownTimer > 0f) { return; }

        // Find a pooled slash object
        GameObject slash = slashPool.GetPooledObject();
        // Spawn in front of the player and copy rotation
        slash.transform.position = slashPoint.transform.position;
        slash.transform.rotation = transform.rotation;
        // Set tag and layer
        slash.gameObject.tag = "Player";
        //slash.gameObject.layer = LayerMask.NameToLayer("Player");
        // Activate slash
        slash.SetActive(true);
        // Reset attack cooldown timer
        attackCooldownTimer = AttackCooldown;
    }

    [Tooltip("Player dash attack. Attacks in a straight line slashing everything in its path.")]
    public void DashAttack()
    {
        // Can only time slice if cooldown timer meets the required value
        if (energyBar.EnergyRatio <= 0f) { return; }

        // Raycast properties
        //Vector2 origin = transform.position + new Vector3(-transform.localScale.x / 2f, 0f, 0f);
        Vector2 boxSize = new Vector2(1f, 1f) + Vector2.right * raycastThickness;
        Vector2 dir = transform.up;

        // Raycast forward
        RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position, boxSize, 0f, dir, TimeSliceDistance);

        // If another object is hit (aside from the player)
        if (hit.Length > 1)
        {
            // Loop through all hit objects
            for (int i = 0; i < hit.Length; i++)
            {
                // Ignore objects tagged as Player
                if (hit[i].collider.gameObject.tag == "Player") { continue; }

                // Create another raycast between player and hit object
                int wallLayer = LayerMask.NameToLayer("Wall"); // Get Wall layer index
                LayerMask filteredLayer = (1 << wallLayer); // Only check for walls
                RaycastHit2D raycastToHitObject = Physics2D.Linecast(transform.position, hit[i].collider.transform.position, filteredLayer);
                // Ignore if there's a wall between them
                if (raycastToHitObject) { continue; }

                // Trigger interface methods for IsAttackable and IsDamageable
                if (hit[i].collider.TryGetComponent<IAttackable>(out IAttackable attackCol))
                {
                    GameManager.Instance.SetHitStopDuration(0.35f);
                    attackCol.IsAttacked(gameObject);
                }
                if (hit[i].collider.TryGetComponent<IDamageable>(out IDamageable damageCol))
                {
                    damageCol.TakeDamage(gameObject);
                }
            }
        }

        // Enable dash effect
        // Find a pooled dash object
        GameObject dash = dashPool.GetPooledObject();
        // Spawn in front of the player and copy rotation
        dash.transform.position = transform.position;
        dash.transform.rotation = transform.rotation;

        // Call time slash events. (Teleport player forward)
        e_TimeSlash?.Invoke();

        // Activate slash
        dash.SetActive(true);

        // Reset cooldown
        _recentlyTimeShifted = true;
    }

    [Tooltip("Take damage then check if health is zero.")]
    public void TakeDamage(GameObject source)
    {
        // Ignore self
        if (source.tag == "Player") { return; }
        // Do nothing if invincible
        if (invincible) { return; }
        // Decrement health
        Health--;
        // Flash player sprite
        StartCoroutine(SpriteEffects.FlashSprite(sprite, 0.25f));

        // Play health UI animation
        healthBar.IsDamaged();
        GameManager.Instance.InvokePlayerTakeDamageEvents();

        if (Health <= 0 )
        {
            // TODO: Game Over
            //Debug.Log("Player has died.");
        }
        else
        {
            // Start invincibility after getting hit
            StartCoroutine(AfterHitInvincibility());
        }
    }

    private IEnumerator AfterHitInvincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(1f);
        invincible = false;
    }

    [Tooltip("Pool the assigned slash prefab.")]
    private void InitialiseObjectPooling()
    {
        // If a Slash Object Pool exists
        if (slashPool != null)
        {
            // Assign slash prefab
            slashPool.ObjectToPool = slashPrefab;
            // If sanity check passes
            if (slashPool.TypeSanityCheck())
            {
                // Initialise pooled objects
                slashPool.InitialisePooling();
            }
        }

        // If a Dash Object Pool exists
        if (dashPool != null)
        {
            // Assign dash prefab
            dashPool.ObjectToPool = dashPrefab;
            // If sanity check passes
            if (dashPool.TypeSanityCheck())
            {
                // Initialise pooled objects
                dashPool.InitialisePooling();
            }
        }
    }

    public int Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    public float TimeSliceDistance
    {
        get { return _timeSliceDistance; }
        private set { _timeSliceDistance = value; }
    }

    public float AttackCooldown
    {
        get { return _attackCooldown; }
        private set { _attackCooldown = value; }
    }

    public bool TimeShiftActive
    {
        get { return _timeShiftActive; }
        set { _timeShiftActive = value; }
    }

    public bool RecentlyTimeShifted
    {
        get { return _recentlyTimeShifted; }
        set { _recentlyTimeShifted = value; }
    }

    public bool CanUseTimeShift
    {
        get { return _canUseTimeShift; }
        private set { _canUseTimeShift = value; }
    }

}
