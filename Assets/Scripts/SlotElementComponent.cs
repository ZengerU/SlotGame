/*
 * Created By Umut Zenger
 * https://github.com/ZengerU/SlotGame
 */
using UnityEngine;

/// <summary>
/// Single slot element on a slot column.
/// </summary>
public class SlotElementComponent : MonoBehaviour
{
    private SlotElement _element;
    private SpriteRenderer _spriteRenderer;
    private SlotController _controller;
    private int _distance;

    private void Reset()
    {
        _element = Resources.Load<SlotElement>($"Elements/{gameObject.name}");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _element.normal;
    }

    private void Awake()
    {
        Reset();
        _controller = transform.parent.GetComponent<SlotController>();
        _controller.ToggleBlur += ToggleBlur;
        _distance = transform.parent.childCount - transform.GetSiblingIndex() - 1;
    }
    private void FixedUpdate()
    {
        PositionSelf();
    }

    /// <summary>
    /// Positions self every fixed update according to its own slot column controller's followY value.
    /// </summary>
    private void PositionSelf()
    {
        float targetY = _controller.followY + _distance * _controller.elementOffset;
        if (targetY > _controller.topY)
        {
            targetY = _controller.followY - (_controller.elementOffset * (_controller.transform.childCount - _distance));
        }
        transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
    }

    /// <summary>
    /// Toggles blur on or off.
    /// </summary>
    private void ToggleBlur()
    {
        bool isBlurOn = _spriteRenderer.sprite == _element.blur;
        _spriteRenderer.sprite = isBlurOn ? _element.normal : _element.blur;
    }
}