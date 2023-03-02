using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AphidCard: Card
{
    private int moves = 2;

    public override string GetName()
    {
        return "aphid";
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Validation(int x, int y)
    {
        int index = State.CoordToIndex(x, y);

        return index != -1 && (State.state[index] == State.players[(State.player + 1) % 2].root || State.state[index] == State.players[(State.player + 1) % 2].deadRoot);
    }

    public override void Action(int x, int y)
    {
        if (moves == 2)
        {
            State.players[State.player].water -= 2;
        }

        int index = State.CoordToIndex(x, y);
        State.state[index] = '-';
        State.players[(State.player + 1) % 2].rootMap.SetTile(new Vector3Int(x, y), null);

        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = State.CoordToIndex(dirX, dirY);
            if (dirI != -1 && State.state[dirI] == State.players[(State.player + 1) % 2].root && 
                !State.AStar(dirX, dirY, State.players[(State.player + 1) % 2].root, State.players[(State.player + 1) % 2].baseRoot))
            {
                KillBranch(dirX, dirY);
            }
        }

        if (moves == 1)
        {
            State.card = new RootCard();
        }

        moves--;
    }

    private void KillBranch(int x, int y)
    {
        int index = State.CoordToIndex(x, y);
        State.state[index] = State.players[(State.player + 1) % 2].deadRoot;
        State.players[(State.player + 1) % 2].rootMap.SetTile(new Vector3Int(x, y), State.deadRootTile);
        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = State.CoordToIndex(dirX, dirY);
            if (dirI != -1 && State.state[dirI] == State.players[(State.player + 1) % 2].root)
            {
                KillBranch(dirX, dirY);
            }
        }
    }
}
