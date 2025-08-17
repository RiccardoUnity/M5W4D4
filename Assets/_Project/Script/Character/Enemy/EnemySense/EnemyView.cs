using System.Collections.Generic;
using UnityEngine;

//Senso attivo
[RequireComponent(typeof(EnemySenseBrain))]
public class EnemyView : MonoBehaviour
{
    private EnemySenseBrain _brain;
    [SerializeField] private bool _useRandom;

    private const float _distance_c = 15f;
    private const float _distanceDelta_c = 10f;
    private const float _angleView_c = 80f;
    private const float _angleDelta_c = 30f;

    private float _marginRandom = 15f;
    [Range(_distance_c - _distanceDelta_c, _distance_c + _distanceDelta_c)]
    [SerializeField] private float _distance = _distance_c;
    private float _distanceMax;
    private float _distanceMin;
    [Range(_angleView_c - _angleDelta_c, _angleView_c + _angleDelta_c)]
    [SerializeField] private float _angleView = _angleView_c;
    private float _angleMax;
    private float _angleMin;
    private float _cos;

    private int _indexNotAllocate;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1.5f, 0f);
    private Collider[] _colliders = new Collider[10];
    private RaycastHit _hit;
    private LayerMask _layerMaskVisible = (1 << 0) | (1 << 3) | (1 << 6);
    private LayerMask _layerMaskCharacter = (1 << 3) | (1 << 6);
    private LayerMask _layerMaskStatic = (1 << 0);
    private QueryTriggerInteraction _qti = QueryTriggerInteraction.Ignore;

    private List<CharacterChat> _meet = new List<CharacterChat>();

    void Awake()
    {
        _brain = GetComponent<EnemySenseBrain>();

        if (_useRandom)
        {
            _distance = Random.Range(_distance_c - _marginRandom / 3f, _distance_c + _marginRandom / 3f);
            _angleView = Random.Range(_angleView_c - _marginRandom, _angleView_c + _marginRandom);
        }
        _distanceMax = _distance + _distanceDelta_c;
        _distanceMin = _distance - _distanceDelta_c;
        _angleMax = _angleView + _angleDelta_c;
        _angleMin = _angleView - _angleDelta_c;
        _cos = Mathf.Cos(_angleView * Mathf.PI / 360f);
    }

    //Angolo più stretto -> distanza maggiore, angolo maggiore -> distanza più breve
    public void ChangeFocusAngle(EnemySenseBrain brain, float fill)
    {
        if (brain == _brain)
        {
            _angleView = Mathf.Lerp(_angleMin, _angleMax, fill);
            _cos = Mathf.Cos(_angleView * Mathf.PI / 360f);

            _distance = Mathf.Lerp(_distanceMin, _distanceMax, fill);
        }
        else
        {
            Debug.LogWarning("Non sei autorizzato a cambiare le impostazioni di questo EnemyView", brain);
        }
    }

    public CharacterChat[] See(EnemySenseBrain brain)
    {
        if (brain == _brain)
        {
            _meet.Clear();

            _indexNotAllocate = Physics.OverlapSphereNonAlloc(transform.position + _offset, _distance, _colliders, _layerMaskCharacter, _qti);

            if (_indexNotAllocate != 0)
            {
                for (int i = 0; i < _indexNotAllocate; i++)
                {
                    Vector3 distanceCharacterNorm = (_colliders[i].transform.position - transform.position).normalized;
                    float cosCharacterH = Vector3.Dot(distanceCharacterNorm, transform.forward);

                    //È nel cono di visione dell'Enemy
                    if (_cos < cosCharacterH)
                    {
                        if (Physics.Linecast(transform.position + _offset, _colliders[i].transform.position + _offset, out _hit, _layerMaskVisible, _qti))
                        {
                            if (_hit.collider.gameObject.layer == _layerMaskStatic)
                            {
                                Debug.Log("Il meet è dietro ad un muro");
                            }
                            else
                            {
                                CharacterChat meet = _colliders[i].GetComponent<CharacterChat>();
                                if (meet != null)
                                {
                                    _meet.Add(meet);
                                }
                            }
                        }
                    }
                }
            }
            return _meet.ToArray();
        }
        return null;
    }
}