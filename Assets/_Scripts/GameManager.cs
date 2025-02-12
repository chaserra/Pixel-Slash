using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    // Events
    public delegate void OnHit();
    public event OnHit e_Hit;
    public delegate void OnPlayerTakeDamage();
    public event OnPlayerTakeDamage e_PlayerTakeDamage;
    //public delegate void OnEnemyDeath();
    //public event OnEnemyDeath e_EnemyDeath;
    public delegate void OnPlayerWin();
    public event OnPlayerWin e_PlayerWin;
    public delegate void OnGameOver();
    public event OnGameOver e_GameOver;

    // Attributes
    [SerializeField] private float _originalHitStopDuration = 0.1f;
    [Tooltip("This is the special timescale used for the game. Prevents directly manipulating Unity's timescale.")]
    private float _inGameTimeScale = 1f;
    private float _pauseScale = 0f;
    private float _playScale = 1f;
    private float _slowTimeScale = 0.2f;
    private float _hitStopDuration = 0.1f;

    // State
    private bool _isGamePaused = false;
    private bool _timeSlowed = false;
    private bool _isHitStopActive = false;
    private int _numEnemies = 0;
    private bool _isGameOver = false;

    // References
    private HitStop _hitStop;

    private void Awake()
    {
        // Ensure Unity timeScale is reset
        Time.timeScale = 1f;

        // Setup Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        //DontDestroyOnLoad(gameObject);

        // Create new HitStop class
        _hitStop = new HitStop(this);
    }

    private void Start()
    {
        _numEnemies = FindFirstObjectByType<ObjectPooler_Enemy>().InitialNumberToPool;
    }

    private void Update()
    {

    }

    [Tooltip("Adapter method to call OnHit events.")]
    public void InvokeOnHitEvents()
    {
        e_Hit?.Invoke();
    }

    [Tooltip("Adapter method to call PlayerTakeDamage events.")]
    public void InvokePlayerTakeDamageEvents()
    {
        e_PlayerTakeDamage?.Invoke();
    }

    [Tooltip("Adapter method to call events related to enemy deaths.")]
    public void InvokeOnEnemyDeathEvents()
    {
        _numEnemies--;
        //e_EnemyDeath?.Invoke();
        if (_numEnemies <= 0)
        {
            // Player wins
            e_PlayerWin?.Invoke();
            FullPause();
        }
    }

    [Tooltip("Adapter method to call events after a game over.")]
    public void InvokeGameOver()
    {
        _isGameOver = true;
        e_GameOver?.Invoke();
        FullPause();
    }

    [Tooltip("Pauses the game using the game's special timescale. This will not affect Unity's built-in timescale. If you want to hard pause the game, then use FullPause")]
    public void PauseGame()
    {
        InGameTimeScale = _pauseScale;
    }

    [Tooltip("Resets the game's special timescale to normal speed.")]
    public void ResumeGame()
    {
        if (_isHitStopActive) { return; } // Prioritise hitstop effect
        InGameTimeScale = _playScale;
        //Time.timeScale = 1f;
        //IsTimeSlowed = false;
    }

    [Tooltip("Fully pauses the game using Unity's timeScale.")]
    public void FullPause()
    {
        Time.timeScale = 0f;
        _isGamePaused = true;
    }

    [Tooltip("Fully resumes the game using Unity's timeScale.")]
    public void FullResume()
    {
        Time.timeScale = 1f;
        _isGamePaused = false;
    }

    [Tooltip("Slows down the game using a special timescale.")]
    public void SlowDownTime()
    {
        if (_isHitStopActive) { return; } // Prioritise hitstop effect
        if (IsTimeSlowed) { return; } // If time is already slowed, ignore this and use the TimeDilationDecay values instead
        InGameTimeScale = _slowTimeScale;
        IsTimeSlowed = true;
    }

    public void SetHitStopDuration(float duration)
    {
        HitStopDuration = duration;
    }

    public void ResetHitStopDuration()
    {
        HitStopDuration = OriginalHitStopDuration;
    }

    public void SetInGameTimeScale(float timeValue)
    {
        InGameTimeScale = timeValue;
    }

    public float OriginalHitStopDuration
    {
        get { return _originalHitStopDuration; }
    }

    public float HitStopDuration
    {
        get { return _hitStopDuration; }
        private set { _hitStopDuration = value; }
    }

    public float InGameTimeScale
    {
        get { return _inGameTimeScale; }
        private set { _inGameTimeScale = value; }
    }

    public float SlowTimeScale
    {
        get { return _slowTimeScale; }
    }

    public bool IsGamePaused
    {
        get { return _isGamePaused; }
    }

    public bool IsTimeSlowed
    {
        get { return _timeSlowed; }
        set { _timeSlowed = value; }
    }

    public bool IsHitStopActive
    {
        get { return _isHitStopActive; }
        set { _isHitStopActive = value; }
    }

    public bool IsGameOver
    {
        get { return _isGameOver; }
    }

}
