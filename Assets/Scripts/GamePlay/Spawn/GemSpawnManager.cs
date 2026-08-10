using Match3.Data.ScriptableObjects;
using Match3.Gameplay.Board;
using System.Collections;
using UnityEngine;

namespace Match3.Gameplay.Spawn
{
    public class GemSpawnManager : MonoBehaviour
    {
        [SerializeField]
        private GridManager grid;

        [Header("Spawn")]
        [SerializeField]
        private float spawnHeight = 1f;

        [SerializeField]
        private float spawnSpeed = 8f;

        private void Awake()
        {
            if (grid == null)
                grid =
                    FindFirstObjectByType<GridManager>();
        }

        public IEnumerator FillEmptyCells()
        {
            if (grid == null)
                yield break;

            bool hasSpawn =
                false;

            for (int x = 0;
                 x < grid.Width;
                 x++)
            {
                for (int y = 0;
                     y < grid.Height;
                     y++)
                {
                    if (grid.GetGem(x, y) != null)
                        continue;

                    GemConfig config =
                        grid.GetRandomConfig();

                    if (config == null)
                        continue;

                    GemTile gem =
                        grid.CreateGem(
                            x,
                            y,
                            config);

                    if (gem == null)
                        continue;

                    hasSpawn = true;

                    // Cho gem xuất hiện phía trên board
                    Vector3 target =
                        grid.GetWorldPosition(
                            x,
                            y);

                    Vector3 start =
                        target +
                        Vector3.up *
                        spawnHeight;

                    gem.transform.position =
                        start;

                    StartCoroutine(
                        MoveGem(
                            gem,
                            target));
                }
            }

            if (hasSpawn)
            {
                yield return new WaitForSeconds(
                    1f / spawnSpeed);
            }
        }

        private IEnumerator MoveGem(
            GemTile gem,
            Vector3 target)
        {
            if (gem == null)
                yield break;

            Vector3 start =
                gem.transform.position;

            float distance =
                Vector3.Distance(
                    start,
                    target);

            float duration =
                distance / spawnSpeed;

            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        time / duration);

                t = Mathf.SmoothStep(
                    0f,
                    1f,
                    t);

                gem.transform.position =
                    Vector3.Lerp(
                        start,
                        target,
                        t);

                yield return null;
            }

            gem.transform.position =
                target;
        }
    }
}