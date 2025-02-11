using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Singleton
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    private PlayerInput playerInput;

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;

    [SerializeField] private GameObject firstPauseSelected;
    [SerializeField] private GameObject firstGameOverSelected;
    [SerializeField] private GameObject firstWinSelected;

    private bool isPaused = false;

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
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnPause()
    {
        // Toggle flag
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause the game
            GameManager.Instance.FullPause();
            // Show Pause Screen
            pauseScreen.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstPauseSelected);
        }
        else
        {
            GameManager.Instance.FullResume();
            // Remove Pause Screen
            pauseScreen.SetActive(false);
        }
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
