using Match3.Gameplay.Board;
using UnityEngine;

namespace Match3.Gameplay.Match
{
    public class MoveChecker : MonoBehaviour
    {
        [SerializeField]
        private GridManager grid;

        private void Awake()
        {
            if (grid == null)
                grid = FindFirstObjectByType<GridManager>();
        }

        // =====================================================
        // KIỂM TRA BOARD CÒN NƯỚC ĐI KHÔNG
        // =====================================================

        public bool HasPossibleMove()
        {
            if (grid == null)
                return false;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    // Thử đổi với ô bên phải
                    if (x + 1 < grid.Width)
                    {
                        if (WouldCreateMatch(
                            x,
                            y,
                            x + 1,
                            y))
                        {
                            return true;
                        }
                    }

                    // Thử đổi với ô bên dưới
                    if (y + 1 < grid.Height)
                    {
                        if (WouldCreateMatch(
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

        // =====================================================
        // GIẢ LẬP SWAP
        // =====================================================

        private bool WouldCreateMatch(
            int x1,
            int y1,
            int x2,
            int y2)
        {
            GemTile first =
                grid.GetGem(x1, y1);

            GemTile second =
                grid.GetGem(x2, y2);

            if (first == null ||
                second == null)
            {
                return false;
            }

            // Hai gem giống nhau swap cũng không tạo
            // ra match mới
            if (first.Config == second.Config)
                return false;

            // Đổi tạm trong BoardCell
            BoardCell cellA =
                grid.GetCell(x1, y1);

            BoardCell cellB =
                grid.GetCell(x2, y2);

            cellA.SetGem(second);
            cellB.SetGem(first);

            bool result =
                HasMatchAt(x1, y1) ||
                HasMatchAt(x2, y2);

            // Đổi lại như cũ
            cellA.SetGem(first);
            cellB.SetGem(second);

            return result;
        }

        // =====================================================
        // KIỂM TRA MATCH TẠI 1 Ô
        // =====================================================

        private bool HasMatchAt(int x, int y)
        {
            GemTile center =
                grid.GetGem(x, y);

            if (center == null)
                return false;

            GemConfigType config =
                GetGemType(center);

            // Ngang
            int horizontal = 1;

            horizontal += CountDirection(
                x,
                y,
                -1,
                0,
                config);

            horizontal += CountDirection(
                x,
                y,
                1,
                0,
                config);

            if (horizontal >= 3)
                return true;

            // Dọc
            int vertical = 1;

            vertical += CountDirection(
                x,
                y,
                0,
                -1,
                config);

            vertical += CountDirection(
                x,
                y,
                0,
                1,
                config);

            return vertical >= 3;
        }

        private int CountDirection(
            int startX,
            int startY,
            int dx,
            int dy,
            GemConfigType config)
        {
            int count = 0;

            int x = startX + dx;
            int y = startY + dy;

            while (grid.IsInside(x, y))
            {
                GemTile gem =
                    grid.GetGem(x, y);

                if (gem == null)
                    break;

                if (GetGemType(gem) != config)
                    break;

                count++;

                x += dx;
                y += dy;
            }

            return count;
        }

        // =====================================================
        // LẤY LOẠI GEM
        // =====================================================

        private GemConfigType GetGemType(GemTile gem)
        {
            return new GemConfigType(
                gem.Config);
        }

        // Wrapper để so sánh GemConfig
        private readonly struct GemConfigType
        {
            private readonly object value;

            public GemConfigType(object value)
            {
                this.value = value;
            }

            public override bool Equals(object obj)
            {
                if (obj is GemConfigType other)
                    return Equals(value, other.value);

                return false;
            }

            public override int GetHashCode()
            {
                return value != null
                    ? value.GetHashCode()
                    : 0;
            }

            public static bool operator ==(
                GemConfigType a,
                GemConfigType b)
            {
                return Equals(a.value, b.value);
            }

            public static bool operator !=(
                GemConfigType a,
                GemConfigType b)
            {
                return !(a == b);
            }
        }
    }
}