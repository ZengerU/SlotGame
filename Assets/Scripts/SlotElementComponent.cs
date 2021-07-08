using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class SlotElementComponent : MonoBehaviour
{
    public SlotElement element;
    private SpriteRenderer _normalSprite, _blurSprite;
    private bool _isBlurActive;
    private const float SpinSpeed = 1f, SpeedUpStep = .1f, MinimumY = -5f;
    private float _spinTime, _stopTime, _currentSpeed, _yOffset, _speedDownStep;
    
    [ReadOnly][SerializeField]
    private Transform topTransform;

    private SlotElementState _state = SlotElementState.Stationary;

    private void Awake()
    {
        _yOffset = transform.parent.GetChild(0).position.y - transform.parent.GetChild(1).position.y;
    }

    private void Reset()
    {
        SlotElement[] elements = Resources.LoadAll<SlotElement>("Elements");
        element = elements[transform.GetSiblingIndex()];
        List<Transform> tempList = transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }
        GameObject first = new GameObject {name = "NormalSprite"};
        first.transform.parent = transform;
        GameObject second = new GameObject {name = "BlurSprite"};
        second.transform.parent = transform;
        _normalSprite = first.AddComponent<SpriteRenderer>();
        _blurSprite = second.AddComponent<SpriteRenderer>();
        SetupSpriteRenderer(_normalSprite);
        SetupSpriteRenderer(_blurSprite);
        if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
        {
            topTransform = transform.parent.GetChild(0);
        }
        else
        {
            topTransform = transform.parent.GetChild(transform.GetSiblingIndex()+1);
        }
    }

    private void SetupSpriteRenderer(SpriteRenderer sr)
    {
        sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        sr.sortingOrder = 1;
        sr.sprite = element.normal;
        sr.transform.localPosition = Vector3.zero;
        sr.transform.localScale = Vector3.one;
    }

    private void FixedUpdate()
    {
        switch (_state)
        {
            case SlotElementState.Stationary:
                return;
            case SlotElementState.Starting:
                _currentSpeed += SpeedUpStep * Time.fixedDeltaTime;
                if (_currentSpeed >= SpinSpeed)
                {
                    _state = SlotElementState.Spinning;
                }
                DoSpinStep();
                break;
            case SlotElementState.Spinning:
                DoSpinStep();
                break;
            case SlotElementState.Stopping:
                if (_isBlurActive)
                {
                    ToggleBlur(false);
                }
                _currentSpeed -= _speedDownStep;
                if (_currentSpeed <= 0)
                {
                    _currentSpeed = 0;
                    _state = SlotElementState.Stationary;
                    return;
                }
                DoSpinStep();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public IEnumerator Spin(float delay, float spinTime, float stopTime)
    {
        yield return new WaitForSeconds(delay);
        _spinTime = spinTime;
        _stopTime = stopTime;
        _speedDownStep = stopTime * Time.fixedDeltaTime;
        yield return new WaitForSeconds(.1f); //Wait before starting to blend.
        ToggleBlur(true);
    }

    private void DoSpinStep()
    {
        transform.MoveTransformation(y: -_currentSpeed * Time.fixedDeltaTime);
        if (transform.position.y < MinimumY)
        {
            transform.SetTransformation(y: topTransform.position.y + _yOffset);
        }
        _spinTime -= Time.fixedDeltaTime;
        if (_spinTime <= 0)
        {
            _state = SlotElementState.Stopping;
        }
    }
    
    private void ToggleBlur(bool toggleOn)
    {
        _isBlurActive = toggleOn;
        _blurSprite.gameObject.SetActive(_isBlurActive);
        _normalSprite.gameObject.SetActive(!_isBlurActive);
    }

}

public enum SlotElementState
{
    Stationary,
    Starting,
    Spinning,
    Stopping
}
