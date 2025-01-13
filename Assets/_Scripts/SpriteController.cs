using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class SpriteController : MonoBehaviour
{
    private PlayerController playerController;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector2 movementVector;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        movementVector = playerController.MovementVector;
        if (movementVector.x > 0)
        {
            sprite.flipX = true;
        } 
        else
        {
            sprite.flipX= false;
        }
        anim.SetFloat("Horizontal", movementVector.x);
        anim.SetFloat("Vertical", movementVector.y);
    }
}
