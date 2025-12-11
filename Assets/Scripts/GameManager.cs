using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInput playerInput;
    [Header("Start menu")]
    public Button startButton;
    public GameObject startMenu;
    public InputActionReference exitAction;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }
    void StartGame()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);

    }
    private void Update()
    {
        if (exitAction != null && exitAction.action.WasPerformedThisFrame())
        {
            Application.Quit();
        }
    }

    


}
