using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootCard: Card
{
    public override string GetName()
    {
        return "Root Phase";
    }

    public override int GetCost()
    {
        return 0;
    }

    public override int GetNumActions()
    {
        return base.manager.state.players[base.manager.state.thisPlayer].rootMoves;
    }

    public override bool Validation(State state, int index)
    {
        if (state.players[state.thisPlayer].rootMoves == 0 || index == -1)
        {
            return false;
        }

        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        if (state.board[index] == '-' || state.board[index] == 'W' || state.board[index] == 'P')
        {
            if (state.HasNeighbor(x, y, state.players[state.thisPlayer].root) || state.HasNeighbor(x, y, state.players[state.thisPlayer].baseRoot))
            {
                return true;
            }
        }
        return false;
    }

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        state.players[state.thisPlayer].rootMoves--;

        if (state.board[index] == 'W')
        {
            state.players[state.thisPlayer].water++;
            State.waterMap.SetTile(new Vector3Int(x, y), null);
        }
        else if (state.board[index] == 'P')
        {
            state.players[state.thisPlayer].points++;
            State.pointsMap.SetTile(new Vector3Int(x, y), null);
        }

        state.Spread(x, y, state.players[state.thisPlayer].deadRoot, state.players[state.thisPlayer].root, State.rootTile, state.players[state.thisPlayer].rootMap);
    }
}
