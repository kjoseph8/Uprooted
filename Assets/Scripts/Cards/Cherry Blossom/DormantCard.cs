using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DormantCard : Card
{
    public override string GetName(State state)
    {
        return "Dormant";
    }

    public override string GetDescription(State state)
    {
        if (state.players[state.thisPlayer].bloom)
        {
            return "Switch to Dormant stance";
        }
        else
        {
            return "Randomly discard 3 of your opponent's cards";
        }
    }

    public override int GetCost(State state)
    {
        return 1;
    }

    public override int GetNumActions(State state)
    {
        return 0;
    }

    public override bool Validation(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        if (player.bloom)
        {
            return true;
        }
        else
        {
            return state.players[state.otherPlayer].hand.Count > 0;
        }
    }

    public override void Action(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        if (player.bloom)
        {
            player.bloom = false;
            if (state.absolute)
            {
                player.plantSprite.sprite = player.dormantSprite;
            }
            player.water += player.festivals;
        }
        else
        {
            Player otherPlayer = state.players[state.otherPlayer];
            for (int i = 0; i < 3; i++)
            {
                if (otherPlayer.hand.Count > 0)
                {
                    otherPlayer.DiscardCard(new System.Random().Next(0, otherPlayer.hand.Count));
                }
            }
        }
    }

    public override float GetVolume(State state)
    {
        return 0.7f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "Your opponent has no cards in their hand to discard";
    }

    public override string GetWarningMessage(State state)
    {
        if (!state.players[state.thisPlayer].bloom)
        {
            Player player = state.players[state.otherPlayer];
            if (player.hand.Count == 1)
            {
                return "Your opponent only has 1 card in their hand to discard.";
            }
            else if (player.hand.Count == 2)
            {
                return "Your opponent only has 2 cards in their hand to discard.";
            }
        }
        return null;
    }

    public override string GetCardTips(State state)
    {
        return "Play Bloom to enter Bloom Stance.";
    }
}
