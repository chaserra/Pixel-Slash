using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health = 1;
    [SerializeField] private float _attackCooldown = .7f;
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private GameObject slashPoint;

    private ObjectPooler_Slash slashPool;
    private SpriteRenderer sprite;
    private float attackCooldownTimer = 0f;
    private bool invincible = false;

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
        TickAttackCooldown();
    }

    [Tooltip("Only allow players to attack on a set cooldown.")]
    private void TickAttackCooldown()
    {
        if (attackCooldownTimer <= 0f) { return; }
        attackCooldownTimer -= Time.deltaTime * GameManager.Instance.InGameTimeScale;
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
            Debug.Log("Player has died.");
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

    public float AttackCooldown
    {
        get { return _attackCooldown; }
        private set { _attackCooldown = value; }
    }

}
