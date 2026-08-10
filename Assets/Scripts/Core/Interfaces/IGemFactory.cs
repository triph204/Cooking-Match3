using UnityEngine;
using Match3.Gameplay.Board;

namespace Match3.Core.Interfaces
{
    public interface IGemFactory
    {
        GemTile CreateRandomGem(Vector2Int gridPosition, Transform parent);

        GemTile CreateGem(int index, Vector2Int gridPosition, Transform parent);
    }
}