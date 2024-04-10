using System.Collections;
using System.Collections.Generic;
using script;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ZombieDebugUI : MonoBehaviour
{
    
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _hpBar;
    [SerializeField] private GameObject _transformationBarBarckGround;
    [SerializeField] private Image _transformationBar;
    [SerializeField] private ZombieAgent _zombieAgent;
    [SerializeField] private TMP_Text _txtMoveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _canvas.transform.localEulerAngles = new Vector3(0,180-_zombieAgent.transform.eulerAngles.y, 0);
        //_canvas.transform.forward = transform.position - _canvas.worldCamera.transform.position;
        _hpBar.fillAmount = _zombieAgent.GetNormalizeHp();
        if (_zombieAgent.IsTransforming()) {
            _transformationBarBarckGround.gameObject.SetActive(true);
            _transformationBar.fillAmount = _zombieAgent.GetNormalizeTransformation();
        }
        else {
            _transformationBarBarckGround.gameObject.SetActive(false);
        }
    }
}
