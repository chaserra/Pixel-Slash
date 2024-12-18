using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public delegate void OnHit();
    public event OnHit e_Hit;

    [SerializeField] private float _hitStopDuration = 0.1f;
    [Tooltip("This is the special timescale used for the game. Prevents directly manipulating Unity's timescale.")]
    private float _inGameTimeScale = 1f;
    private float _pauseScale = 0f;
    private float _playScale = 1f;
    private float _slowTimeScale = 0.2f;

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
        InGameTimeScale = _playScale;
    }

    public float HitStopDuration
    {
        get { return _hitStopDuration; }
    }

    public float InGameTimeScale
    {
        get { return _inGameTimeScale; }
        private set { _inGameTimeScale = value; }
    }
}
