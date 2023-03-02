using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsCard : Card
{
    public override string GetName()
    {
        return "thorns";
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Validation(int x, int y)
    {
        int index = State.CoordToIndex(x, y);

        return index != -1 && State.state[index] == '-';
    }

    public override void Action(int x, int y)
    {
        State.players[State.player].water--;
        int index = State.CoordToIndex(x, y);
        State.state[index] = 'T';
        State.otherMap.SetTile(new Vector3Int(x, y), State.thornTile);
        State.card = new RootCard();
    }
}
