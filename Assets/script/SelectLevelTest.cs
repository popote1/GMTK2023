using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectLevelTest : UIButton
{
    
    [Header("Sub Element Parameters")]
    
    [SerializeField] private Transform[] _subElements;
    public float _subAnimationTime=0.2f;
    public float _subAnimationSize=1.1f;
    public Vector3 _subAnimationRotation = new Vector3(0, 0, 2);
    public AnimationCurve _subAnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    
    
    
    
    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        if (IsSelectable) {
            foreach (var element in _subElements) {
                if (element == null) continue;
                element.DOPause();
                element.localScale = Vector3.one;
                //element.eulerAngles = Vector3.zero;
                element.DOScale(_subAnimationSize, _subAnimationTime).SetEase(_subAnimationCurve);
                //element.DORotate(_subAnimationRotation, _subAnimationTime).SetEase(_subAnimationCurve);
            }
        }
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        if (IsSelectable) {
            foreach (var element in _subElements) {
                if (element == null) continue;
                element.DOPause();
                element.localScale = Vector3.one * _subAnimationSize;
                //element.eulerAngles = _subAnimationRotation;
                element.DOScale(Vector3.one, _subAnimationTime).SetEase(_subAnimationCurve);
                //element.DORotate(Vector3.zero, _subAnimationTime).SetEase(_subAnimationCurve);
            }
        }
    }
}
