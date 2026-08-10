using UnityEngine;

namespace Match3.Core.Services
{
    public class RandomGemService
    {
        public int GetRandomIndex(int max)
        {
            return Random.Range(0, max);
        }
    }
}
