using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AphidCard: Card
{
    public override string GetName()
    {
        return "Aphid Infestation";
    }

    public override int GetCost(State state)
    {
        return 2;
    }

    public override int GetNumActions(State state)
    {
        return 2;
    }

    public override bool Validation(State state, int index)
    {
        return index != -1 && (state.board[index] == state.players[state.otherPlayer].root || state.board[index] == state.players[state.otherPlayer].deadRoot || state.board[index] == state.players[state.otherPlayer].thorn);
    }

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        state.board[index] = '-';
        state.players[state.otherPlayer].rootMap.SetTile(new Vector3Int(x, y), null);

        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = state.CoordToIndex(dirX, dirY);
            if (dirI != -1 && (state.board[dirI] == state.players[state.otherPlayer].root || state.board[dirI] == state.players[state.otherPlayer].fortifiedRoot) && 
                !state.AStar(dirX, dirY, new char[] { state.players[state.otherPlayer].root, state.players[state.otherPlayer].fortifiedRoot }, state.players[state.otherPlayer].baseRoot))
            {
                if (state.board[dirI] == state.players[state.otherPlayer].root)
                {
                    state.board[dirI] = state.players[state.otherPlayer].deadRoot;
                }
                else
                {
                    state.board[dirI] = state.players[state.otherPlayer].deadFortifiedRoot;
                }
                state.players[state.otherPlayer].rootMap.SetTile(new Vector3Int(dirX, dirY), State.deadRootTile);
                state.KillRoots(dirX, dirY, state.players[state.otherPlayer]);
            }
        }
    }
}
