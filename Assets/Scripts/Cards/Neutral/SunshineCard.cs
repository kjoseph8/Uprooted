using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshineCard : Card
{
    public override string GetName(State state)
    {
        return "Ray of Sunshine";
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
        return state.players[state.thisPlayer].discard.Count > 0;
    }

    public override void Action(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        int card = player.discard[new System.Random().Next(0, player.discard.Count - 1)];
        player.hand.Add(card);
        player.discard.Remove(card);
        if (player.discard.Count > 1)
        {
            card = player.discard[new System.Random().Next(0, player.discard.Count - 1)];
            player.hand.Add(card);
            player.discard.Remove(card);
        }
    }

    public override float GetVolume(State state)
    {
        return 0.5f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "There are no cards in your discard pile.";
    }

    public override string GetWarningMessage(State state)
    {
        if (state.players[state.thisPlayer].discard.Count == 1)
        {
            return "You only have 1 card in your discard pile to draw.";
        }
        return null;
    }
}
