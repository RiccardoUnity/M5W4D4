using UnityEngine;
using GM;
using System.Collections;

//Senso passivo
[RequireComponent(typeof(EnemySenseBrain))]
public class EnemyHear : MonoBehaviour
{
    private EnemySenseBrain _brain;

    private Vector3 _positionTarget;
    public Vector3 GetPositionTarget() => _positionTarget;
    private NoiseType _lastTypeNoise;
    private float _lastIntensity = 1f;

    private IEnumerator _lastListen;
    private float _coeffTime = 5f;

    void Awake()
    {
        _brain = GetComponent<EnemySenseBrain>();
    }

    public void Listen(Vector3 position, NoiseType typeNoise, float intensity)
    {
        //Vince il suono con l'indice (importanza) più alto
        if (_lastTypeNoise < typeNoise)
        {
            //Vince il suono più vicino a 0
            if (intensity < _lastIntensity)
            {
                _positionTarget = position;
                _lastTypeNoise = typeNoise;
                _lastIntensity = intensity;
                if (_lastListen == null)
                {
                    _lastListen = LastListen();
                    StartCoroutine(_lastListen);
                }
            }
        }
    }

    //Il suono sentito perde di importanza man mano che passa il tempo
    private IEnumerator LastListen()
    {
        while (_lastIntensity < 1.5f)
        {
            yield return null;
            _lastIntensity += Time.deltaTime / _coeffTime;  //Dopo _coeffTime secondi aumenta di 1
        }
        _lastTypeNoise = NoiseType.None;
        _lastListen = null;
    }

    public bool Felt() => (_lastTypeNoise != NoiseType.None)? true : false;
}
