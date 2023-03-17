using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsCard : Card
{
    public override string GetName()
    {
        return "Every Rose has its Thorn";
    }

    public override int GetCost()
    {
        return 2;
    }

    public override int GetNumActions()
    {
        return 2;
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

        state.board[index] = 'T';
        State.otherMap.SetTile(new Vector3Int(x, y), State.thornTile);
    }
}
