using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickRootCard : Card
{
    public override string GetName(State state)
    {
        return "Thick Root";
    }

    public override string GetDescription(State state)
    {
        return "Fortify one of your roots";
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

    public override void UpdateValidAIMoves(State state)
    {
        Player thisPlayer = state.players[state.thisPlayer];
        Player otherPlayer = state.players[state.otherPlayer];
        char[] enemyRoots = new char[] { otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.invincibleRoot, otherPlayer.baseRoot};
        char[] fires = new char[] { thisPlayer.strongFire, thisPlayer.weakFire, otherPlayer.strongFire, otherPlayer.weakFire };
        state.validAIMoves.Clear();
        int maxThreats = 0;
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);

            if (Validation(state, i))
            {
                int threats = state.CountAllNeighbors(i, new char[] { 'R' });
                threats += 2 * state.CountNeighbors(coords[0], coords[1], enemyRoots);
                threats += 3 * state.CountNeighbors(coords[0], coords[1], new char[] { otherPlayer.thorn });
                threats += 15 * state.CountNeighbors(coords[0], coords[1], fires);
                if (threats > maxThreats)
                {
                    state.validAIMoves.Clear();
                    maxThreats = threats;
                }
                if (threats == maxThreats)
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

    public override float GetVolume(State state)
    {
        return 0.6f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "You have no pairs of unfortified roots to fortify.";
    }

    public override string GetCardTips(State state)
    {
        return "Fortify: Prevents a root from being destroyed once.";
    }
}
