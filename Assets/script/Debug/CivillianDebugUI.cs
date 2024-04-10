
using script;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivillianDebugUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _staminaBar;
    [SerializeField] private CivillianAgent _civillianAgent;
    [SerializeField] private TMP_Text _txtMoveSpeed;
    [SerializeField] private Image _imgChased;
    
    void Start() {
        _canvas.worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        _canvas.transform.localEulerAngles = new Vector3(0,180-_civillianAgent.transform.eulerAngles.y, 0);
        //_canvas.transform.forward = transform.position - _canvas.worldCamera.transform.position;
        _txtMoveSpeed.text = _civillianAgent.GetDebugMoveSpeed().ToString();
        _imgChased.enabled = _civillianAgent.IsRunAway;
        _staminaBar.fillAmount = _civillianAgent.GetCurrentStaminaStat();
    }
}
