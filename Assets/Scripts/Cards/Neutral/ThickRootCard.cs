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

    public override IEnumerator UpdateValidAIMoves(State state)
    {
        yield return null;
        Player thisPlayer = state.players[state.thisPlayer];
        Player otherPlayer = state.players[state.otherPlayer];
        char[] enemyRoots = new char[] { otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.invincibleRoot, otherPlayer.baseRoot};
        char[] fires = new char[] { thisPlayer.strongFire, thisPlayer.weakFire, otherPlayer.strongFire, otherPlayer.weakFire };
        state.validAIMoves.Clear();
        int minThreats = 0;
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);

            if (Validation(state, i))
            {
                int threats = state.CountAllNeighbors(i, new char[] { 'R' });
                threats += 2 * state.CountNeighbors(coords[0], coords[1], enemyRoots);
                threats += 3 * state.CountNeighbors(coords[0], coords[1], new char[] { otherPlayer.thorn });
                threats += 15 * state.CountNeighbors(coords[0], coords[1], fires);
                if (threats > minThreats)
                {
                    state.validAIMoves.Clear();
                    minThreats = threats;
                }
                if (threats == minThreats)
                {
                    state.validAIMoves.Add(i);
                }
            }
        }
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
}
