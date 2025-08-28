using System.Collections.Generic;
using UnityEngine;
using GSM = GM.GameStaticManager;

//Senso attivo
[RequireComponent(typeof(EnemySenseBrain))]
public class EnemyView : MonoBehaviour
{
    private EnemySenseBrain _brain;
    [SerializeField] private bool _useRandom;

    private const float _distance_c = 15f;
    private const float _distanceDelta_c = 3f;
    private const float _angleView_c = 80f;
    private const float _angleDelta_c = 30f;

    private float _marginRandomAngle = 15f;
    private float _marginRandomDistance = 3f;
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
    private int _layerDefault = 0;
    private QueryTriggerInteraction _qti = QueryTriggerInteraction.Ignore;

    private List<CharacterBrain> _meet = new List<CharacterBrain>();

    [Header("Gizmos")]
    [SerializeField] private bool _useGizmos;
    private Color _orange = new Color(1f, 0.4f, 0f, 1f);
    private Vector3[] _pointsCone = new Vector3[12];
    [SerializeField] private float _angleDeltaY;

    void Awake()
    {
        _brain = GetComponent<EnemySenseBrain>();

        if (_useRandom)
        {
            _distance = Random.Range(_distance_c - _marginRandomDistance, _distance_c + _marginRandomDistance);
            _angleView = Random.Range(_angleView_c - _marginRandomAngle, _angleView_c + _marginRandomAngle);
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
            _angleView = Mathf.Lerp(_angleMax, _angleMin, fill);
            _cos = Mathf.Cos(_angleView * Mathf.PI / 360f);

            _distance = Mathf.Lerp(_distanceMin, _distanceMax, fill);
        }
        else
        {
            Debug.LogWarning("Non sei autorizzato a cambiare le impostazioni di questo EnemyView", brain);
        }
    }

    public CharacterBrain[] See(EnemySenseBrain brain)
    {
        if (brain == _brain)
        {
            _meet.Clear();

            _indexNotAllocate = Physics.OverlapSphereNonAlloc(transform.position + _offset, _distance, _colliders, _layerMaskCharacter, _qti);

            if (_indexNotAllocate != 0)
            {
                for (int i = 0; i < _indexNotAllocate; i++)
                {
                    if (_colliders[i].gameObject != gameObject)
                    {
                        Vector3 distanceCharacterNorm = (_colliders[i].transform.position - transform.position).normalized;
                        float cosCharacter = Vector3.Dot(distanceCharacterNorm, transform.forward);

                        //È nel cono di visione dell'Enemy
                        if (_cos < cosCharacter)
                        {
                            if (Physics.Linecast(transform.position + _offset, _colliders[i].transform.position + _offset, out _hit, _layerMaskVisible, _qti))
                            {
                                if (_hit.collider.gameObject.layer == _layerDefault)
                                {
                                    //Debug.Log("Il meet è dietro ad un muro");
                                }
                                else
                                {
                                    CharacterBrain meet = _colliders[i].GetComponent<CharacterBrain>();
                                    if (meet != null)
                                    {
                                        _meet.Add(meet);
                                    }
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

    //I punti devono essere calcolati ad ogni frame perchè può variare l'angolo e la distanza
    void OnDrawGizmos()
    {
        if (_useGizmos)
        {
            float angleStart = (_angleView / -2f) * GSM._myPI;
            float angleRad = (_angleView / _pointsCone.Length) * GSM._myPI;
            float x;
            float z;
            Quaternion rotation = transform.rotation * Quaternion.Euler(0f, _angleDeltaY, 0f);
            for (int i = 0; i < _pointsCone.Length; i++)
            {
                x = Mathf.Cos(angleStart + angleRad * i) * _distance;
                z = Mathf.Sin(angleStart + angleRad * i) * _distance;
                _pointsCone[i] = transform.position + _offset + rotation * new Vector3(x, 0f, z);
                if (Physics.Linecast(transform.position + _offset, _pointsCone[i], out _hit, (1 << 0), _qti))
                {
                    _pointsCone[i] = _hit.point;
                }
            }

            Gizmos.color = _orange;
            for (int i = 0; i < _pointsCone.Length - 1; i++)
            {
                Gizmos.DrawLine(_pointsCone[i], _pointsCone[i + 1]);
                Gizmos.DrawLine(transform.position + _offset, _pointsCone[i]);
            }
            Gizmos.DrawLine(transform.position + _offset, _pointsCone[_pointsCone.Length - 1]);
        }
    }
}