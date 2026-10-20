using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelResultUI : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text title;
    [SerializeField] private Button actionButton;
    [SerializeField] private Text actionLabel;

    private bool won;

    private void Awake()
    {
        resultPanel.SetActive(false);
        actionButton.onClick.AddListener(HandleAction);
    }

    private void OnDestroy()
    {
        actionButton.onClick.RemoveListener(HandleAction);
    }

    public void Show(bool playerWon)
    {
        won = playerWon;
        title.text = won ? "You Win!" : "You Fell!";
        actionLabel.text = won ? "Exit" : "Play Again";
        resultPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(actionButton.gameObject);
    }

    private void HandleAction()
    {
        if (won)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
