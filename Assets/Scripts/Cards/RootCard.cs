using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootCard: Card
{
    public override string GetName()
    {
        return "Place a Root";
    }

    public override int GetCost(State state)
    {
        return 0;
    }

    public override int GetNumActions(State state)
    {
        return state.players[state.thisPlayer].rootMoves;
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

        if (state.board[index] == '-' || state.board[index] == 'W')
        {
            if (state.HasNeighbor(x, y, new char[] { state.players[state.thisPlayer].root, state.players[state.thisPlayer].fortifiedRoot, state.players[state.thisPlayer].baseRoot }))
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

        if (state.turn == state.maxTurns)
        {
            state.board[index] = state.players[state.thisPlayer].fortifiedRoot;
            State.otherMap.SetTile(new Vector3Int(x, y), State.shieldTile);
        }
        else
        {
            state.board[index] = state.players[state.thisPlayer].root;
        }
        state.players[state.thisPlayer].rootMap.SetTile(new Vector3Int(x, y), State.rootTile);
        state.ResurrectRoots(x, y, state.players[state.thisPlayer]);
    }
}
