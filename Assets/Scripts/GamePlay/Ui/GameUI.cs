using Match3.Gameplay.Game;
using TMPro;
using UnityEngine;

namespace Match3.Gameplay.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI targetText;

        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        private void Awake()
        {
            if (gameManager == null)
                gameManager =
                    FindFirstObjectByType<GameManager>();
        }

        private void Start()
        {
            HideResultPanels();
            UpdateUI();
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (gameManager == null)
                return;

            if (scoreText != null)
            {
                scoreText.text =
                    "Score: " +
                    gameManager.Score;
            }

            if (targetText != null)
            {
                targetText.text =
                    "Target: " +
                    gameManager.TargetScore;
            }
        }

        public void HideResultPanels()
        {
            if (winPanel != null)
                winPanel.SetActive(false);

            if (losePanel != null)
                losePanel.SetActive(false);
        }

        public void ShowWinPanel()
        {
            if (losePanel != null)
                losePanel.SetActive(false);

            if (winPanel != null)
                winPanel.SetActive(true);
        }

        public void ShowLosePanel()
        {
            if (winPanel != null)
                winPanel.SetActive(false);

            if (losePanel != null)
                losePanel.SetActive(true);
        }
    }
}