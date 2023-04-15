using System;
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
        Player player = state.players[state.otherPlayer];

        if (state.turn == state.maxTurns)
        {
            return index != -1 && state.board[index] == player.thorn;
        }

        return index != -1 && Array.IndexOf(new char[] { player.root, player.fortifiedRoot, player.deadRoot, player.deadFortifiedRoot, player.thorn }, state.board[index]) != -1;
    }

    public override IEnumerator UpdateValidAIMoves(State state)
    {
        yield return null;
        Player player = state.players[state.thisPlayer];
        int heuristic = 0;
        if (state.thisPlayer == 0)
        {
            heuristic = -1000000000;
        }
        else
        {
            heuristic = 1000000000;
        }

        state.validAIMoves.Clear();
        List<int> neighborRootMoves = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (Validation(state, i))
            {
                bool alreadyAdded = false;
                int[] coords = state.IndexToCoord(i);
                if (state.CountNeighbors(coords[0], coords[1], new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot }) > 0)
                {
                    neighborRootMoves.Add(i);
                    alreadyAdded = true;
                }

                State next = new State(state);
                next.PlayTile(i);
                int nextHeuristic = MiniMaxAI.Heuristic(next);
                if (state.thisPlayer == 0)
                {
                    if (nextHeuristic > heuristic)
                    {
                        state.validAIMoves.Clear();
                        heuristic = nextHeuristic;
                    }
                }
                else
                {
                    if (nextHeuristic < heuristic)
                    {
                        state.validAIMoves.Clear();
                        heuristic = nextHeuristic;
                    }
                }
                if (!alreadyAdded && nextHeuristic == heuristic)
                {
                    state.validAIMoves.Add(i);
                }
            }
        }
        foreach (int index in neighborRootMoves)
        {
            state.validAIMoves.Add(index);
        }
    }

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];
        Player player = state.players[state.otherPlayer];

        if (state.board[index] == player.fortifiedRoot)
        {
            state.board[index] = player.root;
            if (state.absolute)
            {
                State.otherMap.SetTile(new Vector3Int(x, y), null);
            }
            return;
        }
        else if (state.board[index] == player.deadFortifiedRoot)
        {
            state.board[index] = player.deadRoot;
            if (state.absolute)
            {
                State.otherMap.SetTile(new Vector3Int(x, y), null);
            }
            return;
        }
        else if (state.board[index] == player.thorn)
        {
            state.board[index] = '-';
            if (state.absolute)
            {
                player.rootMap.SetTile(new Vector3Int(x, y), null);
            }
            return;
        }

        state.board[index] = '-';
        if (state.absolute)
        {
            player.rootMap.SetTile(new Vector3Int(x, y), null);
        }

        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = state.CoordToIndex(dirX, dirY);
            if (dirI != -1 && (state.board[dirI] == player.root || state.board[dirI] == player.fortifiedRoot) && 
                state.BFS(dirX, dirY, new char[] { player.root, player.fortifiedRoot, player.invincibleRoot }, new char[] { player.baseRoot }) == -1)
            {
                if (state.board[dirI] == player.root)
                {
                    state.board[dirI] = player.deadRoot;
                }
                else
                {
                    state.board[dirI] = player.deadFortifiedRoot;
                }
                if (state.absolute)
                {
                    player.rootMap.SetTile(new Vector3Int(dirX, dirY), State.deadRootTile);
                }
                state.KillRoots(dirX, dirY, state.players[state.otherPlayer]);
            }
        }
    }

    public override string GetDisabledMessage()
    {
        return "Your opponent has nothing you can destroy.";
    }
}
