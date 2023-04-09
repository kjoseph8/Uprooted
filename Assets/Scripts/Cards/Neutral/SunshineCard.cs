using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshineCard : Card
{
    public override string GetName()
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
        player.hand.Add(player.discard[0]);
        player.discard.RemoveAt(0);

        if (player.discard.Count > 1)
        {
            player.hand.Add(player.discard[0]);
            player.discard.RemoveAt(0);
        }
    }

    public override string GetDisabledMessage()
    {
        return "There are no cards in your discard pile.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }
}
