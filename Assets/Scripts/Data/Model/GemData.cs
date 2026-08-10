using UnityEngine;

namespace Match3.Data.Models
{
    public enum GemType
    {
        icon1,
        icon2,
        icon3,
        icon4,
        icon5,
        icon6,
        icon7,
        icon8,
        icon9,
        icon10,
        icon11
    }

    [System.Serializable]
    public class GemData
    {
        public GemType Type;

        public GemData(GemType type)
        {
            Type = type;
        }
    }
}