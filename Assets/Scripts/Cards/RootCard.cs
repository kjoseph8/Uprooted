using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootCard: Card
{

    public static string type = "click";
    private static char[] block = { '0', '1', '!', 'I', '2', '@', 'Z' };

    public static bool Validation(int x, int y)
    {
        int index = State.CoordToIndex(x, y);

        if (index != -1 && !Array.Exists(block, element => element == State.state[index]))
        {
            if (State.HasNeighbor(x, y, State.players[State.player].root) || State.HasNeighbor(x, y, State.players[State.player].baseRoot))
            {
                return true;
            }
        }
        return false;
    }

    public static void Action(int x, int y)
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
        State.state[index] = State.players[State.player].root;
        State.players[State.player].rootMap.SetTile(new Vector3Int(x, y), State.rootTile);
    }
}
