using System.Collections;
using UnityEngine;

public class UIShake : MonoBehaviour
{
    [SerializeField] private Canvas hud;
    [SerializeField] private float magnitude = 4f;
    [SerializeField] private float shakeDuration = .15f;

    private RectTransform hudRect;
    private float timer = 0f;

    private void OnEnable()
    {
        //GameManager.Instance.e_PlayerTakeDamage += ShakeHUD;
    }

    private void Start()
    {
        if (hud == null)
        {
            hud = GetComponentInChildren<Canvas>();
        }
        hudRect = hud.GetComponent<RectTransform>();
        GameManager.Instance.e_PlayerTakeDamage += ShakeHUD;
    }

    private void OnDisable()
    {
        GameManager.Instance.e_PlayerTakeDamage -= ShakeHUD;
    }

    public void ShakeHUD()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        while (timer < shakeDuration && !GameManager.Instance.IsGameOver)
        {
            float vertical = Random.Range(-magnitude, magnitude);
            float horizontal = Random.Range(-magnitude, magnitude);

            Vector2 top = hudRect.offsetMax;
            top.y = vertical;
            hudRect.offsetMax = top;

            Vector2 left = hudRect.offsetMin;
            left.x = horizontal;
            hudRect.offsetMin = left;

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        hudRect.offsetMax = new Vector2(0, 0);
        hudRect.offsetMin = new Vector2(0, 0);
    }
}
