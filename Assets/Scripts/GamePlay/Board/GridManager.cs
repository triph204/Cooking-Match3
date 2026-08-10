using Match3.Data.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Gameplay.Board
{
    public class GridManager : MonoBehaviour
    {
        [Header("Board Size")]
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;

        [Header("Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Padding")]
        [SerializeField] private float horizontalPadding = 0.6f;
        [SerializeField] private float verticalPadding = 1.2f;

        [Header("Cell")]
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform cellRoot;

        [Header("Cell Size")]
        [SerializeField] private float cellWidthRatio = 1.0f;
        [SerializeField] private float cellHeightRatio = 1.0f;

        [Header("Gem")]
        [SerializeField] private Transform boardRoot;
        [SerializeField] private GemConfig[] gemConfigs;

        [Header("Initial Board")]
        [SerializeField] private int maxGenerationAttempts = 1000;

        [Header("Result Animation")]
        [SerializeField] private float resultRowDelay = 0.15f;
        [SerializeField] private float resultDisappearDuration = 0.3f;

        private BoardCell[,] board;

        private float cellSize;
        private float spacing;
        private float step;

        private Vector3 boardOrigin;

        public int Width => width;
        public int Height => height;
        public float Step => step;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            board = new BoardCell[width, height];

            CalculateBoardLayout();
        }

        private void Start()
        {
            GenerateBoard();
        }

        private void CalculateBoardLayout()
        {
            if (mainCamera == null)
            {
                Debug.LogError(
                    "GridManager: Không tìm thấy Main Camera."
                );

                return;
            }

            float worldHeight =
                mainCamera.orthographicSize * 2f;

            float worldWidth =
                worldHeight * mainCamera.aspect;

            float usableWidth =
                worldWidth -
                horizontalPadding * 2f;

            float usableHeight =
                worldHeight -
                verticalPadding * 2f;

            step = Mathf.Min(
                usableWidth / width,
                usableHeight / height
            );

            spacing =
                step * 0.12f;

            cellSize =
                step - spacing;

            float boardWidth =
                width * step;

            float boardHeight =
                height * step;

            boardOrigin =
                new Vector3(
                    -boardWidth * 0.5f +
                    step * 0.5f,

                    boardHeight * 0.5f -
                    step * 0.5f,

                    0f
                );
        }

        public Vector3 GetWorldPosition(
            int x,
            int y)
        {
            return boardOrigin +
                   new Vector3(
                       x * step,
                       -y * step,
                       0f
                   );
        }

        // =========================================================
        // GENERATE BOARD
        // =========================================================

        private void GenerateBoard()
        {
            if (!ValidateGemConfigs())
                return;

            GemConfig[,] layout;

            bool valid =
                TryGenerateValidLayout(
                    out layout
                );

            if (!valid)
            {
                Debug.LogError(
                    "GridManager: Không thể tạo board hợp lệ."
                );

                return;
            }

            ClearBoard();

            board =
                new BoardCell[
                    width,
                    height
                ];

            CreateBoardFromLayout(layout);

            Debug.Log(
                "GridManager: Board generated successfully."
            );
        }

        private bool ValidateGemConfigs()
        {
            if (gemConfigs == null ||
                gemConfigs.Length == 0)
            {
                Debug.LogError(
                    "GridManager: GemConfigs chưa được gán."
                );

                return false;
            }

            for (int i = 0;
                 i < gemConfigs.Length;
                 i++)
            {
                if (gemConfigs[i] == null)
                {
                    Debug.LogError(
                        "GridManager: GemConfig tại index " +
                        i +
                        " đang null."
                    );

                    return false;
                }

                if (gemConfigs[i].prefab == null)
                {
                    Debug.LogError(
                        "GridManager: GemConfig " +
                        gemConfigs[i].name +
                        " chưa có Prefab."
                    );

                    return false;
                }
            }

            return true;
        }

        private bool TryGenerateValidLayout(
            out GemConfig[,] layout)
        {
            layout = null;

            for (int attempt = 0;
                 attempt < maxGenerationAttempts;
                 attempt++)
            {
                GemConfig[,] candidate =
                    GenerateCandidateLayout();

                if (candidate == null)
                    continue;

                if (HasAnyMatch(candidate))
                    continue;

                if (!HasPossibleMove(candidate))
                    continue;

                layout = candidate;

                return true;
            }

            return false;
        }

        private GemConfig[,] GenerateCandidateLayout()
        {
            GemConfig[,] result =
                new GemConfig[
                    width,
                    height
                ];

            for (int y = 0;
                 y < height;
                 y++)
            {
                for (int x = 0;
                     x < width;
                     x++)
                {
                    GemConfig config =
                        GetSafeConfig(
                            result,
                            x,
                            y
                        );

                    if (config == null)
                        return null;

                    result[x, y] = config;
                }
            }

            return result;
        }

        private GemConfig GetSafeConfig(
            GemConfig[,] layout,
            int x,
            int y)
        {
            List<GemConfig> candidates =
                new List<GemConfig>(
                    gemConfigs
                );

            ShuffleList(candidates);

            foreach (GemConfig config in candidates)
            {
                if (WouldCreateHorizontalMatch(
                        layout,
                        x,
                        y,
                        config))
                {
                    continue;
                }

                if (WouldCreateVerticalMatch(
                        layout,
                        x,
                        y,
                        config))
                {
                    continue;
                }

                return config;
            }

            return null;
        }

        private bool WouldCreateHorizontalMatch(
            GemConfig[,] layout,
            int x,
            int y,
            GemConfig config)
        {
            if (x < 2)
                return false;

            GemConfig left1 =
                layout[x - 1, y];

            GemConfig left2 =
                layout[x - 2, y];

            if (left1 == null ||
                left2 == null)
            {
                return false;
            }

            return IsSameConfig(
                       left1,
                       config
                   )
                   &&
                   IsSameConfig(
                       left2,
                       config
                   );
        }

        private bool WouldCreateVerticalMatch(
            GemConfig[,] layout,
            int x,
            int y,
            GemConfig config)
        {
            if (y < 2)
                return false;

            GemConfig above1 =
                layout[x, y - 1];

            GemConfig above2 =
                layout[x, y - 2];

            if (above1 == null ||
                above2 == null)
            {
                return false;
            }

            return IsSameConfig(
                       above1,
                       config
                   )
                   &&
                   IsSameConfig(
                       above2,
                       config
                   );
        }

        private bool HasAnyMatch(
            GemConfig[,] layout)
        {
            for (int y = 0;
                 y < height;
                 y++)
            {
                for (int x = 0;
                     x < width;
                     x++)
                {
                    GemConfig current =
                        layout[x, y];

                    if (current == null)
                        continue;

                    if (x + 2 < width)
                    {
                        if (IsSameConfig(
                                current,
                                layout[x + 1, y]
                            )
                            &&
                            IsSameConfig(
                                current,
                                layout[x + 2, y]
                            ))
                        {
                            return true;
                        }
                    }

                    if (y + 2 < height)
                    {
                        if (IsSameConfig(
                                current,
                                layout[x, y + 1]
                            )
                            &&
                            IsSameConfig(
                                current,
                                layout[x, y + 2]
                            ))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool HasPossibleMove(
            GemConfig[,] layout)
        {
            for (int y = 0;
                 y < height;
                 y++)
            {
                for (int x = 0;
                     x < width;
                     x++)
                {
                    if (x + 1 < width)
                    {
                        if (WouldSwapCreateMatch(
                                layout,
                                x,
                                y,
                                x + 1,
                                y))
                        {
                            return true;
                        }
                    }

                    if (y + 1 < height)
                    {
                        if (WouldSwapCreateMatch(
                                layout,
                                x,
                                y,
                                x,
                                y + 1))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool WouldSwapCreateMatch(
            GemConfig[,] layout,
            int x1,
            int y1,
            int x2,
            int y2)
        {
            GemConfig first =
                layout[x1, y1];

            GemConfig second =
                layout[x2, y2];

            if (first == null ||
                second == null)
            {
                return false;
            }

            if (IsSameConfig(
                    first,
                    second))
            {
                return false;
            }

            layout[x1, y1] = second;
            layout[x2, y2] = first;

            bool result =
                HasMatchAt(
                    layout,
                    x1,
                    y1
                )
                ||
                HasMatchAt(
                    layout,
                    x2,
                    y2
                );

            layout[x1, y1] = first;
            layout[x2, y2] = second;

            return result;
        }

        private bool HasMatchAt(
            GemConfig[,] layout,
            int x,
            int y)
        {
            GemConfig center =
                layout[x, y];

            if (center == null)
                return false;

            int horizontal = 1;

            horizontal += CountDirection(
                layout,
                x,
                y,
                -1,
                0,
                center
            );

            horizontal += CountDirection(
                layout,
                x,
                y,
                1,
                0,
                center
            );

            if (horizontal >= 3)
                return true;

            int vertical = 1;

            vertical += CountDirection(
                layout,
                x,
                y,
                0,
                -1,
                center
            );

            vertical += CountDirection(
                layout,
                x,
                y,
                0,
                1,
                center
            );

            return vertical >= 3;
        }

        private int CountDirection(
            GemConfig[,] layout,
            int startX,
            int startY,
            int dx,
            int dy,
            GemConfig config)
        {
            int count = 0;

            int x =
                startX + dx;

            int y =
                startY + dy;

            while (x >= 0 &&
                   x < width &&
                   y >= 0 &&
                   y < height)
            {
                GemConfig current =
                    layout[x, y];

                if (current == null)
                    break;

                if (!IsSameConfig(
                        current,
                        config))
                {
                    break;
                }

                count++;

                x += dx;
                y += dy;
            }

            return count;
        }

        // =========================================================
        // CREATE BOARD
        // =========================================================

        private void CreateBoardFromLayout(
            GemConfig[,] layout)
        {
            for (int y = 0;
                 y < height;
                 y++)
            {
                for (int x = 0;
                     x < width;
                     x++)
                {
                    CreateCell(
                        x,
                        y,
                        layout[x, y]
                    );
                }
            }
        }

        private void CreateCell(
            int x,
            int y,
            GemConfig config)
        {
            if (config == null)
                return;

            Vector3 pos =
                GetWorldPosition(
                    x,
                    y
                );

            if (cellPrefab != null)
            {
                GameObject cell =
                    Instantiate(
                        cellPrefab,
                        pos,
                        Quaternion.identity,
                        cellRoot
                    );

                SetCellSize(
                    cell.transform
                );
            }

            GemTile gem =
                Instantiate(
                    config.prefab,
                    pos,
                    Quaternion.identity,
                    boardRoot
                );

            FitObjectToSize(
                gem.transform,
                cellSize * 0.78f
            );

            gem.Initialize(
                config,
                new Vector2Int(
                    x,
                    y
                )
            );

            BoardCell boardCell =
                new BoardCell(
                    x,
                    y
                );

            boardCell.SetGem(gem);

            board[x, y] =
                boardCell;
        }

        // =========================================================
        // CREATE GEM
        // =========================================================

        public GemTile CreateGem(
            int x,
            int y,
            GemConfig config)
        {
            if (!IsInside(x, y))
                return null;

            if (config == null)
                return null;

            BoardCell cell =
                board[x, y];

            if (cell == null)
            {
                cell =
                    new BoardCell(
                        x,
                        y
                    );

                board[x, y] =
                    cell;
            }

            Vector3 position =
                GetWorldPosition(
                    x,
                    y
                );

            GemTile gem =
                Instantiate(
                    config.prefab,
                    position,
                    Quaternion.identity,
                    boardRoot
                );

            FitObjectToSize(
                gem.transform,
                cellSize * 0.78f
            );

            gem.Initialize(
                config,
                new Vector2Int(
                    x,
                    y
                )
            );

            cell.SetGem(gem);

            return gem;
        }

        // =========================================================
        // RANDOM CONFIG
        // =========================================================

        public GemConfig GetRandomConfig()
        {
            if (gemConfigs == null ||
                gemConfigs.Length == 0)
            {
                Debug.LogError(
                    "GridManager: GemConfigs chưa được gán!"
                );

                return null;
            }

            return gemConfigs[
                Random.Range(
                    0,
                    gemConfigs.Length
                )
            ];
        }

        // =========================================================
        // REMOVE GEM
        // =========================================================

        public void RemoveGem(
            int x,
            int y)
        {
            if (!IsInside(x, y))
                return;

            BoardCell cell =
                board[x, y];

            if (cell == null)
                return;

            GemTile gem =
                cell.CurrentGem;

            cell.Clear();

            if (gem != null)
            {
                Destroy(
                    gem.gameObject
                );
            }
        }

        // =========================================================
        // CELL SIZE
        // =========================================================

        private void SetCellSize(
            Transform target)
        {
            if (target == null)
                return;

            SpriteRenderer sr =
                target.GetComponent<SpriteRenderer>();

            if (sr == null ||
                sr.sprite == null)
            {
                return;
            }

            Vector2 spriteSize =
                sr.sprite.bounds.size;

            if (spriteSize.x <= 0f ||
                spriteSize.y <= 0f)
            {
                return;
            }

            float targetWidth =
                cellSize *
                cellWidthRatio;

            float targetHeight =
                cellSize *
                cellHeightRatio;

            float scaleX =
                targetWidth /
                spriteSize.x;

            float scaleY =
                targetHeight /
                spriteSize.y;

            target.localScale =
                new Vector3(
                    scaleX,
                    scaleY,
                    1f
                );
        }

        // =========================================================
        // FIT OBJECT
        // =========================================================

        private void FitObjectToSize(
            Transform target,
            float size)
        {
            if (target == null)
                return;

            SpriteRenderer sr =
                target.GetComponent<SpriteRenderer>();

            if (sr == null ||
                sr.sprite == null)
            {
                return;
            }

            Vector2 spriteSize =
                sr.sprite.bounds.size;

            if (spriteSize.x <= 0f ||
                spriteSize.y <= 0f)
            {
                return;
            }

            float scale =
                Mathf.Min(
                    size / spriteSize.x,
                    size / spriteSize.y
                );

            target.localScale =
                Vector3.one *
                scale;
        }

        // =========================================================
        // COMPARE
        // =========================================================

        private bool IsSameConfig(
            GemConfig a,
            GemConfig b)
        {
            if (a == null ||
                b == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(a.id) &&
                !string.IsNullOrEmpty(b.id))
            {
                return a.id == b.id;
            }

            return a.icon == b.icon;
        }

        // =========================================================
        // SHUFFLE
        // =========================================================

        private void ShuffleList<T>(
            List<T> list)
        {
            for (int i = 0;
                 i < list.Count;
                 i++)
            {
                int randomIndex =
                    Random.Range(
                        i,
                        list.Count
                    );

                T temp =
                    list[i];

                list[i] =
                    list[randomIndex];

                list[randomIndex] =
                    temp;
            }
        }

        // =========================================================
        // CLEAR BOARD
        // =========================================================

        private void ClearBoard()
        {
            if (boardRoot != null)
            {
                for (int i =
                         boardRoot.childCount - 1;
                     i >= 0;
                     i--)
                {
                    Destroy(
                        boardRoot
                            .GetChild(i)
                            .gameObject
                    );
                }
            }

            if (cellRoot != null)
            {
                for (int i =
                         cellRoot.childCount - 1;
                     i >= 0;
                     i--)
                {
                    Destroy(
                        cellRoot
                            .GetChild(i)
                            .gameObject
                    );
                }
            }
        }

        // =========================================================
        // INSIDE
        // =========================================================

        public bool IsInside(
            int x,
            int y)
        {
            return x >= 0 &&
                   x < width &&
                   y >= 0 &&
                   y < height;
        }

        // =========================================================
        // GET CELL
        // =========================================================

        public BoardCell GetCell(
            int x,
            int y)
        {
            if (!IsInside(x, y))
                return null;

            return board[x, y];
        }

        // =========================================================
        // GET GEM
        // =========================================================

        public GemTile GetGem(
            int x,
            int y)
        {
            BoardCell cell =
                GetCell(x, y);

            if (cell == null)
                return null;

            return cell.CurrentGem;
        }

        // =========================================================
        // SET GEM
        // =========================================================

        public void SetGem(
            int x,
            int y,
            GemTile gem)
        {
            if (!IsInside(x, y))
                return;

            if (board[x, y] == null)
            {
                board[x, y] =
                    new BoardCell(
                        x,
                        y
                    );
            }

            board[x, y]
                .SetGem(gem);

            if (gem != null)
            {
                gem.SetGridPosition(
                    new Vector2Int(
                        x,
                        y
                    )
                );

                gem.transform.position =
                    GetWorldPosition(
                        x,
                        y
                    );
            }
        }

        // =========================================================
        // CLEAR CELL
        // =========================================================

        public void ClearCell(
            int x,
            int y)
        {
            if (!IsInside(x, y))
                return;

            if (board[x, y] == null)
                return;

            board[x, y].Clear();
        }

        // =========================================================
        // SWAP
        // =========================================================

        public void Swap(
            BoardCell first,
            BoardCell second)
        {
            if (first == null ||
                second == null)
            {
                return;
            }

            GemTile firstGem =
                first.CurrentGem;

            GemTile secondGem =
                second.CurrentGem;

            first.SetGem(secondGem);
            second.SetGem(firstGem);

            if (firstGem != null)
            {
                firstGem.SetGridPosition(
                    second.Position
                );

                firstGem.transform.position =
                    GetWorldPosition(
                        second.Position.x,
                        second.Position.y
                    );
            }

            if (secondGem != null)
            {
                secondGem.SetGridPosition(
                    first.Position
                );

                secondGem.transform.position =
                    GetWorldPosition(
                        first.Position.x,
                        first.Position.y
                    );
            }
        }

        // =========================================================
        // NEIGHBOUR
        // =========================================================

        public bool IsNeighbour(
            BoardCell a,
            BoardCell b)
        {
            if (a == null ||
                b == null)
            {
                return false;
            }

            int dx =
                Mathf.Abs(
                    a.Position.x -
                    b.Position.x
                );

            int dy =
                Mathf.Abs(
                    a.Position.y -
                    b.Position.y
                );

            return dx + dy == 1;
        }

        // =========================================================
        // GET BOARD
        // =========================================================

        public BoardCell[,] GetBoard()
        {
            return board;
        }

        public Vector2Int WorldToGrid(
            Vector3 worldPosition)
        {
            Vector3 local =
                worldPosition -
                boardOrigin;

            int x =
                Mathf.RoundToInt(
                    local.x / step
                );

            int y =
                Mathf.RoundToInt(
                    -local.y / step
                );

            return new Vector2Int(
                x,
                y
            );
        }

        public IEnumerator PlayBoardClearAnimation(
    float rowDelay,
    float duration)
        {
            List<List<GemTile>> rows =
                new List<List<GemTile>>();

            for (int y = 0; y < height; y++)
            {
                List<GemTile> row =
                    new List<GemTile>();

                for (int x = 0; x < width; x++)
                {
                    GemTile gem =
                        GetGem(x, y);

                    if (gem != null)
                        row.Add(gem);
                }

                if (row.Count > 0)
                    rows.Add(row);
            }

            if (rows.Count == 0)
                yield break;

            for (int i = 0; i < rows.Count; i++)
            {
                foreach (GemTile gem in rows[i])
                {
                    if (gem != null)
                        gem.StartResultDisappear(duration);
                }

                if (i < rows.Count - 1)
                {
                    yield return new WaitForSecondsRealtime(
                        rowDelay);
                }
            }

            float totalDuration =
                duration +
                rowDelay * (rows.Count - 1);

            yield return new WaitForSecondsRealtime(
                totalDuration);
        }

        public void Regenerate()
        {
            CalculateBoardLayout();
            GenerateBoard();
        }
    }
}