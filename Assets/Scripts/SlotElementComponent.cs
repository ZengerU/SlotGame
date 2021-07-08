using System;
using UnityEngine;

public class SlotElementComponent : MonoBehaviour
{
    public SlotElement element;
    private GameObject _normalSprite, _blurSprite;
    private SlotController _controller;
    private int _distance;

    private void Reset()
    {
        element = Resources.Load<SlotElement>($"Elements/{gameObject.name}");
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = element.normal;
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = element.blur;
        transform.GetChild(1).gameObject.SetActive(false);
    }

    private void Awake()
    {
        _controller = transform.parent.GetComponent<SlotController>();
        _controller.ToggleBlur += ToggleBlur;
        _normalSprite = transform.GetChild(0).gameObject;
        _blurSprite = transform.GetChild(1).gameObject;
    }

    private void Start()
    {
        _distance = transform.parent.childCount - transform.GetSiblingIndex() - 1;
    }
    private void FixedUpdate()
    {
        PositionSelf();
    }

    private void PositionSelf()
    {
        float targetY = _controller.followY + _distance * _controller.elementOffset;
        if (targetY > _controller.topY)
        {
            targetY = _controller.followY - (_controller.elementOffset * (_controller.transform.childCount - _distance));
        }
        transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
    }

    private void ToggleBlur()
    {
        _blurSprite.SetActive(!_blurSprite.activeSelf);
        _normalSprite.SetActive(!_normalSprite.activeSelf);
    }
}