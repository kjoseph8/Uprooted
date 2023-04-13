using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrWormCard : Card
{
    public override string GetName()
    {
        return "Mr Worm!";
    }

    public override int GetCost(State state)
    {
        return 2;
    }

    public override int GetNumActions(State state)
    {
        return 0;
    }

    public override bool Validation(State state, int index)
    {
        return state.players[state.thisPlayer].wormTurns <= state.maxTurns - state.turn;
    }

    public override void Action(State state, int index)
    {
        state.players[state.thisPlayer].wormTurns += 3;
    }

    public override string GetDisabledMessage()
    {
        return "You already paid Mr.Worm enough to work for you till the end of the game.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }

    public static void GrowRandomRoot(State state)
    {
        Player player = state.players[state.thisPlayer];
        List<int> potential = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);
            int x = coords[0];
            int y = coords[1];

            if (state.board[i] == '-' && state.CountNeighbors(x, y, new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot }) > 0)
            {
                potential.Add(i);
            }
        }

        if (potential.Count > 0)
        {
            int index = potential[new System.Random().Next(0, potential.Count)];
            new RootCard().Action(state, index);
            player.rootMoves++;
        }
    }
}
