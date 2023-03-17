using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizerCard: Card
{
    public override string GetName()
    {
        return "Fertilizer";
    }

    public override int GetCost()
    {
        return 1;
    }

    public override int GetNumActions()
    {
        return 0;
    }

    public override bool Validation(State state, int index)
    {
        return true;
    }

    public override void Action(State state, int index)
    {
        state.players[state.thisPlayer].rootMoves++;
    }
}
