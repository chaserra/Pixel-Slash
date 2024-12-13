using UnityEngine;

public class Slash : MonoBehaviour
{
    [SerializeField] private float _maxLifespan = .2f;
    private float lifespanTimer = 0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        TickLifetime();
    }

    private void OnDisable()
    {
        lifespanTimer = 0f;
    }

    [Tooltip("Disable the object when lifespan value is met.")]
    private void TickLifetime()
    {
        lifespanTimer += Time.deltaTime;
        if (lifespanTimer >= _maxLifespan)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform collisionParent = collision.attachedRigidbody.transform;

        // Call interface method
        if (collisionParent.TryGetComponent<IAttackable>(out IAttackable attackCol))
        {
            attackCol.IsAttacked(gameObject);
            // TODO: Trigger hitstop. Not sure where to put this yet...
        }
        if (collisionParent.TryGetComponent<IDamageable>(out IDamageable damageCol))
        {
            damageCol.TakeDamage(gameObject);
        }
    }

    public float MaxLifespan
    {
        get { return _maxLifespan; }
        private set { _maxLifespan = value; }
    }
}
