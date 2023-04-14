using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCard : Card
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
        Player thisPlayer = state.players[state.thisPlayer];
        Player otherPlayer = state.players[state.otherPlayer];

        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coord = state.IndexToCoord(i);
            int x = coord[0];
            int y = coord[1];

            if (state.board[i] == thisPlayer.thorn && state.CountNeighbors(x, y, new char[] { otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.deadRoot, otherPlayer.deadFortifiedRoot, otherPlayer.thorn }) > 0)
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
        List<int> thornIndexes = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (state.board[i] == state.players[state.thisPlayer].thorn)
            {
                thornIndexes.Add(i);
            }
        }
        foreach (int i in thornIndexes)
        {
            int[] coord = state.IndexToCoord(i);
            int x = coord[0];
            int y = coord[1];
            Player player = state.players[state.otherPlayer];
            int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
            for (int j = 0; j < 4; j++)
            {
                int dirX = dirs[j, 0];
                int dirY = dirs[j, 1];
                int dirI = state.CoordToIndex(dirX, dirY);
                if (dirI != -1 && Array.IndexOf(new char[] { player.root, player.fortifiedRoot, player.deadRoot, player.deadFortifiedRoot, player.thorn }, state.board[dirI]) != -1)
                {
                    ForestFireCard.SpreadFireHelper(state, dirI, '-', null);
                }
            }
        }
    }

    public override string GetDisabledMessage()
    {
        return "None of your thorn blocks can destroy anything.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        int[] coord = state.IndexToCoord(index);
        int x = coord[0];
        int y = coord[1];
        Player thisPlayer = state.players[state.thisPlayer];
        Player otherPlayer = state.players[state.otherPlayer];
        return index != -1 && Array.IndexOf(new char[] { otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.deadRoot, otherPlayer.deadFortifiedRoot, otherPlayer.thorn }, state.board[index]) != -1 && state.CountNeighbors(x, y, new char[] { thisPlayer.thorn }) > 0;
    }
}
