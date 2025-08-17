using System.Collections.Generic;
using UnityEngine;

//Qui gli Enemy si scambiano informazioni
public class EnemySceneManager : MonoBehaviour
{
    #region Singleton
    private static int _count;
    private static EnemySceneManager _instance;
    public static EnemySceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("EnemySceneManager - " + _count.ToString());
                Debug.Log(gameObject.name + " creato");
                gameObject.AddComponent<EnemySceneManager>();
                _count++;
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Debug.Log(gameObject.name + " assegnato");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log(gameObject.name + " distrutto");
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _count = 0;
        }
        else
        {
            if (transform.parent != null)
            {
                _instance.transform.parent = transform.parent;
            }
        }
    }
    #endregion

    #region Enemy
    //Volevo simulare un semplicissimo sistema di accesso ...
    private Dictionary<int, CharacterChat> _enemies = new Dictionary<int, CharacterChat>();

    public int AddEnemy(CharacterChat enemy)
    {
        int id = 0;
        bool stay = true;
        while (stay)
        {
            id = Random.Range(0, 10000);
            if (!_enemies.ContainsKey(id))
            {
                stay = false;
            }
        }
        _enemies.Add(id, enemy);
        return id;
    }
    public bool RemoveEnemy(int id)
    {
        if (_enemies.ContainsKey(id))
        {
            _enemies.Remove(id);
            return true;
        }
        return false;
    }
    #endregion

    #region Target
    private Vector3 _targetPosition;
    public void SetTarget(int id, Vector3 targetPosition)
    {
        if (_enemies.ContainsKey(id))
        {
            _targetPosition = targetPosition;
        }
    }
    #endregion
}
