using System;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace script
{
    public class HUDManager :MonoBehaviour
    {
        public TMP_Text TxtZombieCount;
        public GameObject PanelWin;
        public GameObject PanelLose;
        public RectTransform SelectionBox;


        private void Start() {
            StaticData.OnZombieGain += SetZombieValue;
            StaticData.OnZombieLose += SetZombieValue;
            StaticData.OnGameWin += PlayWin;
            StaticData.OnGameLose += PlayLose;
        }

        private void OnDestroy()
        {
            StaticData.OnZombieGain -= SetZombieValue;
            StaticData.OnZombieLose -= SetZombieValue;
            StaticData.OnGameWin -= PlayWin;
            StaticData.OnGameLose -= PlayLose;
        }

        public void SetZombieValue() {
            TxtZombieCount.text = StaticData.ZombieCount.ToString();
        }

        public void PlayWin() {
            PanelWin.SetActive(true);
        }

        public void PlayLose() {
            PanelLose.SetActive(true);
        }

        public void ReturnToMainMenu() {
            GridManager.Instance = null;
            SceneManager.LoadScene(0);
        }

        public void SetSelectionBox(Vector2 pos, Vector2 Size) {
            SelectionBox.gameObject.SetActive(true);
            SelectionBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size.x);
            SelectionBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size.y);
            SelectionBox.position = pos;
            
        }

        public void CloseSelectionBox() => SelectionBox.gameObject.SetActive(false);
    }
}