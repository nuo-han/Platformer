using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [SerializeField] PlayerState[] states;

    Animator animator;

    PlayerController player;

    PlayerInput input;

    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        player = GetComponentInChildren<PlayerController>();
        input = GetComponent<PlayerInput>();
        stateTable = new Dictionary<System.Type, IState>(states.Length);

        // ��ʼ�����״̬
        foreach (var state in states)
        {
            state.Initialize(this, player, input, animator);
            stateTable.Add(state.GetType(), state);
        }
    }

    public void Start()
    {
        SwitchOn(stateTable[typeof(PlayerIdle)]);
    }
}