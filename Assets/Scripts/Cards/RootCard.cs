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

    public override IEnumerator UpdateValidAIMoves(State state)
    {
        yield return null;
        Player player = state.players[state.thisPlayer];
        state.validAIMoves.Clear();
        Queue<int> queue = new Queue<int>();
        Queue<int> dists = new Queue<int>();
        List<int> visited = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);
            if ((state.board[i] == '-' || state.board[i] == 'W')
                && state.CountNeighbors(coords[0], coords[1], new char[] { player.deadRoot, player.deadFortifiedRoot, player.deadInvincibleRoot }) > 0)
            {
                queue.Enqueue(i);
                dists.Enqueue(0);
            }
            else if (state.board[i] == 'W' || (state.board[i] == '-' && state.CountAllNeighbors(i, new char[] { 'R' }) > 0))
            {
                queue.Enqueue(i);
                dists.Enqueue(1);
            }
        }

        int minDist = -1; ;
        while (queue.Count != 0)
        {
            int i = queue.Dequeue();
            int dist = dists.Dequeue();
            if (minDist != -1 && dist > minDist)
            {
                break;
            }
            else if (Validation(state, i))
            {
                minDist = dist;
                state.validAIMoves.Add(i);
            }
            else if (minDist == -1)
            {
                int[] coords = state.IndexToCoord(i);
                int x = coords[0];
                int y = coords[1];
                int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
                for (int j = 0; j < 4; j++)
                {
                    x = dirs[j, 0];
                    y = dirs[j, 1];
                    int index = state.CoordToIndex(x, y);
                    if (index != -1 && (state.board[index] == '-' || state.board[index] == 'W') && !visited.Contains(index))
                    {
                        queue.Enqueue(index);
                        dists.Enqueue(dist + 1);
                        visited.Add(index);
                    }
                }
            }
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
