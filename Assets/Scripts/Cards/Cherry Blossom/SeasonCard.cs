using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonCard : Card
{
    public override string GetName(State state)
    {
        return "A Seasonal Cycle";
    }

    public override string GetDescription(State state)
    {
        if (state.players[state.thisPlayer].bloom)
        {
            return "Select roots to destroy to gain 2 roots";
        }
        else
        {
            return "Gain 2 roots";
        }
    }

    public override int GetCost(State state)
    {
        return 2;
    }

    public override int GetNumActions(State state)
    {
        if (state.players[state.thisPlayer].bloom)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public override bool Validation(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        if (player.bloom)
        {
            return index != -1 && Array.IndexOf(new char[] { player.root, player.fortifiedRoot, player.deadRoot, player.deadFortifiedRoot }, state.board[index]) != -1;
        }
        else
        {
            return true;
        }
    }

    public override void UpdateValidAIMoves(State state)
    {
        Player player = state.players[state.thisPlayer];
        state.validAIMoves.Clear();
        if (!player.bloom)
        {
            return;
        }
        int minLoss = 100;
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (Validation(state, i))
            {
                int loss = 0;
                if (state.board[i] == player.root || state.board[i] == player.deadRoot)
                {
                    loss += 10;
                }
                if (state.board[i] == player.root || state.board[i] == player.fortifiedRoot)
                {
                    loss += 5;
                }
                int[] coords = state.IndexToCoord(i);
                loss += state.CountNeighbors(coords[0], coords[1], new char[] { player.root, player.fortifiedRoot, player.baseRoot, player.deadRoot, player.deadFortifiedRoot });

                if (loss < minLoss)
                {
                    state.validAIMoves.Clear();
                    minLoss = loss;
                }
                if (loss == minLoss)
                {
                    state.validAIMoves.Add(i);
                }
            }
        }
    }

    public override void Action(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        if (player.bloom)
        {
            state.KillRoot(index, player);
            player.rootMoves += 2;
        }
        else
        {
            player.rootMoves += 2;
        }
    }

    public override float GetVolume(State state)
    {
        return 0.4f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "You have no roots that you can destroy.";
    }

    public override string GetCardTips(State state)
    {
        return "Play Dormant to enter Dormant Stance.\n\nPlay Bloom to enter Bloom Stance.";
    }
}
