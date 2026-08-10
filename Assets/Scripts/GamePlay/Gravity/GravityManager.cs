using System.Collections;
using System.Collections.Generic;
using Match3.Gameplay.Board;
using UnityEngine;

namespace Match3.Gameplay.Gravity
{
    public class GravityManager : MonoBehaviour
    {
        [SerializeField]
        private GridManager grid;

        [Header("Fall Animation")]
        [SerializeField]
        private float fallSpeed = 8f;

        private void Awake()
        {
            if (grid == null)
                grid = FindFirstObjectByType<GridManager>();
        }

        public IEnumerator Collapse()
        {
            if (grid == null)
                yield break;

            List<GemFallData> falls =
                new List<GemFallData>();

            // =====================================================
            // TÌM GEM CẦN RƠI
            // =====================================================

            for (int x = 0; x < grid.Width; x++)
            {
                int writeY = grid.Height - 1;

                for (int readY = grid.Height - 1;
                     readY >= 0;
                     readY--)
                {
                    GemTile gem =
                        grid.GetGem(x, readY);

                    if (gem == null)
                        continue;

                    if (readY != writeY)
                    {
                        BoardCell oldCell =
                            grid.GetCell(
                                x,
                                readY);

                        BoardCell newCell =
                            grid.GetCell(
                                x,
                                writeY);

                        if (oldCell == null ||
                            newCell == null)
                        {
                            continue;
                        }

                        if (gem == null)
                            continue;

                        Vector3 target =
                            grid.GetWorldPosition(
                                x,
                                writeY);

                        // Lưu vị trí bắt đầu
                        Vector3 start =
                            gem.transform.position;

                        // Cập nhật board trước
                        oldCell.Clear();

                        newCell.SetGem(gem);

                        gem.SetGridPosition(
                            new Vector2Int(
                                x,
                                writeY));

                        falls.Add(
                            new GemFallData(
                                gem,
                                start,
                                target));
                    }

                    writeY--;
                }
            }

            // =====================================================
            // CHO GEM RƠI
            // =====================================================

            bool moving = true;

            while (moving)
            {
                moving = false;

                for (int i = 0;
                     i < falls.Count;
                     i++)
                {
                    GemFallData fall =
                        falls[i];

                    if (fall.gem == null)
                        continue;

                    float distance =
                        Vector3.Distance(
                            fall.start,
                            fall.target);

                    float duration =
                        distance /
                        Mathf.Max(
                            fallSpeed,
                            0.01f);

                    if (duration <= 0.001f)
                    {
                        fall.gem.transform.position =
                            fall.target;

                        continue;
                    }

                    fall.time +=
                        Time.deltaTime;

                    float t =
                        Mathf.Clamp01(
                            fall.time /
                            duration);

                    t = Mathf.SmoothStep(
                        0f,
                        1f,
                        t);

                    if (fall.gem != null)
                    {
                        fall.gem.transform.position =
                            Vector3.Lerp(
                                fall.start,
                                fall.target,
                                t);
                    }

                    if (t < 1f)
                        moving = true;
                }

                yield return null;
            }

            // =====================================================
            // ĐẢM BẢO VỊ TRÍ CUỐI
            // =====================================================

            foreach (GemFallData fall in falls)
            {
                if (fall.gem == null)
                    continue;

                fall.gem.transform.position =
                    fall.target;
            }
        }

        // =====================================================
        // DATA
        // =====================================================

        private class GemFallData
        {
            public GemTile gem;

            public Vector3 start;
            public Vector3 target;

            public float time;

            public GemFallData(
                GemTile gem,
                Vector3 start,
                Vector3 target)
            {
                this.gem = gem;
                this.start = start;
                this.target = target;
                this.time = 0f;
            }
        }
    }
}