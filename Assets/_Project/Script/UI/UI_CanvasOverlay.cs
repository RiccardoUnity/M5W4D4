using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_CanvasOverlay : MonoBehaviour
{
    [SerializeField] private GameObject _victory;
    private UI_Overlay[] _uiOverlays;

    private bool _isPointerOnOverlay;
    public bool IsPointerOnOverlay() => _isPointerOnOverlay;

    void Awake()
    {
        _uiOverlays = GetComponentsInChildren<UI_Overlay>(true);
        EnemySceneManager.Instance.SetCanvas(this);
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

    public void MainMenu()
    {
        SceneFade.Instance.StartFade(LoadMainMenu);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void TryAgain()
    {
        SceneFade.Instance.StartFade(LoadCurrentScene);
    }

    private void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Victory()
    {
        _victory.SetActive(true);
        EnemySceneManager.Instance.Victory(this);
    }
}
