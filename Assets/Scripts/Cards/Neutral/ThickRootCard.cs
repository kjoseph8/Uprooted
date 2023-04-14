using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickRootCard : Card
{
    public override string GetName()
    {
        return "Thick Root";
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
        return index != -1 && state.board[index] == state.players[state.thisPlayer].root;
    }

    public override bool AIValidation(State state)
    {
        Player thisPlayer = state.players[state.thisPlayer];
        Player otherPlayer = state.players[state.otherPlayer];
        char[] critical = new char[] { thisPlayer.strongFire, thisPlayer.weakFire, otherPlayer.strongFire, otherPlayer.weakFire, otherPlayer.thorn, otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.invincibleRoot, otherPlayer.baseRoot, 'R' };

        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);

            if (Validation(state, i) && state.CountNeighbors(coords[0], coords[1], critical) > 0)
            {
                return true;
            }
        }
        return false;
    }

    public override List<int> GetValidAIMoves(State state)
    {
        Player thisPlayer = state.players[state.thisPlayer];
        Player otherPlayer = state.players[state.otherPlayer];
        char[] critical = new char[] { thisPlayer.strongFire, thisPlayer.weakFire, otherPlayer.strongFire, otherPlayer.weakFire, otherPlayer.thorn, otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.invincibleRoot, otherPlayer.baseRoot, 'R' };
        List<int> validMoves = new List<int>();
        int minThreats = 0;
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);

            if (Validation(state, i))
            {
                int threats = state.CountNeighbors(coords[0], coords[1], critical);
                if (threats > minThreats)
                {
                    validMoves.Clear();
                    minThreats = threats;
                }
                if (threats == minThreats)
                {
                    validMoves.Add(i);
                }
            }
        }
        return validMoves;
    }

    public override void Action(State state, int index)
    {
        state.board[index] = state.players[state.thisPlayer].fortifiedRoot;
        int[] coords = state.IndexToCoord(index);
        if (state.absolute)
        {
            State.otherMap.SetTile(new Vector3Int(coords[0], coords[1]), State.woodShieldTile);
        }
    }

    public override string GetDisabledMessage()
    {
        return "You have no pairs of unfortified roots to fortify.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }
}
