using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AphidCard: Card
{
    public override string GetName(State state)
    {
        return "Aphid Infestation";
    }

    public override string GetDescription(State state)
    {
        return "Destroy 2 of your opponent's roots or obstacles";
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

        return index != -1 && Array.IndexOf(new char[] { player.root, player.fortifiedRoot, player.deadRoot, player.deadFortifiedRoot, player.thorn }, state.board[index]) != -1;
    }

    public override void UpdateValidAIMoves(State state)
    {
        Player player = state.players[state.thisPlayer];
        state.validAIMoves.Clear();
        bool thornExists = false;
        bool neighboringRootExists = false;
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (Validation(state, i))
            {
                int[] coords = state.IndexToCoord(i);
                if (state.board[i] == state.players[state.otherPlayer].thorn)
                {
                    if (!thornExists)
                    {
                        thornExists = true;
                        state.validAIMoves.Clear();
                    }
                    state.validAIMoves.Add(i);
                }
                else if (!thornExists && state.CountNeighbors(coords[0], coords[1], new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot }) > 0)
                {
                    if (!neighboringRootExists)
                    {
                        neighboringRootExists = true;
                        state.validAIMoves.Clear();
                    }
                    state.validAIMoves.Add(i);
                }
                else if (!thornExists && !neighboringRootExists)
                {
                    state.validAIMoves.Add(i);
                }
            }
        }
    }

    public override void Action(State state, int index)
    {
        state.KillRoot(index, state.players[state.otherPlayer]);
    }

    public override float GetVolume(State state)
    {
        return 0.7f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "Your opponent has nothing you can destroy.";
    }
}
