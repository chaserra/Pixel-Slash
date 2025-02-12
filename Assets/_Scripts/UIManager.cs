using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static GameManager;

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
        GameManager.Instance.e_PlayerWin += OnPlayerWin;
        GameManager.Instance.e_GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.Instance.e_PlayerWin -= OnPlayerWin;
        GameManager.Instance.e_GameOver -= OnGameOver;
    }

    #region PAUSE
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
    #endregion

    #region WIN
    public void OnPlayerWin()
    {
        winScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstWinSelected);
    }
    #endregion

    #region GAME OVER
    public void OnGameOver()
    {
        gameOverScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstGameOverSelected);
    }
    #endregion

    #region BUTTONS
    public void OnMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExit()
    {
        Application.Quit();
    }
    #endregion
}
