using System;
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

    public override IEnumerator UpdateValidAIMoves(State state)
    {
        yield return null;
        state.validAIMoves.Clear();
        List<int> options1 = new List<int>();
        List<int> options2 = new List<int>();
        for (int i = 0; i <= state.boardWidth/2; i += state.boardWidth / 2)
        {
            for (int j = 0; j < state.boardWidth / 2; j++)
            {
                int index = i + j;
                if (Validation(state, index))
                {
                    options1.Add(index);
                }
                index = i + j + state.boardWidth * (state.boardHeight - 1);
                if (Validation(state, index))
                {
                    options2.Add(index);
                }
            }
            if (options1.Count > 0)
            {
                state.validAIMoves.Add(options1[new System.Random().Next(0, options1.Count)]);
            }
            options1.Clear();
            if (options2.Count > 0)
            {
                state.validAIMoves.Add(options2[new System.Random().Next(0, options2.Count)]);
            }
            options2.Clear();
        }

        for (int i = 0; i <= 2 * state.boardHeight / 3; i += state.boardHeight / 3)
        {
            for (int j = 0; j < state.boardHeight / 3; j++)
            {
                int index = state.boardWidth * (i + j);
                if (Validation(state, index))
                {
                    options1.Add(index);
                }
                index = state.boardWidth * (i + j) + state.boardWidth - 1;
                if (Validation(state, index))
                {
                    options2.Add(index);
                }
            }
            if (options1.Count > 0)
            {
                state.validAIMoves.Add(options1[new System.Random().Next(0, options1.Count)]);
            }
            options1.Clear();
            if (options2.Count > 0)
            {
                state.validAIMoves.Add(options2[new System.Random().Next(0, options2.Count)]);
            }
            options2.Clear();
        }
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
        state.ResurrectRoots(x, y, state.players[state.thisPlayer]);
    }

    public override string GetDisabledMessage()
    {
        return "There are no empty tiles at the border of the map to place a new sapling.";
    }
}
