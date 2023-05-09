using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCard : Card
{
    public override string GetName(State state)
    {
        return "Eek! A Bird!";
    }

    public override string GetDescription(State state)
    {
        return "Drain 2 of your opponent's water";
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
        return state.players[state.otherPlayer].water > 0;
    }

    public override void Action(State state, int index)
    {
        Player player = state.players[state.otherPlayer];
        player.water -= 2;
        if (player.water < 0)
        {
            player.water = 0;
        }
    }

    public override float GetVolume(State state)
    {
        return 0.4f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "Your opponent has no water to drain.";
    }

    public override string GetWarningMessage(State state)
    {
        if (state.players[state.otherPlayer].water == 1)
        {
            return "Your opponent only has 1 water to drain.";
        }
        return null;
    }
}
