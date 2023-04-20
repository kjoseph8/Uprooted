using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentCard : Card
{
    public override string GetName(State state)
    {
        return "Pleasant Scent";
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
        return state.players[state.thisPlayer].scentTurns < state.maxTurns - state.turn;
    }

    public override void Action(State state, int index)
    {
        state.players[state.thisPlayer].scentTurns += 3;
    }

    public override float GetVolume(State state)
    {
        return 0.6f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "You already smelled enough roses to last till the end of the game.";
    }
}
