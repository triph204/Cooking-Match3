using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject[] allPanels; // Kéo TẤT CẢ panel vào đây

    public void ShowPanel(GameObject panelToShow)
    {
        foreach (var panel in allPanels)
        {
            panel.SetActive(panel == panelToShow);
        }
    }

    public void HideAllPanels()
    {
        foreach (var panel in allPanels)
            panel.SetActive(false);
    }
}