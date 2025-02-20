using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler_Bullet : ObjectPooler
{
    [SerializeField] private GameObject bulletPrefab;

    private static ObjectPooler_Bullet _instance;
    public static ObjectPooler_Bullet Instance { get { return _instance; } }

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
        //DontDestroyOnLoad(gameObject);
        base.Awake();
        ObjectToPool = bulletPrefab;
    }

    protected override void Start()
    {
        base.Start();
        InitialisePooling();
    }

    [Tooltip("Ensure that the correct component is pooled.")]
    public override bool TypeSanityCheck()
    {
        // Ensure the correct object type is passed as the prefab
        if (!ObjectToPool.TryGetComponent<Bullet>(out Bullet component))
        {
            Debug.LogError("Incorrect component type assigned to pool! " + gameObject.name +
                " is looking for an object with a Bullet component!");
            return false;
        }
        return true;
    }

}
