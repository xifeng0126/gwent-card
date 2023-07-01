using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 originalPosition;
    private Vector3 raisedPosition = new Vector3(0f, 0.2f, 0f); // 抬高的位置，可以根据需要进行调整
    private bool isHovering = false;

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 在拖拽开始时触发
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 在拖拽过程中触发
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 在拖拽结束时触发
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering)
        {
            transform.position += raisedPosition;
            isHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHovering)
        {
            transform.position = originalPosition;
            isHovering = false;
        }
    }
}
