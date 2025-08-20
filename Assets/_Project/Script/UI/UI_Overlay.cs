using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Overlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool _isPointerEnter;
    public bool IsPointerEnter() => _isPointerEnter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerEnter = false;
    }

    void OnDisable()
    {
        _isPointerEnter = false;
    }
}
