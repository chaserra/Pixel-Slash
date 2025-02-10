using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler_Enemy : ObjectPooler
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector2 areaSize;
    [SerializeField] private float minDistanceFromWall = 1.5f;
    [SerializeField] private float minDistanceFromEnemy = 3f;
    [SerializeField] private float minDistanceFromPlayer = 11f;

    private static ObjectPooler_Enemy _instance;
    public static ObjectPooler_Enemy Instance { get { return _instance; } }

    protected override void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
        base.Awake();
        ObjectToPool = enemyPrefab;
    }

    protected override void Start()
    {
        base.Start();
        InitialisePooling();
        SpawnAtRandomPosition();
    }

    [Tooltip("Ensure that the correct component is pooled.")]
    public override bool TypeSanityCheck()
    {
        // Ensure the correct object type is passed as the prefab
        if (!ObjectToPool.TryGetComponent<Enemy>(out Enemy component))
        {
            Debug.LogError("Incorrect component type assigned to pool! " + gameObject.name +
                " is looking for an object with a Slash component!");
            return false;
        }
        return true;
    }

    private void SpawnAtRandomPosition()
    {
        Vector2 randomPos = Vector2.zero;

        // For every pooled enemy
        for (int i = 0; i < InitialNumberToPool; i++)
        {
            GameObject currentEnemyToSpawn = ObjectPool[i];
            bool goodSpawn = false;

            while (!goodSpawn)
            {
                // Set flag to true (logic below will indicate if spawn position is bad
                goodSpawn = true;

                // Pick a random position in the map
                randomPos.x = Random.Range(-areaSize.x, areaSize.x);
                randomPos.y = Random.Range(-areaSize.y, areaSize.y);

                // Spawn enemy in the random position
                currentEnemyToSpawn.transform.position = randomPos;

                // Check if near a player, another enemy, or a wall
                Collider2D[] colliders = Physics2D.OverlapCircleAll(currentEnemyToSpawn.transform.position, minDistanceFromPlayer);
                for (int j = 0;  j < colliders.Length; j++)
                {
                    // Get distance from the collider
                    float collDistance = Vector2.Distance(currentEnemyToSpawn.transform.position, colliders[j].ClosestPoint(currentEnemyToSpawn.transform.position));

                    if (colliders[j].gameObject.tag == "Player")
                    {
                        if (collDistance <= minDistanceFromPlayer)
                        {
                            //Debug.Log("Too close from the Player.");
                            goodSpawn = false;
                            break;
                        }
                    }
                    else if (colliders[j].gameObject.tag == "Enemy")
                    {
                        if (collDistance <= minDistanceFromEnemy)
                        {
                            //Debug.Log("Too close from another enemy.");
                            goodSpawn = false;
                            break;
                        }
                    }
                    else if (colliders[j].gameObject.tag == "Wall")
                    {
                        if (collDistance <= minDistanceFromWall)
                        {
                            //Debug.Log("Too close from a wall. Distance: " + collDistance);
                            goodSpawn = false;
                            break;
                        }
                    }
                }
            }
            // Activate enemy
            currentEnemyToSpawn.gameObject.SetActive(true);
        }
    }
}
