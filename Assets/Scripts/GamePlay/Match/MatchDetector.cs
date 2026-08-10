using System.Collections.Generic;
using Match3.Gameplay.Board;
using UnityEngine;

namespace Match3.Gameplay.Match
{
    public class MatchDetector : MonoBehaviour
    {
        [SerializeField] private GridManager grid;

        private void Awake()
        {
            if (grid == null)
                grid = FindFirstObjectByType<GridManager>();
        }

        public List<GemTile> FindAllMatches()
        {
            List<GemTile> matches =
                new List<GemTile>();

            if (grid == null)
                return matches;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    AddMatch(
                        GetHorizontalRun(x, y),
                        matches);

                    AddMatch(
                        GetVerticalRun(x, y),
                        matches);
                }
            }

            return matches;
        }

        public List<GemTile> FindMatchesAt(
            Vector2Int position)
        {
            List<GemTile> matches =
                new List<GemTile>();

            if (grid == null)
                return matches;

            AddMatch(
                GetHorizontalRun(
                    position.x,
                    position.y),
                matches);

            AddMatch(
                GetVerticalRun(
                    position.x,
                    position.y),
                matches);

            return matches;
        }

        public bool HasMatchAt(Vector2Int position)
        {
            return FindMatchesAt(position).Count >= 3;
        }

        private List<GemTile> GetHorizontalRun(
            int x,
            int y)
        {
            List<GemTile> result =
                new List<GemTile>();

            GemTile center =
                grid.GetGem(x, y);

            if (center == null)
                return result;

            int left = x;

            while (left > 0 &&
                   SameGem(
                       center,
                       grid.GetGem(left - 1, y)))
            {
                left--;
            }

            int right = x;

            while (right < grid.Width - 1 &&
                   SameGem(
                       center,
                       grid.GetGem(right + 1, y)))
            {
                right++;
            }

            for (int i = left; i <= right; i++)
            {
                GemTile gem =
                    grid.GetGem(i, y);

                if (gem != null)
                    result.Add(gem);
            }

            return result;
        }

        private List<GemTile> GetVerticalRun(
            int x,
            int y)
        {
            List<GemTile> result =
                new List<GemTile>();

            GemTile center =
                grid.GetGem(x, y);

            if (center == null)
                return result;

            int bottom = y;

            while (bottom > 0 &&
                   SameGem(
                       center,
                       grid.GetGem(x, bottom - 1)))
            {
                bottom--;
            }

            int top = y;

            while (top < grid.Height - 1 &&
                   SameGem(
                       center,
                       grid.GetGem(x, top + 1)))
            {
                top++;
            }

            for (int i = bottom; i <= top; i++)
            {
                GemTile gem =
                    grid.GetGem(x, i);

                if (gem != null)
                    result.Add(gem);
            }

            return result;
        }

        private void AddMatch(
            List<GemTile> run,
            List<GemTile> result)
        {
            if (run.Count < 3)
                return;

            foreach (GemTile gem in run)
            {
                if (gem != null &&
                    !result.Contains(gem))
                {
                    result.Add(gem);
                }
            }
        }

        private bool SameGem(
            GemTile a,
            GemTile b)
        {
            if (a == null || b == null)
                return false;

            if (a.Config == null ||
                b.Config == null)
                return false;

            // Ưu tiên ID.
            if (!string.IsNullOrEmpty(a.Config.id) &&
                !string.IsNullOrEmpty(b.Config.id))
            {
                return a.Config.id ==
                       b.Config.id;
            }

            // Nếu ID chưa có thì so icon.
            return a.Config.icon ==
                   b.Config.icon;
        }
    }
}