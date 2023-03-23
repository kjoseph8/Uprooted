using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseWarCard : Card
{
    public override string GetName()
    {
        return "War of the Roses";
    }

    public override int GetCost(State state)
    {
        return 4;
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
        List<int> thornIndexes = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (state.board[i] == state.players[state.thisPlayer].thorn)
            {
                thornIndexes.Add(i);
            }
        }
        if (thornIndexes.Count == 0)
        {
            state.numActions = 4;
            state.card = new ThornsCard();
        }
        else
        {
            foreach(int i in thornIndexes)
            {
                int[] coord = state.IndexToCoord(i);
                int x = coord[0];
                int y = coord[1];
                int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
                for (int j = 0; j < 4; j++)
                {
                    int dirX = dirs[j, 0];
                    int dirY = dirs[j, 1];
                    int dirI = state.CoordToIndex(dirX, dirY);
                    if (dirI != -1 && Array.IndexOf(new char[] { state.players[state.otherPlayer].root, state.players[state.otherPlayer].deadRoot, state.players[state.otherPlayer].thorn}, state.board[dirI]) != -1)
                    {
                        ForestFireCard.SpreadFireHelper(state, dirI, '-', null);
                    }
                }
            }
        }
    }
}
