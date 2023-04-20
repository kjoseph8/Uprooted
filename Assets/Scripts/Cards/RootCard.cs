using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootCard: Card
{
    public override string GetName(State state)
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
        Player player = state.players[state.thisPlayer];
        if (player.rootMoves == 0 || index == -1)
        {
            return false;
        }

        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        if (state.board[index] == '-' || state.board[index] == 'W')
        {
            if (state.HasNeighbor(x, y, new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot }))
            {
                return true;
            }
        }
        return false;
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
                && state.CountNeighbors(coords[0], coords[1], new char[] { player.deadRoot, player.deadFortifiedRoot, player.deadInvincibleRoot }) > 0)
            {
                priority.Add(i);
            }
            else if (state.board[i] == 'W' || (state.board[i] == '-' && (state.CountAllNeighbors(i, new char[] { 'R' }) > 0 || state.oasisIndexes.Contains(i))))
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
        state.BFS(start, new char[] { '-' }, new char[0], "rootAI");
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

        state.players[state.thisPlayer].rootMoves--;

        if (state.board[index] == 'W')
        {
            state.players[state.thisPlayer].water += 2;
            if (state.absolute)
            {
                State.waterMap.SetTile(new Vector3Int(x, y), null);
            }
        }

        if (state.turn == state.maxTurns)
        {
            state.board[index] = state.players[state.thisPlayer].invincibleRoot;
            if (state.absolute)
            {
                State.otherMap.SetTile(new Vector3Int(x, y), State.metalShieldTile);
            }
        }
        else
        {
            state.board[index] = state.players[state.thisPlayer].root;
        }
        if (state.absolute)
        {
            state.players[state.thisPlayer].rootMap.SetTile(new Vector3Int(x, y), State.rootTile);
        }
        state.ResurrectRoots(x, y, state.players[state.thisPlayer]);
    }

    public override float GetVolume(State state)
    {
        return 1.0f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "There are no possible spaces to place new roots.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }
}
