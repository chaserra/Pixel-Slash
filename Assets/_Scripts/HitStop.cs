using System.Collections;
using UnityEngine;

public class HitStop 
{
    private GameManager _gameManager;

    /// <summary>
    /// Default constructor for the HitStop class.
    /// </summary>
    public HitStop()
    {
        _gameManager = GameManager.Instance;
        GameManager.Instance.e_Hit += OnHit;
    }

    /// <summary>
    /// Constructor for the hitstop class.
    /// </summary>
    /// <param name="gameManager">The GameManager object.</param>
    public HitStop(GameManager gameManager)
    {
        _gameManager = gameManager;
        GameManager.Instance.e_Hit += OnHit;
    }

    /// <summary>
    /// Event method for OnHit. Triggers the HitStop effect.
    /// </summary>
    public void OnHit()
    {
        // Start Coroutine by calling it from a MonoBehaviour class
        _gameManager.StartCoroutine(TriggerHitStop());
    }

    /// <summary>
    /// The Coroutine to switch the game's special timescale on and off.
    /// </summary>
    /// <returns>Coroutine WaitForSeconds</returns>
    IEnumerator TriggerHitStop()
    {
        // Briefly pause the game
        GameManager.Instance.IsHitStopActive = true;
        GameManager.Instance.PauseGame();
        yield return new WaitForSeconds(GameManager.Instance.HitStopDuration);
        // Reset HitStopDuration to original value (for hitstops that take longer)
        GameManager.Instance.ResetHitStopDuration();
        // Then revert back to normal in-game time
        GameManager.Instance.ResumeGame();
        GameManager.Instance.IsHitStopActive = false;
    }

}
