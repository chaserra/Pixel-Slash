using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private Enemy enemy;

    private void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    public void OnAttack()
    {
        enemy.OnAttack();
    }

    public void OnShoot()
    {
        enemy.OnShoot();
    }
}
