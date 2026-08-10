using Match3.Data.ScriptableObjects;
using Match3.Gameplay.Audio;
using Match3.Gameplay.Swap;
using System.Collections;
using UnityEngine;

namespace Match3.Gameplay.Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class GemTile : MonoBehaviour
    {
        public GemConfig Config { get; private set; }
        public Vector2Int GridPosition { get; private set; }

        private SpriteRenderer spriteRenderer;
        private SwapManager swapManager;
        private Vector3 normalScale;
        private bool isDestroying;

        [Header("Select")]
        [SerializeField] private float selectedScale = 1.08f;
        [SerializeField] private GameObject selectEffect;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            swapManager = FindFirstObjectByType<SwapManager>();
        }

        public void Initialize(GemConfig config, Vector2Int position)
        {
            Config = config;
            GridPosition = position;

            if (spriteRenderer != null)
                spriteRenderer.sprite = config.icon;

            normalScale = transform.localScale;
            isDestroying = false;

            if (selectEffect != null)
                selectEffect.SetActive(false);
        }

        public void SetGridPosition(Vector2Int position)
        {
            GridPosition = position;
        }

        public void SetSelected(bool selected)
        {
            if (isDestroying)
                return;

            if (selectEffect != null)
                selectEffect.SetActive(selected);

            transform.localScale =
                selected
                    ? normalScale * selectedScale
                    : normalScale;
        }

        public void StartResultDisappear(float duration)
        {
            if (isDestroying)
                return;

            isDestroying = true;
            StartCoroutine(ResultDisappearCoroutine(duration));
        }

        private IEnumerator ResultDisappearCoroutine(float duration)
        {
            Vector3 startScale = transform.localScale;
            float time = 0f;

            if (selectEffect != null)
                selectEffect.SetActive(false);

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(time / duration);
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.localScale =
                    Vector3.Lerp(
                        startScale,
                        Vector3.zero,
                        t);

                yield return null;
            }

            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }

        public IEnumerator PlayDestroyAnimation(float duration)
        {
            if (isDestroying)
                yield break;

            isDestroying = true;

            Vector3 startScale = transform.localScale;
            float time = 0f;

            if (selectEffect != null)
                selectEffect.SetActive(false);

            while (time < duration)
            {
                time += Time.deltaTime;

                float t = Mathf.Clamp01(time / duration);
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.localScale =
                    Vector3.Lerp(
                        startScale,
                        Vector3.zero,
                        t);

                yield return null;
            }

            transform.localScale = Vector3.zero;
        }

        private void OnMouseDown()
        {
            if (isDestroying)
                return;

            if (swapManager == null)
                swapManager = FindFirstObjectByType<SwapManager>();

            if (swapManager == null)
                return;

            BoardCell cell = FindCell();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.clickSound);
            }
            if (cell == null)
                return;

            swapManager.SelectGem(cell);
        }

        private BoardCell FindCell()
        {
            GridManager grid =
                FindFirstObjectByType<GridManager>();

            if (grid == null)
                return null;

            return grid.GetCell(
                GridPosition.x,
                GridPosition.y);
        }
    }
}