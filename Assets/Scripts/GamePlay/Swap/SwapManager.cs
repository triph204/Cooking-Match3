using System.Collections;
using Match3.Gameplay.Board;
using Match3.Gameplay.Match;
using UnityEngine;

namespace Match3.Gameplay.Swap
{
    public class SwapManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridManager grid;
        [SerializeField] private MatchDetector detector;
        [SerializeField] private MatchManager matchManager;

        [Header("Swap")]
        [SerializeField] private float swapDuration = 0.18f;

        private GemTile selectedGem;

        public bool IsBusy { get; private set; }

        private void Awake()
        {
            if (grid == null)
                grid =
                    FindFirstObjectByType<GridManager>();

            if (detector == null)
                detector =
                    FindFirstObjectByType<MatchDetector>();

            if (matchManager == null)
                matchManager =
                    FindFirstObjectByType<MatchManager>();
        }

        // =====================================================
        // SELECT
        // =====================================================

        public void SelectGem(BoardCell cell)
        {
            if (IsBusy)
                return;

            if (cell == null)
                return;

            GemTile clickedGem =
                cell.CurrentGem;

            if (clickedGem == null)
                return;

    

            if (selectedGem == null)
            {
                selectedGem = clickedGem;

                selectedGem.SetSelected(true);

                Debug.Log(
                    "Selected: " +
                    selectedGem.GridPosition);

                return;
            }

           
            if (selectedGem == clickedGem)
            {
                selectedGem.SetSelected(false);

                selectedGem = null;

                Debug.Log("Selection cleared.");

                return;
            }

        
            GemTile secondGem =
                clickedGem;

            BoardCell firstCell =
                grid.GetCell(
                    selectedGem.GridPosition.x,
                    selectedGem.GridPosition.y);

            BoardCell secondCell =
                grid.GetCell(
                    secondGem.GridPosition.x,
                    secondGem.GridPosition.y);

            if (firstCell == null ||
                secondCell == null)
            {
                selectedGem.SetSelected(false);
                selectedGem = null;

                return;
            }


            if (!grid.IsNeighbour(
                    firstCell,
                    secondCell))
            {
                // Tắt gem cũ
                selectedGem.SetSelected(false);

                // Chọn gem mới
                selectedGem = secondGem;

                selectedGem.SetSelected(true);

                Debug.Log(
                    "New Selected: " +
                    selectedGem.GridPosition);

                return;
            }

      

            GemTile firstGem =
                selectedGem;

            // Tắt select trước khi swap
            firstGem.SetSelected(false);

            selectedGem = null;

            TrySwap(
                firstGem,
                secondGem);
        }


        public void TrySwap(
            GemTile first,
            GemTile second)
        {
            if (IsBusy)
                return;

            if (first == null ||
                second == null)
                return;

            if (grid == null)
                return;

            BoardCell firstCell =
                grid.GetCell(
                    first.GridPosition.x,
                    first.GridPosition.y);

            BoardCell secondCell =
                grid.GetCell(
                    second.GridPosition.x,
                    second.GridPosition.y);

            if (firstCell == null ||
                secondCell == null)
                return;

            TrySwap(
                firstCell,
                secondCell);
        }

        // =====================================================
        // TRY SWAP - BOARDCELL
        // =====================================================

        public void TrySwap(
            BoardCell first,
            BoardCell second)
        {
            if (IsBusy)
                return;

            if (first == null ||
                second == null)
                return;

            if (first.CurrentGem == null ||
                second.CurrentGem == null)
                return;

            if (!grid.IsNeighbour(
                    first,
                    second))
                return;

            StartCoroutine(
                SwapRoutine(
                    first,
                    second));
        }

        // =====================================================
        // SWAP ROUTINE
        // =====================================================

        private IEnumerator SwapRoutine(
            BoardCell firstCell,
            BoardCell secondCell)
        {
            IsBusy = true;

            GemTile firstGem =
                firstCell.CurrentGem;

            GemTile secondGem =
                secondCell.CurrentGem;

            if (firstGem == null ||
                secondGem == null)
            {
                IsBusy = false;
                yield break;
            }

            Vector3 firstStart =
                firstGem.transform.position;

            Vector3 secondStart =
                secondGem.transform.position;

            // -------------------------------------------------
            // 1. Animation
            // -------------------------------------------------

            yield return StartCoroutine(
                MoveGems(
                    firstGem,
                    secondGem,
                    firstStart,
                    secondStart));

            // -------------------------------------------------
            // 2. Update Board
            // -------------------------------------------------

            grid.Swap(
                firstCell,
                secondCell);

            // -------------------------------------------------
            // 3. Check Match
            // -------------------------------------------------

            bool firstMatch =
                detector != null &&
                detector.HasMatchAt(
                    firstCell.Position);

            bool secondMatch =
                detector != null &&
                detector.HasMatchAt(
                    secondCell.Position);

            bool hasMatch =
                firstMatch ||
                secondMatch;

            // -------------------------------------------------
            // 4. Không match
            // -------------------------------------------------

            if (!hasMatch)
            {
                Debug.Log(
                    "NO MATCH -> SWAP BACK");

                yield return StartCoroutine(
                    MoveGems(
                        firstGem,
                        secondGem,
                        secondStart,
                        firstStart));

                // Đưa board về trạng thái ban đầu
                grid.Swap(
                    firstCell,
                    secondCell);

                IsBusy = false;

                yield break;
            }

            // -------------------------------------------------
            // 5. Có match
            // -------------------------------------------------

            Debug.Log(
                "MATCH FOUND -> CLEAR");

            if (matchManager != null)
            {
                yield return StartCoroutine(
                    matchManager.ProcessAfterSwap());
            }

            IsBusy = false;
        }

        // =====================================================
        // MOVE GEMS
        // =====================================================

        private IEnumerator MoveGems(
            GemTile first,
            GemTile second,
            Vector3 firstPosition,
            Vector3 secondPosition)
        {
            float elapsed = 0f;

            while (elapsed < swapDuration)
            {
                elapsed += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed /
                        swapDuration);

                t =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        t);

                first.transform.position =
                    Vector3.Lerp(
                        firstPosition,
                        secondPosition,
                        t);

                second.transform.position =
                    Vector3.Lerp(
                        secondPosition,
                        firstPosition,
                        t);

                yield return null;
            }

            first.transform.position =
                secondPosition;

            second.transform.position =
                firstPosition;
        }
    }
}