using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeCard : Card
{
    public override string GetName(State state)
    {
        return "A Sym-Bee-Otic Relationship";
    }

    public override int GetCost(State state)
    {
        return 2;
    }

    public override int GetNumActions(State state)
    {
        return 1;
    }

    public override bool Validation(State state, int index)
    {
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        return index != -1 && (state.board[index] == state.players[state.otherPlayer].root || state.board[index] == state.players[state.otherPlayer].deadRoot) && state.HasNeighbor(x, y, new char[] { state.players[state.thisPlayer].root, state.players[state.thisPlayer].fortifiedRoot, state.players[state.thisPlayer].baseRoot });
    }

    public override void Action(State state, int index)
    {
        state.KillRoot(index, state.players[state.otherPlayer]);
        state.PlaceRoot(index, state.players[state.thisPlayer]);
    }

    public override float GetVolume(State state)
    {
        return 0.7f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "None of your roots are next to any of your opponent's unfortified roots.";
    }
}
