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
        }
    }
    #endregion

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
            Debug.LogError("Ci sono pi� GameObject che si spacciano per il Player", player.gameObject);
            Destroy(player.gameObject);
        }
    }

    private List<Interactable> _interactable = new List<Interactable>();
    public void AddInteractable(Interactable interactable) => _interactable.Add(interactable);
    public void RemoveInteractable(Interactable interactable) => _interactable?.Remove(interactable);

    private bool _isGraphicRayCollider;
    public bool GetIsGraphicRayCollider() => _isGraphicRayCollider;

    void Update()
    {
        _isGraphicRayCollider = false;
        foreach (Interactable interactable in _interactable)
        {
            if (interactable.gameObject.activeSelf && interactable.GetCustomButton().GetIsPointerEnter())
            {
                _isGraphicRayCollider = true;
                break;
            }
        }
    }
}
