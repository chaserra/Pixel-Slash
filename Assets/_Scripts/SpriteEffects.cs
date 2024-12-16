using System.Collections;
using UnityEngine;

public static class SpriteEffects 
{
    public static IEnumerator FlashSprite(SpriteRenderer sprite, float duration)
    {
        Color white = Color.white;
        Color originalColor = sprite.color;
        float timer = 0f;

        while (timer < duration)
        {
            sprite.color = white;
            yield return null;
            sprite.color = originalColor;
            yield return null;
            timer += Time.deltaTime * GameManager.Instance.InGameTimeScale;
        }
        sprite.color = originalColor;
        yield return null;
    }

    public static IEnumerator FlashSprite(SpriteRenderer sprite, float duration, float flashGaps)
    {
        Color white = Color.white;
        Color originalColor = sprite.color;
        float timer = 0f;

        while (timer < duration)
        {
            sprite.color = white;
            yield return new WaitForSeconds(flashGaps);
            sprite.color = originalColor;
            yield return new WaitForSeconds(flashGaps);
            timer += Time.deltaTime * GameManager.Instance.InGameTimeScale;
        }
        sprite.color = originalColor;
        yield return null;
    }
}
