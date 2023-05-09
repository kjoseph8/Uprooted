using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaplingCard : Card
{
    public override string GetName(State state)
    {
        return "Sapling";
    }

    public override string GetDescription(State state)
    {
        return "Place a sapling at the edge of the board";
    }

    public override int GetCost(State state)
    {
        return 3;
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

    public override void UpdateValidAIMoves(State state)
    {
        Player player = state.players[state.thisPlayer];
        state.validAIMoves.Clear();
        List<int> priority = new List<int>();
        List<int> otherTarget = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);
            if ((state.board[i] == '-' || state.board[i] == 'W')
                && state.HasNeighbor(coords[0], coords[1], new char[] { player.deadRoot, player.deadFortifiedRoot, player.deadInvincibleRoot }))
            {
                priority.Add(i);
            }
            else if (state.board[i] == 'W' || (state.board[i] == '-' && state.CountAllNeighbors(i, new char[] { 'R' }) > 0))
            {
                otherTarget.Add(i);
            }
        }
        List<int> start = new List<int>();
        foreach (int i in priority)
        {
            start.Add(i);
        }
        foreach (int i in otherTarget)
        {
            start.Add(i);
        }
        state.BFS(start, new char[] { '-' }, new char[0], "saplingAI", 7);
        if (state.validAIMoves.Count == 0)
        {
            base.UpdateValidAIMoves(state);
        }
    }

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];
        Player player = state.players[state.thisPlayer];

        state.PlaceRoot(index, player);
        state.board[index] = player.baseRoot;
        if (state.absolute)
        {
            State.otherMap.SetTile(new Vector3Int(x, y), State.seedTile);
        }
    }

    public override float GetVolume(State state)
    {
        return 0.5f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "There are no empty tiles at the border of the map to place a new sapling.";
    }

    public override string GetCardTips(State state)
    {
        return "Sapling: An indestructible base that you can place roots from.";
    }
}
