using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private Transform[] _transforms;

    void Awake()
    {
        if (_transforms == null)
        {
            Debug.LogError("Nessuna posizione inserita nello script Path", gameObject);
        }
        else if (_transforms.Length < 2)
        {
            Debug.LogError("2 posizione minime da inserire nello script Path", gameObject);
        }
    }

    public Vector3 GetCurrentPoint(int index) => _transforms[index].position;

    public Vector3 GetNextPoint(ref int index, bool inverse)
    {
        if (inverse)
        {
            index = (index - 1 > 0) ? _transforms.Length : index - 1;
        }
        else
        {
            index = (index + 1 == _transforms.Length) ? 0 : index + 1;
        }
        return _transforms[index].position;

    }

    public Vector3 GetRandomPoint(int escludeIindex)
    {
        int randomIndex = 0;
        while (randomIndex != escludeIindex)
        {
            randomIndex = Random.Range(0, _transforms.Length);
        }
        return _transforms[randomIndex].position;
    }

    public Vector3 GetClosestPoint(Vector3 position)
    {
        float distanceSqr = float.MaxValue;
        float currentDistance;
        Vector3 closest = transform.position;
        foreach (Transform transform in _transforms)
        {
            currentDistance = (transform.position - position).sqrMagnitude;
            if (currentDistance < distanceSqr)
            {
                distanceSqr = currentDistance;
                closest = transform.position;
            }
        }
        return closest;
    }
}
