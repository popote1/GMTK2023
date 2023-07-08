using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public void OpenOptionlPanel() {
        PanlOption.gameObject.SetActive(true);
        PanlOption.OpenPanel();
        PanelMainMenu.ClosePanel();
    }
    public void OpenCreditlPanel() {
        PanelCredits.gameObject.SetActive(true);
        PanelCredits.OpenPanel();
        PanelMainMenu.ClosePanel();
    }
    public void CloseCreditlPanel() {
        PanelMainMenu.gameObject.SetActive(true);
        PanelMainMenu.OpenPanel();
        PanelCredits.ClosePanel();
    }
    public void CloseOptionPanel() {
        PanelMainMenu.gameObject.SetActive(true);
        PanelMainMenu.OpenPanel();
        PanlOption.ClosePanel();
    }
    public void CloseLevellPanel() {
        PanelMainMenu.gameObject.SetActive(true);
        PanelMainMenu.OpenPanel();
        PanelLevel.ClosePanel();
    }

    public void PlayTheLevel1() {
        SceneManager.LoadScene(1);
    }
}
