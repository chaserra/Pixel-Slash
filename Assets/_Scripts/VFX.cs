using UnityEngine;

[RequireComponent (typeof(Animator))]
public class VFX : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnAnimEnd()
    {
        gameObject.SetActive(false);
    }

}
