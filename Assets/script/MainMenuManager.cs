using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public PanelOpeing PanelMainMenu;
    public PanelOpeing PanelLevel;
    public PanelOpeing PanlOption;
    public PanelOpeing PanelCredits;


    public void OpenLevelPanel() {
        PanelLevel.gameObject.SetActive(true);
        PanelLevel.OpenPanel();
        PanelMainMenu.ClosePanel();
    }
}
