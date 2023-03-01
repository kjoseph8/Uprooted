using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootCard: Card
{
    private char[] block = { '0', '1', '!', 'I', '2', '@', 'Z' };

    public bool isInstant()
    {
        return false;
    }

    public bool Validation(int x, int y)
    {
        int index = State.CoordToIndex(x, y);

        if (index != -1 && !Array.Exists(block, element => element == State.state[index]))
        {
            if (State.HasNeighbor(x, y, State.players[State.player].root) || State.HasNeighbor(x, y, State.players[State.player].baseRoot))
            {
                return true;
            }
        }
        return false;
    }

    public void Action(int x, int y)
    {
        int index = State.CoordToIndex(x, y);

        if (State.state[index] == 'W')
        {
            State.players[State.player].water++;
            State.waterMap.SetTile(new Vector3Int(x, y), null);
        }
        else if (State.state[index] == 'P')
        {
            State.players[State.player].points++;
            State.pointsMap.SetTile(new Vector3Int(x, y), null);
        }
        reviveBranch(x, y);
        if (State.players[State.player].rootMoves > 1)
        {
            State.players[State.player].rootMoves--;
        }
        else
        {
            if (State.player == 1)
            {
                State.turn++;
            }
            State.player = (State.player + 1) % 2;
            State.card = null;
        }
    }

    private void reviveBranch(int x, int y)
    {
        int index = State.CoordToIndex(x, y);
        State.state[index] = State.players[State.player].root;
        State.players[State.player].rootMap.SetTile(new Vector3Int(x, y), State.rootTile);
        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = State.CoordToIndex(dirX, dirY);
            if (dirI != -1 && State.state[dirI] == State.players[State.player].deadRoot)
            {
                reviveBranch(dirX, dirY);
            }
        }
    }
}
