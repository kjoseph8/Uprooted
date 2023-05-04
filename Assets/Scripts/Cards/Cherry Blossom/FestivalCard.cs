using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FestivalCard : Card
{
    public override string GetName(State state)
    {
        return "Enjoy the Cherry Blossom Festival!";
    }

    public override int GetCost(State state)
    {
        return 3;
    }

    public override int GetNumActions(State state)
    {
        return 0;
    }

    public override bool Validation(State state, int index)
    {
        return true;
    }

    public override void Action(State state, int index)
    {
        state.players[state.thisPlayer].festivals++;
    }

    public override float GetVolume(State state)
    {
        return 0.5f;
    }

    public override string GetCardTips(State state)
    {
        return "Play Dormant to enter Dormant Stance.\n\nPlay Bloom to enter Bloom Stance.";
    }
}
