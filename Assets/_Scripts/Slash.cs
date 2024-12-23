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
        lifespanTimer += Time.deltaTime * GameManager.Instance.InGameTimeScale;
        if (lifespanTimer >= _maxLifespan)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get object with the rigidbody component
        Transform collisionParent = collision.attachedRigidbody.transform;

        // Check if there's a wall between the player and the slash object
        Vector2 origin = transform.position - transform.up;
        int wallLayer = LayerMask.NameToLayer("Wall");
        LayerMask layerFilter = 1 << wallLayer;

        // Ignore anything beyond the wall
        RaycastHit2D hit = Physics2D.Linecast(origin, collisionParent.transform.position, layerFilter);
        if (hit) { return; }

        // Call interface method
        if (collisionParent.TryGetComponent<IAttackable>(out IAttackable attackCol))
        {
            attackCol.IsAttacked(gameObject);
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
