using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestFireCard : Card
{
    public override string GetName()
    {
        return "Forest Fire";
    }

    public override int GetCost()
    {
        return 3;
    }

    public override int GetNumActions()
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

        if (state.board[index] == state.players[state.otherPlayer].root)
        {
            if (state.CountNeighbors(x, y, state.players[state.otherPlayer].root) < 2)
            {
                return true;
            }
        }
        else if (state.board[index] == state.players[state.otherPlayer].deadRoot)
        {
            if (state.CountNeighbors(x, y, state.players[state.otherPlayer].deadRoot) < 2)
            {
                return true;
            }
        }
        return state.board[index] == 'T';
    }
    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        state.board[index] = state.players[state.otherPlayer].strongFire;
        state.players[state.otherPlayer].strongFireIndex.Add(index);
        state.players[state.otherPlayer].rootMap.SetTile(new Vector3Int(x, y), null);
        State.otherMap.SetTile(new Vector3Int(x, y), State.strongFireTile);

        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = state.CoordToIndex(dirX, dirY);
            if (dirI != -1 && state.board[dirI] == state.players[state.otherPlayer].root &&
                !state.AStar(dirX, dirY, state.players[state.otherPlayer].root, state.players[state.otherPlayer].baseRoot))
            {
                state.Spread(dirX, dirY, state.players[state.otherPlayer].root, state.players[state.otherPlayer].deadRoot, State.deadRootTile, state.players[state.otherPlayer].rootMap);
            }
        }
    }
}
