using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsCard : Card
{
    public override string GetName()
    {
        return "Every Rose has its Thorn";
    }

    public override int GetCost(State state)
    {
        return 2;
    }

    public override int GetNumActions(State state)
    {
        return 1;
    }

    public override bool Validation(State state, int index)
    {
        return index != -1 && state.board[index] == '-';
    }

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        state.board[index] = state.players[state.thisPlayer].thorn;
        state.players[state.thisPlayer].rootMap.SetTile(new Vector3Int(x, y), State.thornTile);
    }
}
