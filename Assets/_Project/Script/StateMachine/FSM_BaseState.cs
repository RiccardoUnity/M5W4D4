using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class FSM_BaseState
{
    [SerializeField] private FSM_Transition[] _transitions;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
