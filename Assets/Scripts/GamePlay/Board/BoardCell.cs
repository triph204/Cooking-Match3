using UnityEngine;

namespace Match3.Gameplay.Board
{
    public class BoardCell
    {
        public Vector2Int Position { get; }

        public GemTile CurrentGem { get; private set; }

        public bool IsEmpty => CurrentGem == null;

        public BoardCell(int x, int y)
        {
            Position = new Vector2Int(x, y);
        }

        public void SetGem(GemTile gem)
        {
            CurrentGem = gem;

            if (gem != null)
                gem.SetGridPosition(Position);
        }

        public void Clear()
        {
            CurrentGem = null;
        }
    }
}