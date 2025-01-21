using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler_Dash : ObjectPooler
{
    private static ObjectPooler_Dash _instance;
    public static ObjectPooler_Dash Instance { get { return _instance; } }

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
    }

    protected override void Start()
    {
        base.Start();
    }

    [Tooltip("Ensure that the correct component is pooled.")]
    public override bool TypeSanityCheck()
    {
        // Ensure the correct object type is passed as the prefab
        if (!ObjectToPool.TryGetComponent<VFX>(out VFX component))
        {
            Debug.LogError("Incorrect component type assigned to pool! " + gameObject.name +
                " is looking for an object with a Slash component!");
            return false;
        }
        return true;
    }

}
