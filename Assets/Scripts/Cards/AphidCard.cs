using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AphidCard: Card
{
    private int moves = 2;

    public override string GetName()
    {
        return "Aphid Infestation";
    }

    public override bool IsInstant()
    {
        return false;
    }

    public override bool Validation(int x, int y)
    {
        State state = base.manager.state;
        int index = state.CoordToIndex(x, y);

        return index != -1 && (state.board[index] == state.players[state.otherPlayer].root || state.board[index] == state.players[state.otherPlayer].deadRoot || state.board[index] == 'T');
    }

    public override void Action(int x, int y)
    {
        State state = base.manager.state;
        if (moves == 2)
        {
            state.players[state.thisPlayer].water -= 2;
        }

        int index = state.CoordToIndex(x, y);
        state.board[index] = '-';
        state.players[state.otherPlayer].rootMap.SetTile(new Vector3Int(x, y), null);
        State.otherMap.SetTile(new Vector3Int(x, y), null);

        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = state.CoordToIndex(dirX, dirY);
            if (dirI != -1 && state.board[dirI] == state.players[state.otherPlayer].root && 
                !state.AStar(dirX, dirY, state.players[state.otherPlayer].root, state.players[state.otherPlayer].baseRoot))
            {
                state.Spread(dirX, dirY, state.players[state.otherPlayer].root, state.players[state.otherPlayer].deadRoot, State.deadRootTile, state.players[state.otherPlayer].rootMap);
            }
        }

        if (moves == 1)
        {
            state.card = GameObject.FindGameObjectWithTag("RootCard").GetComponent<RootCard>();
        }

        moves--;
    }

    public override void SetCard()
    {
        base.manager.state.card = this;
    }
}
