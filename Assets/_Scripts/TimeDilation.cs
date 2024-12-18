using UnityEngine;

public class TimeDilation : MonoBehaviour
{
    [SerializeField] private float _maxTimeDilationDuration = 5f;

    private float _timeDilationTimer = 0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        TimeDilationDecay();
    }

    // TODO: DECOUPLE. Also, fix GameManager property setting. Try to abstract as much as possible
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

                // If timer is 80% of max allowed
                if (_timeDilationTimer > _maxTimeDilationDuration * 0.8f)
                {
                    // Ensure timescale does not go higher than 1
                    if (GameManager.Instance.InGameTimeScale < 1f)
                    {
                        // Slowly return time speed back to 1
                        float newTimeScaleValue = GameManager.Instance.InGameTimeScale + (_maxTimeDilationDuration * 0.2f) / 100f;
                        GameManager.Instance.SetInGameTimeScale(newTimeScaleValue);
                        
                    }
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
}
