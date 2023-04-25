using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ForestFireCard : Card
{
    public override string GetName(State state)
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
        if (index == -1 || state.oasisIndexes.Contains(index))
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

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];
        Player player = state.players[state.otherPlayer];

        state.KillRoot(index, player);
        state.board[index] = player.strongFire;
        if (state.absolute)
        {
            player.rootMap.SetTile(new Vector3Int(x, y), State.rootTile);
            State.otherMap.SetTile(new Vector3Int(x, y), State.strongFireTile);
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
            if (dirI != -1)
            {
                if (Array.IndexOf(new char[] { state.players[0].root, state.players[1].root, state.players[0].deadRoot, state.players[1].deadRoot, state.players[0].thorn, state.players[1].thorn }, state.board[dirI]) != -1)
                {
                    SpreadFireHelper(state, dirI, fire, tile);
                }
                else if (Array.IndexOf(new char[] { state.players[0].fortifiedRoot, state.players[0].deadFortifiedRoot }, state.board[dirI]) != -1)
                {
                    state.KillRoot(dirI, state.players[0]);
                }
                else if (Array.IndexOf(new char[] { state.players[1].fortifiedRoot, state.players[1].deadFortifiedRoot }, state.board[dirI]) != -1)
                {
                    state.KillRoot(dirI, state.players[1]);
                }
            }
        }
    }

    public static void SpreadFireHelper(State state, int index, char fire, TileBase tile)
    {
        int target = -1;
        bool isThorn = false;
        if (Array.IndexOf(new char[] { state.players[0].root, state.players[0].deadRoot }, state.board[index]) != -1)
        {
            target = 0;
        }
        else if (Array.IndexOf(new char[] { state.players[1].root, state.players[1].deadRoot }, state.board[index]) != -1)
        {
            target = 1;
        }
        else if (state.board[index] == state.players[0].thorn)
        {
            target = 0;
            isThorn = true;
        }
        else if (state.board[index] == state.players[1].thorn)
        {
            target = 1;
            isThorn = true;
        }

        if (target != -1)
        {
            state.KillRoot(index, state.players[target]);

            int[] coord = state.IndexToCoord(index);
            int x = coord[0];
            int y = coord[1];

            if (state.absolute)
            {
                if (isThorn)
                {
                    state.players[target].rootMap.SetTile(new Vector3Int(x, y), State.thornTile);
                }
                else
                {
                    state.players[target].rootMap.SetTile(new Vector3Int(x, y), State.rootTile);
                }
            }

            state.board[index] = fire;
            if (state.absolute)
            {
                State.otherMap.SetTile(new Vector3Int(x, y), tile);
                if (tile == null)
                {
                    state.players[target].rootMap.SetTile(new Vector3Int(x, y), null);
                }
            }
        }
    }

    public override float GetVolume(State state)
    {
        return 1.0f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "Your opponent has no end roots to ignite.";
    }
}
