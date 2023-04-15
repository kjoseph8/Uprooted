using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveCard : Card
{
    public override string GetName()
    {
        return "A Symbol of Love";
    }

    public override int GetCost(State state)
    {
        return 2;
    }

    public override int GetNumActions(State state)
    {
        return 2;
    }

    public override bool Validation(State state, int index)
    {
        if (index == -1)
        {
            return false;
        }

        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];

        int[] partnerCoords = state.IndexToCoord(state.loveCardPartnerIndex);
        int partnerX = partnerCoords[0];
        int partnerY = partnerCoords[1];

        if (state.numActions != GetNumActions(state))
        {
            bool isPartner = false;
            int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
            for (int i = 0; i < 4; i++)
            {
                int dirX = dirs[i, 0];
                int dirY = dirs[i, 1];
                if (dirX == partnerX && dirY == partnerY)
                {
                    isPartner = true;
                }
            }
            if (!isPartner)
            {
                return false;
            }
        }
        else if (state.CountNeighbors(x, y, new char[] { state.players[state.thisPlayer].root }) == 0)
        {
            return false;
        }

        return state.board[index] == state.players[state.thisPlayer].root;
    }

    public override bool AIValidation(State state)
    {
        Player thisPlayer = state.players[state.thisPlayer];
        Player otherPlayer = state.players[state.otherPlayer];
        char[] critical = new char[] { thisPlayer.strongFire, thisPlayer.weakFire, otherPlayer.strongFire, otherPlayer.weakFire, otherPlayer.thorn, otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.invincibleRoot, otherPlayer.baseRoot, 'R' };

        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (!Validation(state, i))
            {
                continue;
            }
            if (state.numActions != GetNumActions(state))
            {
                return true;
            }

            int[] coords = state.IndexToCoord(i);
            if (state.CountNeighbors(coords[0], coords[1], critical) > 0)
            {
                return true;
            }
        }
        return false;
    }

    public override IEnumerator UpdateValidAIMoves(State state)
    {
        yield return null;
        state.validAIMoves.Clear();
        if (state.numActions != GetNumActions(state))
        {
            for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
            {
                if (Validation(state, i))
                {
                    state.validAIMoves.Add(i);
                }
            }
        }
        else
        {
            Player thisPlayer = state.players[state.thisPlayer];
            Player otherPlayer = state.players[state.otherPlayer];
            char[] enemyRoots = new char[] { otherPlayer.root, otherPlayer.fortifiedRoot, otherPlayer.invincibleRoot, otherPlayer.baseRoot };
            char[] fires = new char[] { thisPlayer.strongFire, thisPlayer.weakFire, otherPlayer.strongFire, otherPlayer.weakFire };
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
    }

    public override void Action(State state, int index)
    {
        state.board[index] = state.players[state.thisPlayer].fortifiedRoot;
        state.loveCardPartnerIndex = index;
        int[] coords = state.IndexToCoord(index);
        int x = coords[0];
        int y = coords[1];
        if (state.absolute)
        {
            State.otherMap.SetTile(new Vector3Int(x, y), State.woodShieldTile);
        }
    }

    public override string GetDisabledMessage()
    {
        return "You have no pairs of unfortified roots to fortify.";
    }
}
