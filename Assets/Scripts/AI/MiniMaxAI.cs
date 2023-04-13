using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MiniMaxAI
{
    private int lastTime = (int)Time.time;

    public async void Control(State state, GameManager manager)
    {
        await FastDelay(1000);
        lastTime = (int)Time.time;
        if (state.discardPhase)
        {
            await Task.Run(() => SelectDiscard(state, 2));
            lastTime = (int)Time.time;
            await FastDelay(1000);
            state.DiscardCard();
        }
        else if (state.tilePhase)
        {
            await Task.Run(() => SelectTile(state, 2));
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
            await Task.Run(() => SelectCard(state, 2));
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

    public int ControlHelper(State state, int turns)
    {
        if (state.discardPhase)
        {
            SelectDiscard(state, turns);
            state.DiscardCard();
        }
        else if (state.tilePhase)
        {
            SelectTile(state, turns);
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
            turns--;
            if (turns <= 0)
            {
                return Heuristic(state);
            }
            SelectCard(state, turns);
            if (state.card == null || state.cardIndex == -1)
            {
                return Heuristic(state);
            }
            else if (state.cardIndex != 0)
            {
                state.PlayCard();
            }
        }
        return ControlHelper(state, turns);
    }

    private void SelectCard(State state, int turns)
    {
        Player player = state.players[state.thisPlayer];
        int card = 0;
        int heuristic = Heuristic(state);

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
                    State next = new State(state);
                    next.SetCard(index);
                    next.PlayCard();
                    ControlHelper(next, turns);
                    int nextHeuristic = Heuristic(next);
                    if (state.thisPlayer == 0)
                    {
                        if (nextHeuristic > heuristic)
                        {
                            card = index;
                            heuristic = nextHeuristic;
                        }
                    }
                    else
                    {
                        if (nextHeuristic < heuristic)
                        {
                            card = index;
                            heuristic = nextHeuristic;
                        }
                    }
                    break;
                }
            }
        }

        if (card != 0 || player.rootMoves != 0)
        {
            state.SetCard(card);
            if (card == 0)
            {
                state.tilePhase = true;
            }
            else
            {
                state.tilePhase = false;
            }
        }
        else
        {
            state.card = null;
            state.cardIndex = -1;
            state.tilePhase = false;
        }
    }

    private void SelectTile(State state, int turns)
    {
        int move = -1;
        int heuristic = 0;
        if (state.thisPlayer == 0)
        {
            heuristic = -1000000000;
        }
        else
        {
            heuristic = 1000000000;
        }

        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (state.card.Validation(state, i))
            {
                State next = new State(state);
                next.PlayTile(i);
                ControlHelper(next, turns);
                int nextHeuristic = Heuristic(next);
                if (state.thisPlayer == 0)
                {
                    if (nextHeuristic > heuristic)
                    {
                        move = i;
                        heuristic = nextHeuristic;
                    }
                }
                else
                {
                    if (nextHeuristic < heuristic)
                    {
                        move = i;
                        heuristic = nextHeuristic;
                    }
                }
            }
        }

        state.tileIndex = move;
    }

    private void SelectDiscard(State state, int turns)
    {
        Player player = state.players[state.thisPlayer];
        int card = -1;
        int heuristic = 0;
        if (state.thisPlayer == 0)
        {
            heuristic = -1000000000;
        }
        else
        {
            heuristic = 1000000000;
        }

        for (int i = 0; i < player.hand.Count; i++)
        {
            int index = player.hand[i];
            State next = new State(state);
            next.SetCard(index);
            next.DiscardCard();
            ControlHelper(next, turns);
            int nextHeuristic = Heuristic(next);
            if (state.thisPlayer == 0)
            {
                if (nextHeuristic > heuristic)
                {
                    card = index;
                    heuristic = nextHeuristic;
                }
            }
            else
            {
                if (nextHeuristic < heuristic)
                {
                    card = index;
                    heuristic = nextHeuristic;
                }
            }
        }

        if (card != -1)
        {
            state.SetCard(card);
        }
        else
        {
            state.SetCard(player.hand[new System.Random().Next(0, player.hand.Count)]);
        }
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

    private int Heuristic(State state)
    {
        state.UpdatePoints();
        if (state.turn > state.maxTurns)
        {
            if (state.players[0].points > state.players[1].points)
            {
                return 1000000000;
            }
            else if (state.players[0].points < state.players[1].points)
            {
                return -1000000000;
            }
            return 0;
        }
        int points = state.players[0].points - state.players[1].points;
        points += state.players[0].wormTurns;
        points -= state.players[1].wormTurns;
        points += 2 * state.players[0].scentTurns;
        points -= 2 * state.players[1].scentTurns;
        return points;
    }
}
