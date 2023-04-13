using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class RandomAI
{
    private int lastTime = (int)Time.time;

    public async void Control(State state, GameManager manager)
    {
        await FastDelay(1000);
        lastTime = (int)Time.time;
        if (state.discardPhase)
        {
            await Task.Run(() => SelectDiscard(state));
            lastTime = (int)Time.time;
            await FastDelay(1000);
            state.DiscardCard();
        }
        else if (state.tilePhase)
        {
            await Task.Run(() => SelectTile(state));
            if (state.tileIndex == -1)
            {
                state.CancelCard();
            }
            else
            {
                state.PlayTile(state.tileIndex);
            }
        }
        else
        {
            await Task.Run(() => SelectCard(state));
            if (state.card == null || state.cardIndex == -1)
            {
                manager.ChangeTurn(true);
                return;
            }
            if (state.cardIndex != 0)
            {
                lastTime = (int)Time.time;
                await FastDelay(1000);
                state.PlayCard();
            }
        }
        Control(state, manager);
    }

    private void SelectCard(State state)
    {
        Player player = state.players[state.thisPlayer];
        List<int> validCards = new List<int>();
        validCards.Add(0);
        for (int i = 0; i < player.hand.Count; i++)
        {
            int index = player.hand[i];
            if (State.collection.cards[index].GetCost(state) > player.water)
            {
                continue;
            }

            for (int j = 0; j < state.boardHeight * state.boardWidth; j++)
            {
                if (State.collection.cards[index].Validation(state, j))
                {
                    validCards.Add(index);
                    break;
                }
            }
        }
        int card = validCards[new System.Random().Next(0, validCards.Count)];
        if (card != 0 || player.rootMoves != 0)
        {
            state.SetCard(card);
            if (card == 0)
            {
                state.tilePhase = true;
            }
        }
    }

    private void SelectTile(State state)
    {
        List<int> validMoves = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (state.card.Validation(state, i))
            {
                validMoves.Add(i);
            }
        }
        if (validMoves.Count == 0)
        {
            state.tileIndex = -1;
        }
        else
        {
            state.tileIndex = validMoves[new System.Random().Next(0, validMoves.Count)];
        }
    }

    private void SelectDiscard(State state)
    {
        List<int> hand = state.players[state.thisPlayer].hand;
        int card = hand[new System.Random().Next(0, hand.Count)];
        state.SetCard(card);
    }

    private Task FastDelay(int delay)
    {
        int thisTime = (int)Time.time;
        if (thisTime - lastTime < delay)
        {
            return Task.Delay(delay - (thisTime - lastTime));
        }
        return Task.Delay(0);
    }

    private void SimulateSlowness(int breadth, int depth)
    {
        try
        {
            for (int i = 0; i < breadth; i++)
            {
                int val = i;
                for (int j = 0; j < depth; j++)
                {
                    val = SimulateSlownessHelper(val);
                }
            }
        }
        catch
        {

        }
    }

    private int SimulateSlownessHelper(int val)
    {
        if (val == 0)
        {
            return 1;
        }
        return val * SimulateSlownessHelper(val - 1);
    }
}
