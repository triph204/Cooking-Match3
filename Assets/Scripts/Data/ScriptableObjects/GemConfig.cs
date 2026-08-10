using UnityEngine;
using Match3.Gameplay.Board;

namespace Match3.Data.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Match3/Gem Config")]
    public class GemConfig : ScriptableObject
    {
        public string id;

        public Sprite icon;

        public GemTile prefab;
    }
}