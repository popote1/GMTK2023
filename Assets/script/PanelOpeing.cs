using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelOpeing : MonoBehaviour
{
    public float AnimationTime=0.2f;
    
    public Vector3 CloseAngle = new Vector3(-90, 0, 0);
    public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public List<Button> Buttons;
    

    
    [ContextMenu("Open")]
    public void OpenPanel()
    {
        transform.DOPause();
        transform.eulerAngles = CloseAngle;
        transform.DORotate(Vector3.zero, AnimationTime).SetEase(AnimationCurve).OnComplete(UnlockAllButton);;
    }

    [ContextMenu("Close")]
    public void ClosePanel() {
        transform.DOPause();
        LockAllButton();
        transform.DOPause();
        transform.eulerAngles = Vector3.zero;
        transform.DORotate(CloseAngle, AnimationTime).OnComplete(CloseGameObject);
        
    }

    private void UnlockAllButton() {
        foreach (var button in Buttons) {
            button.interactable = true;
            if (button.GetComponent<UIButton>())
            {
                button.GetComponent<UIButton>().IsSelectable = true;
                button.GetComponent<UIButton>().OnPointerExit(null);
            }
        }
    }

    private void LockAllButton() {
        foreach (var button in Buttons) {
            button.interactable = false;
            if (button.GetComponent<UIButton>()) button.GetComponent<UIButton>().IsSelectable = false;
        }
    }

    private void CloseGameObject()
    {
        gameObject.SetActive(false);
    }
}
