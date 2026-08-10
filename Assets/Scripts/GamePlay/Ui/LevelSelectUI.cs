using Match3.Gameplay.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;

    [SerializeField] private string[] sceneNames;

    [SerializeField] private float lockedAlpha = 0.5f;

    private void Start()
    {
        RefreshButtons();
    }

    private void OnEnable()
    {
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        if (levelButtons == null)
            return;

        for (int i = 0;
             i < levelButtons.Length;
             i++)
        {
            Button button =
                levelButtons[i];

            if (button == null)
                continue;

            int levelNumber =
                i + 1;

            bool unlocked =
                GameManager.IsLevelUnlocked(
                    levelNumber);

            SetLocked(
                button,
                !unlocked);

            button.onClick.RemoveAllListeners();

            if (!unlocked)
                continue;

            if (sceneNames == null)
                continue;

            if (i >= sceneNames.Length)
                continue;

            string sceneToLoad =
                sceneNames[i];

            if (string.IsNullOrEmpty(
                    sceneToLoad))
                continue;

            button.onClick.AddListener(
                () =>
                {
                    Time.timeScale = 1f;

                    SceneManager.LoadScene(
                        sceneToLoad);
                });
        }
    }

    private void SetLocked(
        Button button,
        bool locked)
    {
        button.interactable =
            !locked;

        CanvasGroup group =
            button.GetComponent<CanvasGroup>();

        if (group == null)
        {
            group =
                button.gameObject
                    .AddComponent<CanvasGroup>();
        }

        group.alpha =
            locked
                ? lockedAlpha
                : 1f;

        group.interactable =
            !locked;

        group.blocksRaycasts =
            !locked;
    }
}