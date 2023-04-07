using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentCard : Card
{
    public override string GetName()
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
        return state.turn != state.maxTurns;
    }

    public override void Action(State state, int index)
    {
        state.players[state.thisPlayer].scentTurns += 3;
    }

    public override string GetDisabledMessage()
    {
        return "This card is useless in the last turn.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }
}
