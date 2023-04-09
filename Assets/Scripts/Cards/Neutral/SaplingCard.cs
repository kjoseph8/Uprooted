using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaplingCard : Card
{
    public override string GetName()
    {
        return "Sapling";
    }

    public override int GetCost(State state)
    {
        return 4;
    }

    public override int GetNumActions(State state)
    {
        return 1;
    }

    public override bool Validation(State state, int index)
    {
        if (index == -1)
        {
            return false;
        }

        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        return state.board[index] == '-' && state.CountNeighbors(x, y, new char[] { '0' }) > 0;
    }

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        state.board[index] = state.players[state.thisPlayer].baseRoot;
        if (state.absolute)
        {
            state.players[state.thisPlayer].rootMap.SetTile(new Vector3Int(x, y), State.rootTile);
            State.otherMap.SetTile(new Vector3Int(x, y), State.seedTile);
        }
    }

    public override string GetDisabledMessage()
    {
        return "There are no empty tiles at the border of the map to place a new sapling.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }
}
