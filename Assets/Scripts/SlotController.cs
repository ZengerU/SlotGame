using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    private const float SpinMaxSpeed = 3f;
    public float speed;

    public float followY;
    public float topY, elementOffset;
    private float _minY;


    public event Action ToggleBlur;
    
    private void Awake()
    {
        followY = transform.GetChild(transform.childCount - 1).localPosition.y;
        topY = transform.GetChild(0).localPosition.y;
        elementOffset = transform.GetChild(0).localPosition.y - transform.GetChild(1).localPosition.y;
        _minY = followY - elementOffset;
        GetComponent<VerticalLayoutGroup>().enabled = false;
    }

    public IEnumerator Spin(float delay, float spinTime, float stopTime, ElementType target)
    {
        float top = elementOffset * 2;
        float yEndValue = top - (elementOffset * (4 - (int) target));
        yield return new WaitForSeconds(delay);
        DOTween.To(val => speed = val,0, SpinMaxSpeed, 1f).OnComplete(ToggleBlur.Invoke).SetUpdate(UpdateType.Fixed);
        yield return new WaitForSeconds(spinTime);
        ToggleBlur.Invoke();
        DOTween.To(val => speed = val,SpinMaxSpeed, 0, stopTime).SetUpdate(UpdateType.Fixed);
    }

    private void FixedUpdate()
    {
        followY -= speed;
        if (followY < _minY)
        {
            followY = topY;
        }
    }
}
