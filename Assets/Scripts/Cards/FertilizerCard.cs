using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizerCard: Card
{
    public bool isInstant()
    {
        return true;
    }

    public bool Validation(int x, int y)
    {
        return true;
    }

    public void Action(int x, int y)
    {
        State.players[State.player].water--;
        State.players[State.player].rootMoves++;
        State.card = new RootCard();
    }
}
