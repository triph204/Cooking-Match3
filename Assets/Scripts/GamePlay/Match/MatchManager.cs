using Match3.Gameplay.Audio;
using Match3.Gameplay.Board;
using Match3.Gameplay.Game;
using Match3.Gameplay.Gravity;
using Match3.Gameplay.Spawn;
using Match3.Gameplay.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Gameplay.Match
{
    public class MatchManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ComboUI comboUI;

        [Header("References")]
        [SerializeField] private GridManager grid;
        [SerializeField] private MatchDetector detector;
        [SerializeField] private GravityManager gravity;
        [SerializeField] private GemSpawnManager spawner;
        [SerializeField] private GameManager gameManager;

        [Header("Destroy")]
        [SerializeField] private float destroyDelay = 0.05f;
        [SerializeField] private float destroyAnimationDuration = 0.2f;

        [Header("Score")]
        [SerializeField] private int scorePerGem = 10;

        [Header("Combo")]
        [SerializeField] private bool enableCombo = true;
        [SerializeField] private int maxComboMultiplier = 10;

        public bool IsProcessing { get; private set; }
        public int CurrentCombo { get; private set; }

        private void Awake()
        {
            if (grid == null)
                grid = FindFirstObjectByType<GridManager>();

            if (detector == null)
                detector = FindFirstObjectByType<MatchDetector>();

            if (gravity == null)
                gravity = FindFirstObjectByType<GravityManager>();

            if (spawner == null)
                spawner = FindFirstObjectByType<GemSpawnManager>();

            if (gameManager == null)
                gameManager = FindFirstObjectByType<GameManager>();

            if (comboUI == null)
                comboUI = FindFirstObjectByType<ComboUI>();
        }

        public IEnumerator ProcessAfterSwap()
        {
            if (IsProcessing)
                yield break;

            if (gameManager != null &&
                gameManager.IsGameOver)
                yield break;

            IsProcessing = true;
            CurrentCombo = 0;

            yield return StartCoroutine(
     ProcessMatches());

            ResetCombo();

            yield return null;

            if (gameManager != null)
            {
                if (gameManager.WinPending)
                {
                    yield return StartCoroutine(
                        gameManager.PlayWinSequence());

                    IsProcessing = false;
                    yield break;
                }

                MoveChecker moveChecker =
                    FindFirstObjectByType<MoveChecker>();

                if (moveChecker != null &&
                    !moveChecker.HasPossibleMove())
                {
                    gameManager.LoseGame();

                    yield return StartCoroutine(
                        gameManager.PlayLoseSequence());

                    IsProcessing = false;
                    yield break;
                }
            }

            IsProcessing = false;
        }

        private IEnumerator ProcessMatches()
        {
            while (true)
            {
                if (detector == null)
                    yield break;

                List<GemTile> matches =
                    detector.FindAllMatches();

                if (matches == null ||
                    matches.Count < 3)
                    yield break;

                List<GemTile> gems =
                    GetUniqueGems(matches);

                if (gems.Count < 3)
                    yield break;

                IncreaseCombo();

                AddMatchScore(gems.Count);
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.gemMatchSound);
                }
                ClearGemCells(gems);

                if (destroyDelay > 0f)
                {
                    yield return new WaitForSeconds(
                        destroyDelay);
                }

                yield return StartCoroutine(
                    PlayDestroyAnimation(gems));

                DestroyMatchedGems(gems);

                yield return null;

                if (gravity != null)
                {
                    yield return StartCoroutine(
                        gravity.Collapse());
                }

                if (spawner != null)
                {
                    yield return StartCoroutine(
                        spawner.FillEmptyCells());
                }

                yield return null;
            }
        }

        private List<GemTile> GetUniqueGems(
            List<GemTile> matches)
        {
            List<GemTile> result =
                new List<GemTile>();

            if (matches == null)
                return result;

            foreach (GemTile gem in matches)
            {
                if (gem == null)
                    continue;

                if (result.Contains(gem))
                    continue;

                result.Add(gem);
            }

            return result;
        }

        private void ClearGemCells(
            List<GemTile> gems)
        {
            if (grid == null)
                return;

            foreach (GemTile gem in gems)
            {
                if (gem == null)
                    continue;

                Vector2Int pos =
                    gem.GridPosition;

                BoardCell cell =
                    grid.GetCell(
                        pos.x,
                        pos.y);

                if (cell == null)
                    continue;

                if (cell.CurrentGem != gem)
                    continue;

                grid.ClearCell(
                    pos.x,
                    pos.y);
            }
        }

        private IEnumerator PlayDestroyAnimation(
            List<GemTile> gems)
        {
            if (gems == null ||
                gems.Count == 0)
                yield break;

            float duration =
                Mathf.Max(
                    destroyAnimationDuration,
                    0.01f);

            float time = 0f;

            List<Vector3> startScales =
                new List<Vector3>();

            foreach (GemTile gem in gems)
            {
                if (gem != null)
                {
                    startScales.Add(
                        gem.transform.localScale);
                }
                else
                {
                    startScales.Add(
                        Vector3.zero);
                }
            }

            while (time < duration)
            {
                time += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        time / duration);

                t =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        t);

                for (int i = 0;
                     i < gems.Count;
                     i++)
                {
                    GemTile gem =
                        gems[i];

                    if (gem == null)
                        continue;

                    gem.transform.localScale =
                        Vector3.Lerp(
                            startScales[i],
                            Vector3.zero,
                            t);
                }

                yield return null;
            }

            foreach (GemTile gem in gems)
            {
                if (gem == null)
                    continue;

                gem.transform.localScale =
                    Vector3.zero;
            }
        }

        private void DestroyMatchedGems(
            List<GemTile> gems)
        {
            if (gems == null)
                return;

            foreach (GemTile gem in gems)
            {
                if (gem == null)
                    continue;

                Destroy(
                    gem.gameObject);
            }
        }

        private void IncreaseCombo()
        {
            if (!enableCombo)
            {
                CurrentCombo = 1;
                return;
            }

            CurrentCombo++;

            if (CurrentCombo >
                maxComboMultiplier)
            {
                CurrentCombo =
                    maxComboMultiplier;
            }

            if (comboUI != null &&
     CurrentCombo >= 2)
            {
                comboUI.ShowCombo(CurrentCombo);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.comboDingSound);
                }
            }
        }

        private int GetComboMultiplier()
        {
            if (!enableCombo)
                return 1;

            return Mathf.Max(
                CurrentCombo,
                1);
        }

        private void ResetCombo()
        {
            CurrentCombo = 0;
        }

        private void AddMatchScore(
            int gemCount)
        {
            if (gameManager == null)
                return;

            int comboMultiplier =
                GetComboMultiplier();

            int score =
                gemCount *
                scorePerGem *
                comboMultiplier;

            gameManager.AddRawScore(score);
        }
    }
}