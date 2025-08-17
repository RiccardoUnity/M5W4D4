using UnityEngine;
using GM;

public class Noise : MonoBehaviour
{
    private NoiseType _type = NoiseType.None;
    public NoiseType GetTypeNoise() => _type;

    private PlayerController _playerController;
    private Distraction _distraction;

    private const float _maxIntensity_c = 15f;
    private const float _coeffErrorMaxIntensity_c = 5f;

    private int _indexNotAllocate;
    [SerializeField] private Vector3 _offset = Vector3.zero;
    [Range(1f, _maxIntensity_c)]
    [SerializeField] private float _intensity = 10f;
    private float _intensitySqr;
    private Collider[] _colliders = new Collider[10];
    private LayerMask _layerMaskCharacter = (1 << 3) | (1 << 6);
    private LayerMask _layerMaskStructure = (1 << 0);
    private QueryTriggerInteraction _qti = QueryTriggerInteraction.Ignore;

    void Awake()
    {
        if (_intensity < 0f)
        {
            _intensity = 5f;
        }
        _intensitySqr = _intensity * _intensity;

        _playerController = GetComponentInParent<PlayerController>();
        _distraction = GetComponent<Distraction>();
        if (_playerController != null)
        {
            _type = NoiseType.Step;
            _playerController.onStep += Emission;
        }
        else if (_distraction != null)
        {
            _type = NoiseType.Hit;
        }
    }

    public void Emission()
    {
        _indexNotAllocate = Physics.OverlapSphereNonAlloc(transform.position + _offset, _intensity, _colliders, _layerMaskCharacter, _qti);

        for (int i = 0; i < _indexNotAllocate; i++)
        {
            EnemyHear hear = _colliders[i].GetComponent<EnemyHear>();
            if (hear != null)
            {
                //Dal punto di vista di ascolta
                //Più sei vicino al rumore, più questo valore tende a 0
                float intensity = 1f - Mathf.Exp(-(transform.position - hear.transform.position).sqrMagnitude / _intensitySqr);
                Vector3 position = transform.position + Random.insideUnitSphere * (_coeffErrorMaxIntensity_c * intensity);
                //Se c'è una parete tra l'origine del suono e chi ascolta, il suono perde di importanza (tende a 1f)
                if (Physics.Linecast(transform.position + _offset, hear.transform.position, _layerMaskStructure, _qti))
                {
                    intensity = Mathf.Clamp01(intensity * 2f);
                }
                hear.Listen(position, _type, intensity);
            }
        }
    }
}
