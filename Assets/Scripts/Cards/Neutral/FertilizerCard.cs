using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizerCard: Card
{
    public override string GetName(State state)
    {
        return "Fertilizer";
    }

    public override string GetDescription(State state)
    {
        return "Gain an extra root to place this turn";
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
        return true;
    }

    public override void Action(State state, int index)
    {
        state.players[state.thisPlayer].rootMoves++;
    }

    public override float GetVolume(State state)
    {
        return 1.0f;
    }
}
