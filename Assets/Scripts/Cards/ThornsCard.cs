using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsCard : Card
{
    public override string GetName()
    {
        return "Every Rose has its Thorn";
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Validation(int x, int y)
    {
        State state = base.manager.state;
        int index = state.CoordToIndex(x, y);

        return index != -1 && state.board[index] == '-';
    }

    public override void Action(int x, int y)
    {
        State state = base.manager.state;
        state.players[state.thisPlayer].water--;
        int index = state.CoordToIndex(x, y);
        state.board[index] = 'T';
        State.otherMap.SetTile(new Vector3Int(x, y), State.thornTile);
        state.card = GameObject.FindGameObjectWithTag("RootCard").GetComponent<RootCard>();
    }

    public override void SetCard()
    {
        base.manager.state.card = this;
    }
}
