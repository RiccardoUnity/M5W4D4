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
    private Dictionary<string, CharacterBrain> _enemies = new Dictionary<string, CharacterBrain>();

    public string AddEnemy(CharacterBrain enemy)
    {
        string id = "";
        bool stay = true;
        while (stay)
        {
            id = Random.Range(0, 1000).ToString();
            if (!_enemies.ContainsKey(id))
            {
                stay = false;
            }
        }
        _enemies.Add(id, enemy);
        return id;
    }
    public bool RemoveEnemy(string id)
    {
        if (_enemies.ContainsKey(id))
        {
            _enemies.Remove(id);
            return true;
        }
        return false;
    }

    public bool IsEnemyIDValid(string id) => _enemies.ContainsKey(id);

    public CharacterBrain GetEnemyByID(string id)
    {
        if (IsEnemyIDValid(id))
        {
            return _enemies[id];
        }
        return null;
    }

    //Funzione molto "potente", per questo ci sono tutti questi controlli
    public bool SwitchOffAllEnemy(Enemy_FSM_Controller fsmController, string id)
    {
        if (IsEnemyIDValid(id))
        {
            if (GetEnemyByID(id).GetComponent<Enemy_FSM_Controller>() == fsmController)
            {
                if (fsmController.IsCurrentStateTake())
                {
                    _generalSwich = false;
                    foreach (CharacterBrain character in _enemies.Values)
                    {
                        character.GetComponent<Enemy_FSM_Controller>().MainSwitchOff();
                    }
                }
            }
        }
        return false;
    }

    //L'unica funzione che cambia questo valore è SwitchOffAllEnemy
    private bool _generalSwich = true;
    public bool GetGeneralSwich() => _generalSwich;
    #endregion

    #region Target
    private Vector3 _targetPosition;
    public void SetTarget(string id, Vector3 targetPosition)
    {
        if (_enemies.ContainsKey(id))
        {
            _targetPosition = targetPosition;
        }
    }
    #endregion
}
