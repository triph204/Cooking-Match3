using System.Collections;
using Match3.Gameplay.Board;
using Match3.Gameplay.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Gameplay.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int targetScore = 1000;
        [SerializeField] private int scorePerGem = 10;

        [Header("References")]
        [SerializeField] private GridManager grid;
        [SerializeField] private GameUI gameUI;

        [Header("Result Animation")]
        [SerializeField] private float resultRowDelay = 0.08f;
        [SerializeField] private float resultDisappearDuration = 0.3f;
        [SerializeField] private float resultWaitTime = 1f;

        public int Score { get; private set; }

        public int TargetScore => targetScore;

        public bool IsGameOver { get; private set; }

        public bool IsWin { get; private set; }

        public bool WinPending { get; private set; }

        public bool LosePending { get; private set; }

        private bool resultRunning;

        private const string UNLOCKED_LEVEL_KEY =
            "UnlockedLevel";

        private void Awake()
        {
            if (grid == null)
                grid = FindFirstObjectByType<GridManager>();

            if (gameUI == null)
                gameUI = FindFirstObjectByType<GameUI>();
        }

        private void Start()
        {
            Time.timeScale = 1f;

            Score = 0;
            IsGameOver = false;
            IsWin = false;

            WinPending = false;
            LosePending = false;

            resultRunning = false;

            if (gameUI != null)
                gameUI.HideResultPanels();
        }

        public void AddScore(int gemCount)
        {
            if (IsGameOver)
                return;

            if (gemCount <= 0)
                return;

            AddRawScore(
                gemCount * scorePerGem);
        }

        public void AddRawScore(int amount)
        {
            if (IsGameOver)
                return;

            if (amount <= 0)
                return;

            Score += amount;

            if (Score >= targetScore)
                WinPending = true;
        }

        public void LoseGame()
        {
            if (IsGameOver ||
                resultRunning)
                return;

            LosePending = true;
        }

        public IEnumerator PlayWinSequence()
        {
            if (resultRunning)
                yield break;

            resultRunning = true;

            IsGameOver = true;
            IsWin = true;
            WinPending = false;

            Time.timeScale = 0f;

            if (grid != null)
            {
                yield return StartCoroutine(
                    grid.PlayBoardClearAnimation(
                        resultRowDelay,
                        resultDisappearDuration));
            }

            UnlockNextLevel();

            if (gameUI != null)
                gameUI.ShowWinPanel();

            Time.timeScale = 0f;
        }

        public IEnumerator PlayLoseSequence()
        {
            if (resultRunning)
                yield break;

            resultRunning = true;

            IsGameOver = true;
            IsWin = false;
            LosePending = false;

            Time.timeScale = 0f;

            if (grid != null)
            {
                yield return StartCoroutine(
                    grid.PlayBoardClearAnimation(
                        resultRowDelay,
                        resultDisappearDuration));
            }

            if (gameUI != null)
                gameUI.ShowLosePanel();

            Time.timeScale = 0f;
        }

        // =====================================================
        // LEVEL UNLOCK
        // =====================================================

        public static int GetUnlockedLevel()
        {
            return PlayerPrefs.GetInt(
                UNLOCKED_LEVEL_KEY,
                1);
        }

        public static bool IsLevelUnlocked(
            int levelNumber)
        {
            return levelNumber <=
                   GetUnlockedLevel();
        }

        public static void UnlockLevel(
            int levelNumber)
        {
            int currentUnlocked =
                GetUnlockedLevel();

            if (levelNumber <= currentUnlocked)
                return;

            PlayerPrefs.SetInt(
                UNLOCKED_LEVEL_KEY,
                levelNumber);

            PlayerPrefs.Save();

            Debug.Log(
                "Unlocked Level: " +
                levelNumber);
        }

        private void UnlockNextLevel()
        {
            int currentLevel =
                GetCurrentLevelNumber();

            if (currentLevel <= 0)
                return;

            int nextLevel =
                currentLevel + 1;

            UnlockLevel(nextLevel);
        }

        private int GetCurrentLevelNumber()
        {
            string sceneName =
                SceneManager.GetActiveScene().name;

            int number = 0;

            string digits = "";

            for (int i = 0;
                 i < sceneName.Length;
                 i++)
            {
                if (char.IsDigit(sceneName[i]))
                {
                    digits += sceneName[i];
                }
            }

            if (!string.IsNullOrEmpty(digits))
            {
                int.TryParse(
                    digits,
                    out number);
            }

            return number;
        }

        public static void ResetLevelProgress()
        {
            PlayerPrefs.DeleteKey(
                UNLOCKED_LEVEL_KEY);

            PlayerPrefs.Save();

            Debug.Log(
                "Level progress reset.");
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene()
                    .buildIndex);
        }
    }
}