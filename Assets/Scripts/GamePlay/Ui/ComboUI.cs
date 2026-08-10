using System.Collections;
using TMPro;
using UnityEngine;

namespace Match3.Gameplay.UI
{
    public class ComboUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text comboText;

        [Header("Animation")]
        [SerializeField] private float appearDuration = 0.15f;
        [SerializeField] private float holdDuration = 0.5f;
        [SerializeField] private float fadeDuration = 0.2f;

        [SerializeField] private float startScale = 0.5f;
        [SerializeField] private float maxScale = 1.15f;

        private Coroutine currentCoroutine;

        private void Awake()
        {
            if (comboText == null)
            {
                comboText =
                    GetComponentInChildren<TMP_Text>(true);
            }

            if (comboText == null)
            {
                Debug.LogError(
                    "ComboUI: Không tìm thấy TMP_Text.");
            }
        }

        private void Start()
        {
            HideImmediately();
        }

        public void ShowCombo(int combo)
        {
            if (combo < 2)
                return;

            if (comboText == null)
            {
                Debug.LogError(
                    "ComboUI: comboText chưa được gán.");
                return;
            }



            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentCoroutine =
                StartCoroutine(
                    ShowComboCoroutine(combo));
        }

        private IEnumerator ShowComboCoroutine(
            int combo)
        {
            if (comboText == null)
                yield break;

            comboText.text =
                "COMBO x" + combo;

            comboText.gameObject.SetActive(true);

            RectTransform rect =
                comboText.rectTransform;

            Vector3 normalScale =
                Vector3.one;

            rect.localScale =
                normalScale * startScale;

            Color color =
                comboText.color;

            color.a = 1f;

            comboText.color =
                color;

            float time = 0f;

            while (time < appearDuration)
            {
                time += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        time / appearDuration);

                t = Mathf.SmoothStep(
                    0f,
                    1f,
                    t);

                float scale =
                    Mathf.Lerp(
                        startScale,
                        maxScale,
                        t);

                rect.localScale =
                    normalScale * scale;

                yield return null;
            }

            rect.localScale =
                normalScale * maxScale;

    

            time = 0f;

            while (time < holdDuration)
            {
                time += Time.deltaTime;

                yield return null;
            }


            time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        time / fadeDuration);

                color =
                    comboText.color;

                color.a =
                    Mathf.Lerp(
                        1f,
                        0f,
                        t);

                comboText.color =
                    color;

                rect.localScale =
                    Vector3.Lerp(
                        normalScale * maxScale,
                        normalScale,
                        t);

                yield return null;
            }

            HideImmediately();

            currentCoroutine = null;
        }

        private void HideImmediately()
        {
            if (comboText == null)
                return;

            Color color =
                comboText.color;

            color.a = 0f;

            comboText.color =
                color;

            comboText.rectTransform.localScale =
                Vector3.one;

            comboText.gameObject.SetActive(true);
        }
    }
}