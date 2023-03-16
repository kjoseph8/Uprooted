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
        State state = base.manager.state;
        state.players[state.thisPlayer].water--;
        state.players[state.thisPlayer].rootMoves++;
        state.card = GameObject.FindGameObjectWithTag("RootCard").GetComponent<RootCard>();
    }

    public override void SetCard()
    {
        base.manager.state.card = this;
    }
}
