using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FSM_Transition : MonoBehaviour
{
    private enum Condition
    {
        None,
        Trigger,
        Float
    }

    [SerializeField] private Condition[] _conditions;

    void Awake()
    {
        if (_conditions == null)
        {
            Debug.LogError("Condizione per la transizione mancante", gameObject);
        }
    }
}
