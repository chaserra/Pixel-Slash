using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    // Events
    public delegate void OnHit();
    public event OnHit e_Hit;

    // Attributes
    [SerializeField] private float _originalHitStopDuration = 0.1f;
    [Tooltip("This is the special timescale used for the game. Prevents directly manipulating Unity's timescale.")]
    private float _inGameTimeScale = 1f;
    private float _pauseScale = 0f;
    private float _playScale = 1f;
    private float _slowTimeScale = 0.2f;
    private float _hitStopDuration = 0.1f;

    // State
    private bool _timeSlowed = false;
    private bool _isHitStopActive = false;

    // References
    private HitStop _hitStop;

    private void Awake()
    {
        // Setup Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);

        // Create new HitStop class
        _hitStop = new HitStop(this);
    }

    private void Start()
    {
        
    }

    private void Update()
    {

    }

    [Tooltip("Adapter method to call OnHit events.")]
    public void InvokeOnHitEvents()
    {
        e_Hit?.Invoke();
    }

    [Tooltip("Pauses the game using the game's special timescale. This will not affect Unity's built-in timescale. If you want to hard pause the game, modify Unity's timescale value instead.")]
    public void PauseGame()
    {
        InGameTimeScale = _pauseScale;
    }

    [Tooltip("Resets the game's special timescale to normal speed.")]
    public void ResumeGame()
    {
        if (_isHitStopActive) { return; } // Prioritise hitstop effect
        InGameTimeScale = _playScale;
        Time.timeScale = 1f;
        //IsTimeSlowed = false;
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

}
