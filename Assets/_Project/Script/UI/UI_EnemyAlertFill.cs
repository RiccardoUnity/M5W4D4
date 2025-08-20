using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyAlertFill : MonoBehaviour
{
    private Transform _camera;
    private Enemy_FSM_Controller _controller;

    [SerializeField] private Image _bar;
    private Vector3 _scale = new Vector3(0f, 1f, 1f);
    [SerializeField] private Gradient _gradient;

    void Awake()
    {
        _camera = Camera.main.transform;
        _controller = GetComponentInParent<Enemy_FSM_Controller>();
        _controller.onAlertChange += AlertChange;

        _bar.rectTransform.localScale = _scale;
    }

    void LateUpdate()
    {
        transform.rotation = _camera.rotation;
    }

    public void AlertChange(Enemy_FSM_Controller controller, float x)
    {
        if (controller == _controller)
        {
            _scale.x = x;
            _bar.rectTransform.localScale = _scale;
            _bar.color = _gradient.Evaluate(x);
        }
        else
        {
            Debug.LogWarning("Un Enemy_FSM_Controller sta provando a cambiare un fill non suo", controller);
        }
    }
}
