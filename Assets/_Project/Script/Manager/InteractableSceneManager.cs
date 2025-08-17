using System.Collections.Generic;
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

    #region Player
    private PlayerManager _player;
    public PlayerManager GetPlayer() => _player;
    public void SetPlayer(PlayerManager player)
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
    #endregion

    #region Interactable
    private List<Interactable> _interactables = new List<Interactable>();
    public void AddInteractable(Interactable interactable) => _interactables.Add(interactable);
    public void RemoveInteractable(Interactable interactable) => _interactables?.Remove(interactable);

    private bool _isGraphicRayCollider;
    public bool GetIsGraphicRayCollider() => _isGraphicRayCollider;

    void Update()
    {
        _isGraphicRayCollider = false;
        foreach (Interactable interactable in _interactables)
        {
            if (interactable.gameObject.activeSelf && interactable.GetCustomButton().GetIsPointerEnter())
            {
                _isGraphicRayCollider = true;
                break;
            }
        }
    }
    #endregion
}
