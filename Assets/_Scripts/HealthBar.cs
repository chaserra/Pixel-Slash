using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void IsDamaged()
    {
        animator.SetTrigger("TakeDamage");
    }

    public void IsHealed()
    {
        animator.SetTrigger("Healed");
    }
}
