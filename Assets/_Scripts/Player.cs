using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour, IDamageable
{
    public delegate void OnTimeSlash();
    public event OnTimeSlash e_TimeSlash;

    [SerializeField] private int _health = 1;
    [SerializeField] private float _timeSliceDistance = 10f;
    [SerializeField] private float _attackCooldown = .7f;
    [SerializeField] private float _timeSliceCooldown = 3f;
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private GameObject slashPoint;
    private float raycastThickness = 3f;

    private ObjectPooler_Slash slashPool;
    private SpriteRenderer sprite;
    private float attackCooldownTimer = 0f;
    private float timeSliceCooldownTimer = 0f;
    private bool invincible = false;
    private bool _timeShiftActive = false;
    private bool _canUseTimeShift = true;

    private void Awake()
    {
        // Get slash object pooling instance
        slashPool = ObjectPooler_Slash.Instance;
        // Get reference to sprite renderer
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        InitialiseObjectPooling();
    }

    private void Update()
    {
        TickAttackCooldowns();
    }

    [Tooltip("Only allow players to attack on a set cooldown.")]
    private void TickAttackCooldowns()
    {
        // Normal Attack
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime * GameManager.Instance.InGameTimeScale;
        }

        // Time Slice
        if (timeSliceCooldownTimer > 0f)
        {
            CanUseTimeShift = false;
            timeSliceCooldownTimer -= Time.deltaTime * GameManager.Instance.InGameTimeScale;
        }
        else
        {
            CanUseTimeShift = true;
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
        slash.gameObject.layer = LayerMask.NameToLayer("Player");
        // Activate slash
        slash.SetActive(true);
        // Reset attack cooldown timer
        attackCooldownTimer = AttackCooldown;
    }

    [Tooltip("Player dash attack. Attacks in a straight line slashing everything in its path.")]
    public void DashAttack()
    {
        // Can only time slice if cooldown timer meets the required value
        if (timeSliceCooldownTimer > 0f) { return; }

        // TODO: Perform time slice logic
        Vector2 origin = transform.position + new Vector3(-transform.localScale.x / 2f, 0f, 0f);
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

                // Trigger interface methods for IsAttackable and IsDamageable
                if (hit[i].collider.TryGetComponent<IAttackable>(out IAttackable attackCol))
                {
                    attackCol.IsAttacked(gameObject);
                }
                if (hit[i].collider.TryGetComponent<IDamageable>(out IDamageable damageCol))
                {
                    damageCol.TakeDamage(gameObject);
                }
            }
        }

        // Call time slash events. (Teleport player forward)
        e_TimeSlash?.Invoke();

        // Reset cooldown
        timeSliceCooldownTimer = TimeShiftCooldown;
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

        if (Health <= 0 )
        {
            // TODO: Game Over
            //Debug.Log("Player has died.");
        }
        else
        {
            // TODO: Move sprite flash here
            // TODO: Move invincibility here
        }
        // Start invincibility after getting hit
        StartCoroutine(AfterHitInvincibility());
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

    public float TimeShiftCooldown
    {
        get { return _timeSliceCooldown; }
        private set { _timeSliceCooldown = value; }
    }

    public bool TimeShiftActive
    {
        get { return _timeShiftActive; }
        set { _timeShiftActive = value; }
    }

    public bool CanUseTimeShift
    {
        get { return _canUseTimeShift; }
        private set { _canUseTimeShift = value; }
    }

}
