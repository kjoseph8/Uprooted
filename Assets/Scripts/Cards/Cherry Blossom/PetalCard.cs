using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalCard : Card
{
    public override string GetName(State state)
    {
        return "Petal Swarm";
    }

    public override string GetDescription(State state)
    {
        if (state.players[state.thisPlayer].bloom)
        {
            return "Draw 2 cards that cost 0 this turn";
        }
        else if (state.tilePhase)
        {
            return "Destroy an opposing root or obstacle";
        }
        else
        {
            return "Discard your hand and destroy an opposing root for each card";
        }
    }

    public override int GetCost(State state)
    {
        return 3;
    }

    public override int GetNumActions(State state)
    {
        if (state.cardIndex == 17 && state.tilePhase)
        {
            return 4;
        }
        return 0;
    }

    public override bool Validation(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        if (player.bloom)
        {
            return true;
        }
        else if (state.tilePhase)
        {
            Player otherPlayer = state.players[state.otherPlayer];

            return index != -1 && Array.IndexOf(new char[] { otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.deadRoot, otherPlayer.deadFortifiedRoot, otherPlayer.thorn }, state.board[index]) != -1;
        }
        else
        {
            return player.hand.Count > 1;
        }
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
        Player player = state.players[state.thisPlayer];
        if (player.bloom)
        {
            for (int i = 0; i < 2; i++)
            {
                player.freeCards.Add(player.hand.Count);
                player.DrawCard();
            }
        }
        else if (state.tilePhase)
        {
            state.KillRoot(index, state.players[state.otherPlayer]);
        }
        else
        {
            while (player.hand.Count > 0)
            {
                if (player.hand[0] == 18)
                {
                    player.rootMoves += 2;
                }
                player.DiscardCard(0);
                state.numActions++;
            }
            state.tilePhase = true;
        }
    }

    public override float GetVolume(State state)
    {
        return 0.4f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "There are no other cards in your hand to discard.";
    }

    public override string GetCardTips(State state)
    {
        return "Play Dormant to enter Dormant Stance.\n\nPlay Bloom to enter Bloom Stance.";
    }
}
