using UnityEngine;

[RequireComponent (typeof(Player), typeof(PlayerController))]
public class TimeDilation : MonoBehaviour
{
    [SerializeField] private float _maxTimeDilationDuration = 5f;

    private float _timeDilationTimer = 0f;

    private Player _player;
    private PlayerController _controller;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _controller = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        _controller.e_ShiftHeld += ActivateTimeDilation;
        _controller.e_ShiftReleased += DeactivateTimeDilation;
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
    }

    // TODO: Fix GameManager property setting. Try to abstract as much as possible
    [Tooltip("Slowly return time speed to normal while time dilation is active.")]
    private void TimeDilationDecay()
    {
        // Only decay if time is slowed down.
        if (GameManager.Instance.IsTimeSlowed)
        {
            // While timer is less than max allowed
            if (_timeDilationTimer < _maxTimeDilationDuration)
            {
                // Increment timer
                _timeDilationTimer += Time.deltaTime;

                // If timer is 60% of max allowed. And ensure timescale does not go higher than 1
                if (_timeDilationTimer > _maxTimeDilationDuration * 0.6f &&
                    GameManager.Instance.InGameTimeScale < 1f)
                {
                    // Slowly return time speed back to 1
                    float newTimeScaleValue = GameManager.Instance.InGameTimeScale + (_maxTimeDilationDuration * 0.4f) / 100f;
                    GameManager.Instance.SetInGameTimeScale(newTimeScaleValue);
                }
            }
            // If exceeded, cancel time dilation
            else
            {
                GameManager.Instance.ResumeGame();
            }
        }
        // Reset timer if time is not slowed
        else
        {
            _timeDilationTimer = 0f;
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
}
