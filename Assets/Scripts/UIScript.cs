using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript Instance;

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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {

        returnButton.onClick.AddListener(RestartGame);
        gameOverMenu.SetActive(false);
        ammoPanel.SetActive(true);
        ammoCounter.text = Player.Instance.shotsLeft.ToString();
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

        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }
}
