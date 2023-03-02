using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizerCard: Card
{
    public override string GetName()
    {
        return "Fertilizer";
    }

    public override bool IsInstant()
    {
        return true;
    }

    public override bool Validation(int x, int y)
    {
        return true;
    }

    public override void Action(int x, int y)
    {
        State.players[State.player].water--;
        State.players[State.player].rootMoves++;
        State.card = new RootCard();
    }
}
