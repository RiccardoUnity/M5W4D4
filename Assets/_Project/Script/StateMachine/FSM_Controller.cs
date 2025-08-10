using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Controller : MonoBehaviour
{
    private FSM_BaseState[] _states;

    void Awake()
    {
        _states = GetComponents<FSM_BaseState>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
