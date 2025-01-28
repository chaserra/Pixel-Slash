using UnityEngine;
using static Player;

[RequireComponent (typeof(Player), typeof(PlayerController))]
public class TimeDilation : MonoBehaviour
{
    [SerializeField] private float _maxTimeDilationDuration = 8f;

    private float _energyRatio;
    private float _timeDilationTimer = 0f;

    private Player _player;
    private PlayerController _controller;

    private EnergyBar energyBar;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _controller = GetComponent<PlayerController>();
        energyBar = FindFirstObjectByType<EnergyBar>();
    }

    private void OnEnable()
    {
        _controller.e_ShiftHeld += ActivateTimeDilation;
        _controller.e_ShiftReleased += DeactivateTimeDilation;
        _player.e_TimeSlash += OnTimeSlash;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        TimeDilationDecay();
    }

    private void OnDisable()
    {
        _controller.e_ShiftHeld -= ActivateTimeDilation;
        _controller.e_ShiftReleased -= DeactivateTimeDilation;
        _player.e_TimeSlash -= OnTimeSlash;
    }

    // TODO: Fix GameManager property setting. Try to abstract as much as possible
    [Tooltip("Slowly return time speed to normal while time dilation is active.")]
    private void TimeDilationDecay()
    {
        // Update UI energy fill
        EnergyRatio = 1 - (_timeDilationTimer / _maxTimeDilationDuration);
        energyBar.EnergyRatio = EnergyRatio;

        // Only decay if time is slowed down.
        if (GameManager.Instance.IsTimeSlowed)
        {
            // While timer is less than max allowed
            if (_timeDilationTimer < _maxTimeDilationDuration)
            {
                // Increment timer
                _timeDilationTimer += Time.deltaTime;

                // If timer is 60% of max allowed. And ensure timescale does not go higher than 1
                if (EnergyRatio < .4f &&
                    GameManager.Instance.InGameTimeScale < 1f)
                {
                    // Slowly return time speed back to 1
                    float newTimeScaleValue = Mathf.Lerp(GameManager.Instance.InGameTimeScale, 1f, 2f * Time.deltaTime);
                    GameManager.Instance.SetInGameTimeScale(newTimeScaleValue);
                }
            }
            // If exceeded, cancel time dilation
            else
            {
                GameManager.Instance.ResumeGame();
            }
        }
        // Slowly reset timer if time is not slowed
        else
        {
            if (_timeDilationTimer > 0f)
            {
                _timeDilationTimer -= 2f * Time.deltaTime;
                _timeDilationTimer = Mathf.Clamp(_timeDilationTimer, 0f, _maxTimeDilationDuration);
            }
        }
    }

    public void ActivateTimeDilation()
    {
        // TODO: Activate line indicator + other vfx
        // Slow down time via time dilation
        GameManager.Instance.SlowDownTime();
        _player.TimeShiftActive = true;
    }

    public void DeactivateTimeDilation()
    {
        // TODO: Deactivate line indicator + other vfx
        // Cancel time dilation
        GameManager.Instance.ResumeGame();
        GameManager.Instance.IsTimeSlowed = false;
        _player.TimeShiftActive = false;
    }

    public void OnTimeSlash()
    {
        // Expend all energy when time slash is used
        _timeDilationTimer = _maxTimeDilationDuration;
    }

    public float EnergyRatio
    {
        get { return _energyRatio; }
        private set { _energyRatio = value; }
    }
}
