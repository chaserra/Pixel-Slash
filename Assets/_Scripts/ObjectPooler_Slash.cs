using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler_Slash : ObjectPooler
{
    private static ObjectPooler_Slash _instance;
    public static ObjectPooler_Slash Instance {  get { return _instance; } }

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
    }

    protected override void Start()
    {
        base.Start();
    }

    [Tooltip("Ensure that the correct component is pooled.")]
    public override bool TypeSanityCheck()
    {
        // Ensure the correct object type is passed as the prefab
        if (!ObjectToPool.TryGetComponent<Slash>(out Slash component))
        {
            Debug.LogError("Incorrect component type assigned to pool! " + gameObject.name +
                " is looking for an object with a Slash component!");
            return false;
        }
        return true;
    }

}
