using UnityEngine;

public class UI_CanvasOverlay : MonoBehaviour
{
    private UI_Overlay[] _uiOverlays;

    private bool _isPointerOnOverlay;
    public bool IsPointerOnOverlay() => _isPointerOnOverlay;

    void Awake()
    {
        _uiOverlays = GetComponentsInChildren<UI_Overlay>(true);
    }

    void Update()
    {
        _isPointerOnOverlay = false;
        foreach (UI_Overlay overlay in _uiOverlays)
        {
            if (overlay.IsPointerEnter())
            {
                _isPointerOnOverlay = true;
                break;
            }
        }
    }

}
