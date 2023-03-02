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
        int index = State.CoordToIndex(x, y);

        if (index != -1 && (State.state[index] == '-' || State.state[index] == 'W' || State.state[index] == 'P'))
        {
            if (State.HasNeighbor(x, y, State.players[State.player].root) || State.HasNeighbor(x, y, State.players[State.player].baseRoot))
            {
                return true;
            }
        }
        return false;
    }

    public override void Action(int x, int y)
    {
        int index = State.CoordToIndex(x, y);

        if (State.state[index] == 'W')
        {
            State.players[State.player].water++;
            State.waterMap.SetTile(new Vector3Int(x, y), null);
        }
        else if (State.state[index] == 'P')
        {
            State.players[State.player].points++;
            State.pointsMap.SetTile(new Vector3Int(x, y), null);
        }
        State.Spread(x, y, State.players[State.player].deadRoot, State.players[State.player].root, State.rootTile, State.players[State.player].rootMap);
        if (State.players[State.player].rootMoves > 1)
        {
            State.players[State.player].rootMoves--;
        }
        else
        {
            State.card = null;
        }
    }
}
