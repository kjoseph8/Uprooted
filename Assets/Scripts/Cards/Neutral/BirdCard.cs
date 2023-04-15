using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCard : Card
{
    public override string GetName()
    {
        return "Eek! A Bird!";
    }

    public override int GetCost(State state)
    {
        return 1;
    }

    public override int GetNumActions(State state)
    {
        return 0;
    }

    public override bool Validation(State state, int index)
    {
        return state.players[state.otherPlayer].water > 0;
    }

    public override void Action(State state, int index)
    {
        state.players[state.otherPlayer].water -= 2;
        if (state.players[state.otherPlayer].water < 0)
        {
            state.players[state.otherPlayer].water = 0;
        }
    }

    public override string GetDisabledMessage()
    {
        return "Your opponent has no water to reduce.";
    }
}
