using UnityEngine;
using UnityEngine.UI;

public class SyncSpriteToImage : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Image imageComponent;

    void LateUpdate()
    {
        // Ensure both references are assigned
        if (spriteRenderer != null && imageComponent != null)
        {
            // Update the Image's sprite with the SpriteRenderer's sprite
            imageComponent.sprite = spriteRenderer.sprite;
        }
    }
}