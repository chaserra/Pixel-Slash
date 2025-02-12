using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
