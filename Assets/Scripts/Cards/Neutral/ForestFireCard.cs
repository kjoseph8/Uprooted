using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ForestFireCard : Card
{
    public override string GetName()
    {
        return "Forest Fire";
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

        if (state.board[index] == state.players[state.otherPlayer].root)
        {
            if (state.CountNeighbors(x, y, new char[] { state.players[state.otherPlayer].root, state.players[state.otherPlayer].fortifiedRoot, state.players[state.otherPlayer].baseRoot }) < 2)
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

    public override List<int> GetValidAIMoves(State state)
    {
        List<int> validMoves = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (Validation(state, i))
            {
                validMoves.Add(i);
            }
        }
        return validMoves;
    }

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        state.board[index] = state.players[state.otherPlayer].strongFire;
        if (state.absolute)
        {
            State.otherMap.SetTile(new Vector3Int(x, y), State.strongFireTile);
        }

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
                if (state.absolute)
                {
                    state.players[state.otherPlayer].rootMap.SetTile(new Vector3Int(dirX, dirY), State.deadRootTile);
                }
                state.KillRoots(dirX, dirY, state.players[state.otherPlayer]);
            }
        }
    }

    public static void UpdateFire(State state)
    {
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (state.board[i] == state.players[state.thisPlayer].weakFire)
            {
                ForestFireCard.SpreadFire(state, i, '-', null);
            }
        }
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (state.board[i] == state.players[state.thisPlayer].strongFire)
            {
                ForestFireCard.SpreadFire(state, i, state.players[state.thisPlayer].weakFire, State.weakFireTile);
            }
        }
    }

    public static void SpreadFire(State state, int index, char fire, TileBase tile)
    {
        int[] coord = state.IndexToCoord(index);
        int x = coord[0];
        int y = coord[1];
        state.board[index] = '-';
        if (state.absolute)
        {
            state.players[0].rootMap.SetTile(new Vector3Int(x, y), null);
            state.players[1].rootMap.SetTile(new Vector3Int(x, y), null);
            State.otherMap.SetTile(new Vector3Int(x, y), null);
        }
        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = state.CoordToIndex(dirX, dirY);
            if (dirI != -1 && Array.IndexOf(new char[] { state.players[0].root, state.players[1].root, state.players[0].fortifiedRoot, state.players[1].fortifiedRoot, state.players[0].deadRoot, state.players[1].deadRoot, state.players[0].deadFortifiedRoot, state.players[1].deadFortifiedRoot, state.players[0].thorn, state.players[1].thorn }, state.board[dirI]) != -1)
            {
                SpreadFireHelper(state, dirI, fire, tile);
            }
        }
    }

    public static void SpreadFireHelper(State state, int index, char fire, TileBase tile)
    {
        int target = -1;
        if (state.board[index] == state.players[0].root)
        {
            target = 0;
        }
        else if (state.board[index] == state.players[1].root)
        {
            target = 1;
        }
        int[] coord = state.IndexToCoord(index);
        int x = coord[0];
        int y = coord[1];

        if (state.board[index] == state.players[0].fortifiedRoot)
        {
            state.board[index] = state.players[0].root;
            if (state.absolute)
            {
                State.otherMap.SetTile(new Vector3Int(x, y), null);
            }
            return;
        }
        else if (state.board[index] == state.players[1].fortifiedRoot)
        {
            state.board[index] = state.players[1].root;
            if (state.absolute)
            {
                State.otherMap.SetTile(new Vector3Int(x, y), null);
            }
            return;
        }
        else if (state.board[index] == state.players[0].deadFortifiedRoot)
        {
            state.board[index] = state.players[0].deadRoot;
            if (state.absolute)
            {
                State.otherMap.SetTile(new Vector3Int(x, y), null);
            }
            return;
        }
        else if (state.board[index] == state.players[1].deadFortifiedRoot)
        {
            state.board[index] = state.players[1].deadRoot;
            if (state.absolute)
            {
                State.otherMap.SetTile(new Vector3Int(x, y), null);
            }
            return;
        }

        state.board[index] = fire;
        if (state.absolute)
        {
            State.otherMap.SetTile(new Vector3Int(x, y), tile);
        }
        if (tile == null && state.absolute)
        {
            state.players[0].rootMap.SetTile(new Vector3Int(x, y), null);
            state.players[1].rootMap.SetTile(new Vector3Int(x, y), null);
        }
        if (target != -1)
        {
            int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
            for (int i = 0; i < 4; i++)
            {
                int dirX = dirs[i, 0];
                int dirY = dirs[i, 1];
                int dirI = state.CoordToIndex(dirX, dirY);
                if (dirI != -1 && target == 0 && (state.board[dirI] == state.players[0].root || state.board[dirI] == state.players[0].fortifiedRoot) &&
                !state.AStar(dirX, dirY, new char[] { state.players[0].root, state.players[0].fortifiedRoot, state.players[0].invincibleRoot }, state.players[0].baseRoot))
                {
                    if (state.board[dirI] == state.players[0].root)
                    {
                        state.board[dirI] = state.players[0].deadRoot;
                    }
                    else
                    {
                        state.board[dirI] = state.players[0].deadFortifiedRoot;
                    }
                    if (state.absolute)
                    {
                        state.players[0].rootMap.SetTile(new Vector3Int(dirX, dirY), State.deadRootTile);
                    }
                    state.KillRoots(dirX, dirY, state.players[0]);
                }
                else if (dirI != -1 && target == 1 && (state.board[dirI] == state.players[1].root || state.board[dirI] == state.players[1].fortifiedRoot) &&
                !state.AStar(dirX, dirY, new char[] { state.players[1].root, state.players[1].fortifiedRoot, state.players[1].invincibleRoot }, state.players[1].baseRoot))
                {
                    if (state.board[dirI] == state.players[1].root)
                    {
                        state.board[dirI] = state.players[1].deadRoot;
                    }
                    else
                    {
                        state.board[dirI] = state.players[1].deadFortifiedRoot;
                    }
                    if (state.absolute)
                    {
                        state.players[1].rootMap.SetTile(new Vector3Int(dirX, dirY), State.deadRootTile);
                    }
                    state.KillRoots(dirX, dirY, state.players[1]);
                }
            }
        }
    }

    public override string GetDisabledMessage()
    {
        return "Your opponent has no end roots to ignite.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }
}
