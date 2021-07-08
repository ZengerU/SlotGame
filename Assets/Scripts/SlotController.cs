using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    private const float SpinMaxSpeed = 1f;
    private float _speed, _minY;
    private GameController _controller;
    public float followY, topY, elementOffset;


    public event Action ToggleBlur;

    private void Awake()
    {
        _controller = FindObjectOfType<GameController>();
        followY = transform.GetChild(transform.childCount - 1).localPosition.y;
        topY = transform.GetChild(0).localPosition.y;
        elementOffset = transform.GetChild(0).localPosition.y - transform.GetChild(1).localPosition.y;
        _minY = followY - elementOffset;
        GetComponent<VerticalLayoutGroup>().enabled = false;
    }

    public IEnumerator Spin(float delay, float spinTime, float stopTime, ElementType target)
    {
        float top = elementOffset * 2;
        float yEndValue = target switch
        {
            ElementType.A => 2.8f,
            ElementType.Bonus => 5.6f, // or -8.4f
            ElementType.Seven => -5.6f,
            ElementType.Wild => -2.8f,
            ElementType.Jackpot => 0f
        };
        yield return new WaitForSeconds(delay);
        DOTween.To(val => _speed = val, 0, SpinMaxSpeed, 1f).OnComplete(ToggleBlur.Invoke).SetUpdate(UpdateType.Fixed);
        yield return new WaitForSeconds(spinTime);
        ToggleBlur.Invoke();
        float distance = followY > yEndValue ? followY - yEndValue : (followY - _minY) + (topY - yEndValue);
        _speed = distance * .8f / stopTime * Time.fixedDeltaTime;
        yield return new WaitForSeconds(stopTime);
        yield return new WaitUntil(() => followY - yEndValue <= .1f);
        _speed = 0;
        followY = yEndValue;
        _controller.SpinnerStopped();
    }

    private void FixedUpdate()
    {
        followY -= _speed;
        if (followY < _minY)
        {
            followY = topY;
        }
    }
}