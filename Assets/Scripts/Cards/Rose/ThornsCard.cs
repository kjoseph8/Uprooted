using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsCard : Card
{
    public override string GetName(State state)
    {
        return "Every Rose has its Thorn";
    }

    public override int GetCost(State state)
    {
        return 1;
    }

    public override int GetNumActions(State state)
    {
        return 1;
    }

    public override bool Validation(State state, int index)
    {
        return index != -1 && state.board[index] == '-';
    }

    public override bool AIValidation(State state)
    {
        Player player = state.players[state.otherPlayer];

        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);

            if (Validation(state, i) && state.CountNeighbors(coords[0], coords[1], new char[] { player.root, player.fortifiedRoot, player.deadRoot, player.deadFortifiedRoot }) > 1)
            {
                return true;
            }
        }
        return false;
    }

    public override void UpdateValidAIMoves(State state)
    {
        Player player = state.players[state.otherPlayer];
        state.validAIMoves.Clear();
        int maxDestructibles = 0;
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);

            if (Validation(state, i))
            {
                int destructibles = state.CountNeighbors(coords[0], coords[1], new char[] { player.root, player.fortifiedRoot, player.deadRoot, player.deadFortifiedRoot });

                if (destructibles > maxDestructibles)
                {
                    state.validAIMoves.Clear();
                    maxDestructibles = destructibles;
                }
                if (destructibles == maxDestructibles)
                {
                    state.validAIMoves.Add(i);
                }
            }
        }
    }

    public override void Action(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        state.board[index] = state.players[state.thisPlayer].thorn;
        if (state.absolute)
        {
            state.players[state.thisPlayer].rootMap.SetTile(new Vector3Int(x, y), State.thornTile);
        }
    }

    public override float GetVolume(State state)
    {
        return 0.4f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "I don't know how you did it, but there are no empty spaces on the board to place a thorn block.";
    }
}
