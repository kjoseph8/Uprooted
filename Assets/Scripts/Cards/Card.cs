using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card: MonoBehaviour
{
    [HideInInspector] public GameManager manager;

    public void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public abstract string GetName();

    public abstract int GetCost();

    public abstract int GetNumActions();

    public abstract bool Validation(State state, int index);

    public abstract void Action(State state, int index);

    public void SetCard()
    {
        State state = manager.state;
        state.card = this;
        state.numActions = GetNumActions();
        state.players[state.thisPlayer].water -= GetCost();
    }
}
