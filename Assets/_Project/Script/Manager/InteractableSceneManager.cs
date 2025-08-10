using UnityEngine;

public class InteractableSceneManager : MonoBehaviour
{
    #region Singleton
    private static int _count;
    private static InteractableSceneManager _instance;
    public static InteractableSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("InteractableSceneManager - " + _count.ToString());
                Debug.Log(gameObject.name + " creato");
                gameObject.AddComponent<InteractableSceneManager>();
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
        }
    }
    #endregion

    private Transform _player;
    public Transform GetPlayer() => _player;
    public void SetPlayer(Transform player)
    {
        if (_player == null)
        {
            _player = player;
            Debug.Log(gameObject.name + " Player registrato");
        }
        else
        {
            Debug.LogError("Ci sono più GameObject che si spacciano per il Player", player.gameObject);
            Destroy(player.gameObject);
        }
    }

    //aggiungere lista di interactable per poi confrontare il graphicRaycast per evitare di colpire la NavMesh
}
