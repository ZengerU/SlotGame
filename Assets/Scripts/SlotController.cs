/*
 * Created By Umut Zenger
 * https://github.com/ZengerU/SlotGame
 */

using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component used on each column of a slot.
/// </summary>
public class SlotController : MonoBehaviour
{
    private const float SpinMaxSpeed = 1f;
    private float _speed, _minY;
    private GameController _controller;
    [HideInInspector] public float followY, topY, elementOffset;

    /// <summary>
    /// Received from elements on this row, toggles blur on or off.
    /// </summary>
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

    /// <summary>
    /// Spins the column it is on.
    /// </summary>
    /// <param name="delay">Time to wait before starting to spin.</param>
    /// <param name="spinTime">Time between starting to spin and stopping to spin.</param>
    /// <param name="stopTime">Time it takes to come to a full stop after starting to stop.</param>
    /// <param name="target">The target value followY should end up on.</param>
    /// <returns></returns>
    public IEnumerator Spin(float delay, float spinTime)
    {
        yield return new WaitForSeconds(delay);
        DOTween.To(val => _speed = val, 0, SpinMaxSpeed, 1f).OnComplete(ToggleBlur.Invoke).SetUpdate(UpdateType.Fixed);
        yield return new WaitForSeconds(spinTime);
        _controller.SpinnerReachedAllocatedTime();
    }

    public IEnumerator StopSpin(float delay, float stopTime, ElementType target)
    {
        yield return new WaitForSeconds(delay);
        float yEndValue = target switch
        {
            ElementType.A => 2.8f,
            ElementType.Bonus => 5.6f, // or -8.4f
            ElementType.Seven => -5.6f,
            ElementType.Wild => -2.8f,
            ElementType.Jackpot => 0f
        };
        _speed = 0; //TODO: ease out
        ToggleBlur.Invoke();
        float stepAmount = Time.fixedDeltaTime / stopTime;
        Tweener slowDowner = null;
        if (stopTime > 1)
        {
            float distance = (SpinMaxSpeed / 2) * stopTime;
            float point = distance % (topY - _minY);
            followY = point;
            slowDowner = DOTween.To(val => _speed = val, SpinMaxSpeed, 0.1f, stopTime).SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear);
            slowDowner.Play();
        }
        else
        {
            followY = yEndValue + elementOffset;
            _speed = (followY - yEndValue) * stepAmount;
        }
        yield return new WaitForSeconds(stopTime -.2f);
        yield return new WaitUntil(() => followY - yEndValue < .1f && followY > yEndValue);
        slowDowner?.Kill();
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