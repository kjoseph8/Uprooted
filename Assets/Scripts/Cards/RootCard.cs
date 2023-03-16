using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootCard: Card
{
    public override string GetName()
    {
        return "Root Phase";
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Validation(int x, int y)
    {
        State state = base.manager.state;
        int index = state.CoordToIndex(x, y);

        if (index != -1 && (state.board[index] == '-' || state.board[index] == 'W' || state.board[index] == 'P'))
        {
            if (state.HasNeighbor(x, y, state.players[state.thisPlayer].root) || state.HasNeighbor(x, y, state.players[state.thisPlayer].baseRoot))
            {
                return true;
            }
        }
        return false;
    }

    public override void Action(int x, int y)
    {
        State state = base.manager.state;
        int index = state.CoordToIndex(x, y);

        if (state.board[index] == 'W')
        {
            state.players[state.thisPlayer].water++;
            State.waterMap.SetTile(new Vector3Int(x, y), null);
        }
        else if (state.board[index] == 'P')
        {
            state.players[state.thisPlayer].points++;
            State.pointsMap.SetTile(new Vector3Int(x, y), null);
        }
        state.Spread(x, y, state.players[state.thisPlayer].deadRoot, state.players[state.thisPlayer].root, State.rootTile, state.players[state.thisPlayer].rootMap);
        if (state.players[state.thisPlayer].rootMoves > 1)
        {
            state.players[state.thisPlayer].rootMoves--;
        }
        else
        {
            state.card = null;
        }
    }

    public override void SetCard()
    {
        State state = base.manager.state;
        if (state.card != null && state.card.GetName() == "Root Phase")
        {
            base.manager.ChangeTurn();
        }
        else
        {
            state.card = GameObject.FindGameObjectWithTag("RootCard").GetComponent<RootCard>();
            base.manager.ChangePhase();
            base.manager.phaseChange.GetComponent<Animator>().Play("Sweep");
        }
    }
}
