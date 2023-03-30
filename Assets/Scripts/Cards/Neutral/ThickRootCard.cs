using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickRootCard : Card
{
    public override string GetName()
    {
        return "Thick Root";
    }

    public override int GetCost(State state)
    {
        return 1;
    }

    public override int GetNumActions(State state)
    {
        return 1;
    }

    public override bool Validation(State state, int index)
    {
        return index != -1 && state.board[index] == state.players[state.thisPlayer].root;
    }

    public override void Action(State state, int index)
    {
        state.board[index] = state.players[state.thisPlayer].fortifiedRoot;
        int[] coords = state.IndexToCoord(index);
        State.otherMap.SetTile(new Vector3Int(coords[0], coords[1]), State.woodShieldTile);
    }
}
