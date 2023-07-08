using System;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;

namespace script
{
    public class HUDManager :MonoBehaviour
    {
        public TMP_Text TxtZombieCount;
        public GameObject PanelWin;
        public GameObject PanelLose;


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
    }
}