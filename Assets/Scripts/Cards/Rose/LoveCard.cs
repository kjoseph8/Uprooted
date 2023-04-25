using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveCard : Card
{
    public override string GetName(State state)
    {
        return "A Symbol of Love";
    }

    public override int GetCost(State state)
    {
        return 3;
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
        else if (state.CountNeighbors(x, y, new char[] { '-', 'W' }) == 0)
        {
            return false;
        }

        Player player = state.players[state.thisPlayer];
        return Array.IndexOf(new char[] { '-', 'W' }, state.board[index]) != -1 && state.HasNeighbor(x, y, new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot }); ;
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

    public override void UpdateValidAIMoves(State state)
    {
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
            int maxThreats = 0;
            for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
            {
                int[] coords = state.IndexToCoord(i);

                if (Validation(state, i))
                {
                    int threats = 5 * state.CountAllNeighbors(i, new char[] { 'R' });
                    threats += state.CountNeighbors(coords[0], coords[1], enemyRoots);
                    threats += state.CountNeighbors(coords[0], coords[1], new char[] { otherPlayer.thorn });
                    threats += state.CountNeighbors(coords[0], coords[1], fires);
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
    }

    public override void Action(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        state.PlaceRoot(index, player);
        if (state.turn < state.maxTurns)
        {
            state.board[index] = player.fortifiedRoot;
            if (state.absolute)
            {
                int[] coords = state.IndexToCoord(index);
                State.otherMap.SetTile(new Vector3Int(coords[0], coords[1]), State.woodShieldTile);
            }
        }
        state.loveCardPartnerIndex = index;
    }

    public override float GetVolume(State state)
    {
        return 0.6f;
    }

    public override string GetDisabledMessage(State state)
    {
        return "You have no pairs of unfortified roots to fortify.";
    }
}
