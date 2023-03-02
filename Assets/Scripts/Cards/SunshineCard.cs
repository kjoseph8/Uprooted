using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunshineCard : Card
{
    public override string GetName()
    {
        return "Ray of Sunshine";
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Validation(int x, int y)
    {
        int index = State.CoordToIndex(x, y);

        return index != -1 && (State.state[index] == State.players[(State.player + 1) % 2].root || State.state[index] == State.players[(State.player + 1) % 2].deadRoot || State.state[index] == 'T');
    }
    public override void Action(int x, int y)
    {
        State.players[State.player].water -= 5;
        int index = State.CoordToIndex(x, y);
        State.state[index] = State.players[(State.player + 1) % 2].strongFire;
        State.players[(State.player + 1) % 2].strongFireIndex.Add(index);
        State.players[(State.player + 1) % 2].rootMap.SetTile(new Vector3Int(x, y), null);
        State.otherMap.SetTile(new Vector3Int(x, y), State.strongFireTile);

        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = State.CoordToIndex(dirX, dirY);
            if (dirI != -1 && State.state[dirI] == State.players[(State.player + 1) % 2].root &&
                !State.AStar(dirX, dirY, State.players[(State.player + 1) % 2].root, State.players[(State.player + 1) % 2].baseRoot))
            {
                State.Spread(dirX, dirY, State.players[(State.player + 1) % 2].root, State.players[(State.player + 1) % 2].deadRoot, State.deadRootTile, State.players[(State.player + 1) % 2].rootMap);
            }
        }

        State.card = new RootCard();
    }
}
