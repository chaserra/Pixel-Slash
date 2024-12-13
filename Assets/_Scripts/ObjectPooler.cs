using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPooler : MonoBehaviour
{
    [SerializeField] protected int _initialNumToPool = 5;
    protected GameObject _objectToPool;
    protected List<GameObject> _objectPool;

    public abstract bool TypeSanityCheck();

    protected virtual void Awake()
    {
        _objectPool = new List<GameObject>();
    }

    protected virtual void Start()
    {

    }

    [Tooltip("Create inital pool of objects.")]
    public void InitialisePooling()
    {
        // Create inital number of objects to pool
        for (int i = 0; i < _initialNumToPool; i++)
        {
            CreateNewObject();
        }
    }

    [Tooltip("Returns an inactive pooled object. Creates a new object and pools if none is found.")]
    public GameObject GetPooledObject()
    {
        // Check each child object and look for an inactive object
        foreach (GameObject o in ObjectPool)
        {
            // If an inactive object is found
            if (!o.activeSelf)
            {
                return o;
            }
        }

        // If none found, create a new pooled object
        return CreateNewObject();
    }

    [Tooltip("Creates a new object and adds it to the pool.")]
    protected GameObject CreateNewObject()
    {
        GameObject newObj;

        // Instantiate object, child to this pool's transform, then deactivate object
        newObj = Instantiate(_objectToPool, this.gameObject.transform);
        newObj.gameObject.SetActive(false);
        ObjectPool.Add(newObj);

        return newObj;
    }

    protected int InitialNumberToPool
    {
        get { return _initialNumToPool; }
    }

    public GameObject ObjectToPool
    {
        get { return _objectToPool; }
        set { _objectToPool = value; }
    }

    public List<GameObject> ObjectPool
    {
        get { return _objectPool; }
    }
}
