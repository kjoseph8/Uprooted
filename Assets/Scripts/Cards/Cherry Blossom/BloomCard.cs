using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomCard : Card
{
    public override string GetName(State state)
    {
        if (state.players[state.thisPlayer].bloom)
        {
            return "Select a root to destroy to fortify its neighbors";
        }
        else
        {
            return "Switch to Bloom stance";
        }
    }

    public override int GetCost(State state)
    {
        return 1;
    }

    public override int GetNumActions(State state)
    {
        if (state.players[state.thisPlayer].bloom)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public override bool Validation(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        if (player.bloom)
        {
            int[] coords = state.IndexToCoord(index);
            return index != -1 && Array.IndexOf(new char[] { player.root, player.fortifiedRoot, player.deadRoot, player.deadFortifiedRoot }, state.board[index]) != -1
                && state.CountNeighbors(coords[0], coords[1], new char[] { player.root, player.deadRoot }) > 0;
        }
        else
        {
            return true;
        }
    }

    public override void UpdateValidAIMoves(State state)
    {
        Player player = state.players[state.thisPlayer];
        state.validAIMoves.Clear();
        if (!player.bloom)
        {
            return;
        }
        int maxNeighbors = 0;
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);

            if (Validation(state, i))
            {
                int neighbors = state.CountNeighbors(coords[0], coords[1], new char[] { player.root, player.deadRoot });

                if (neighbors > maxNeighbors)
                {
                    state.validAIMoves.Clear();
                    maxNeighbors = neighbors;
                }
                if (neighbors == maxNeighbors)
                {
                    state.validAIMoves.Add(i);
                }
            }
        }
    }

    public override void Action(State state, int index)
    {
        Player player = state.players[state.thisPlayer];
        if (player.bloom)
        {
            state.KillRoot(index, player);
            int[] coords = state.IndexToCoord(index);
            int[,] dirs = { { coords[0] - 1, coords[1] }, { coords[0] + 1, coords[1] }, { coords[0], coords[1] - 1 }, { coords[0], coords[1] + 1 } };
            for (int i = 0; i < 4; i++)
            {
                int dirX = dirs[i, 0];
                int dirY = dirs[i, 1];
                int dirI = state.CoordToIndex(dirX, dirY);
                if (dirI != -1 && (state.board[dirI] == player.root || state.board[dirI] == player.deadRoot))
                {
                    if (state.board[dirI] == player.root)
                    {
                        state.board[dirI] = player.fortifiedRoot;
                    }
                    else
                    {
                        state.board[dirI] = player.deadFortifiedRoot;
                    }
                    if (state.absolute)
                    {
                        State.otherMap.SetTile(new Vector3Int(dirX, dirY), State.woodShieldTile);
                    }
                }
            }
        }
        else
        {
            player.bloom = true;
            if (state.absolute)
            {
                player.plantSprite.sprite = player.bloomSprite;
            }
            player.water += player.festivals;
        }
    }

    public override float GetVolume(State state)
    {
        return 1;
    }

    public override string GetDisabledMessage(State state)
    {
        return "You have no roots to destroy that are next to unfortified roots.";
    }
}
