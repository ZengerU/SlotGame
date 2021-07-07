using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class SlotElementComponent : MonoBehaviour
{
    public SlotElement element;
    private SpriteRenderer _spriteRenderer;

    private void Reset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SlotElement[] elements = Resources.LoadAll<SlotElement>("Elements");
        element = elements[transform.GetSiblingIndex()];
        _spriteRenderer.material = element.material;
        _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        _spriteRenderer.sortingOrder = 1;
    }
}
