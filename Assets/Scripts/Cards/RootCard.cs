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

    public override bool AIValidation(State state)
    {
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (Validation(state, i))
            {
                return true;
            }
        }
        return false;
    }

    public override IEnumerator UpdateValidAIMoves(State state)
    {
        Player player = state.players[state.thisPlayer];
        state.validAIMoves.Clear();
        int maxDist = 1000000000;
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);

            if (Validation(state, i))
            {
                int dist = -1;
                if (state.HasAnyNeighbor(i, new char[] { 'R' }) || state.HasNeighbor(coords[0], coords[1], new char[] { player.deadRoot, player.deadFortifiedRoot, player.deadInvincibleRoot }))
                {
                    dist = 0;
                }
                else
                {
                    yield return null;
                    dist = state.BFS(coords[0], coords[1], new char[] { '-' }, new char[] { player.deadRoot, player.deadFortifiedRoot, player.deadInvincibleRoot, 'W', 'R' });
                }
                if (dist == -1)
                {
                    dist = 1000000000;
                }
                if (dist < maxDist)
                {
                    state.validAIMoves.Clear();
                    maxDist = dist;
                }
                if (dist == maxDist)
                {
                    state.validAIMoves.Add(i);
                }
            }
        }
        if (maxDist == 1000000000 && state.validAIMoves.Count > 0)
        {
            int index = state.validAIMoves[new System.Random().Next(0, state.validAIMoves.Count)];
            state.validAIMoves.Clear();
            state.validAIMoves.Add(index);
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

    public override string GetDisabledMessage()
    {
        return "There are no possible spaces to place new roots.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }
}
