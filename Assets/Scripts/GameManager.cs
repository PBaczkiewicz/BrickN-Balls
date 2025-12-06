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

    [Header("Game over menu")]
    public GameObject gameOverMenu;
    public Button returnButton;
    public TextMeshProUGUI finalScoreText;

    [Header("Player UI")]
    public GameObject ammoPanel;
    public TextMeshProUGUI ammoCounter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        returnButton.onClick.AddListener(RestartGame);
        gameOverMenu.SetActive(false);
        ammoPanel.SetActive(false);
    }
    private void Update()
    {
        if (exitAction != null && exitAction.action.WasPerformedThisFrame())
        {
            Application.Quit();
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
        startMenu.SetActive(false);
    }

    public void GameOver()
    {
        Debug.Log("Game over");
        gameOverMenu.SetActive(true);
        finalScoreText.text = "Final Score: " + Player.Instance.points.ToString();
    }

    void RestartGame()
    {
        ammoPanel.SetActive(false);
        gameOverMenu.SetActive(false);
        SceneManager.UnloadSceneAsync("GameScene");
        startMenu.SetActive(true);
    }
}
