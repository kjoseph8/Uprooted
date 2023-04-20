using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MultiMiniMaxAI
{
    private static int lastTime = (int)Time.time;
    private static int maxMoves = 3;

    public static async void Control(State state, GameManager manager, int turns = 2, int moves = 2)
    {
        await FastDelay(1000);
        lastTime = (int)Time.time;
        maxMoves = moves;
        if (state.discardPhase)
        {
            await Task.Run(() => SelectDiscard(state, turns, moves, true));
            lastTime = (int)Time.time;
            await FastDelay(1000);
            manager.DiscardCard(true);
        }
        else if (state.tilePhase)
        {
            await Task.Run(() => SelectTile(state, turns, moves, true));
            if (state.tileIndex == -1)
            {
                if (state.cardIndex == 0)
                {
                    manager.ChangeTurn(true);
                    return;
                }
                else
                {
                    state.CancelCard();
                }
            }
            else
            {
                manager.PlayTile(state.tileIndex);
            }
        }
        else
        {
            await Task.Run(() => SelectCard(state, turns, moves, true));
            if (state.card == null || state.cardIndex == -1)
            {
                manager.ChangeTurn(true);
                return;
            }
            if (state.cardIndex != 0)
            {
                lastTime = (int)Time.time;
                await FastDelay(1000);
                manager.PlayCard(true);
            }
        }
        Control(state, manager);
    }

    public static void ControlHelper(State state, int turns, int moves)
    {
        if (state.discardPhase)
        {
            SelectDiscard(state, turns, moves);
            state.DiscardCard();
        }
        else if (state.tilePhase)
        {
            SelectTile (state, turns, moves);
            if (state.tileIndex == -1)
            {
                if (state.cardIndex == 0)
                {
                    state.ChangeTurn();
                    turns--;
                    moves = maxMoves;
                }
                else
                {
                    state.CancelCard();
                }
            }
            else
            {
                state.PlayTile(state.tileIndex);
            }
        }
        else
        {
            moves--;
            if (moves > 0)
            {
                SelectCard(state, turns, moves);
            }
            if (state.card == null || state.cardIndex == -1)
            {
                state.ChangeTurn();
                turns--;
                moves = maxMoves;
            }
            else if (state.cardIndex != 0)
            {
                state.PlayCard();
            }
        }
        if (turns > 0)
        {
            ControlHelper(state, turns, moves);
        }
    }

    private static void SelectCard(State state, int turns, int moves, bool makeThreads = false)
    {
        Player player = state.players[state.thisPlayer];
        int card = 0;
        State copy = new State(state);
        if (player.rootMoves > 0)
        {
            copy.SetCard(0);
            copy.tilePhase = true;
            copy.PlayCard();
        }
        else
        {
            copy.ChangeTurn();
        }
        float heuristic = MiniMaxAI.Heuristic(copy);

        List<int> cards = new List<int>();
        List<State> states = new List<State>();
        List<Task> tasks = new List<Task>();
        for (int i = 0; i < player.hand.Count; i++)
        {
            int index = player.hand[i];

            if (State.collection.cards[index].GetCost(state) <= player.water && State.collection.cards[index].AIValidation(state))
            {
                cards.Add(index);
                State next = new State(state);
                next.SetCard(index);
                next.PlayCard();
                states.Add(next);
                if (makeThreads)
                {
                    tasks.Add(Task.Run(() => ControlHelper(next, turns, moves)));
                }
                else
                {
                    ControlHelper(next, turns, moves);
                }
            }
        }

        if (makeThreads)
        {
            Task.WaitAll(tasks.ToArray());
        }

        for (int i = 0; i < cards.Count; i++)
        {
            float nextHeuristic = MiniMaxAI.Heuristic(states[i]);
            if (state.thisPlayer == 0)
            {
                if (nextHeuristic > heuristic)
                {
                    card = cards[i];
                    heuristic = nextHeuristic;
                }
            }
            else
            {
                if (nextHeuristic < heuristic)
                {
                    card = cards[i];
                    heuristic = nextHeuristic;
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

    private static void SelectTile(State state, int turns, int moves, bool makeThreads = false)
    {
        int move = -1;
        float heuristic = 0;
        if (state.thisPlayer == 0)
        {
            heuristic = -1000000000;
        }
        else
        {
            heuristic = 1000000000;
        }

        state.card.UpdateValidAIMoves(state);

        while (Mathf.Pow(state.validAIMoves.Count, state.card.GetNumActions(state)) > 10)
        {
            state.validAIMoves.RemoveAt(new System.Random().Next(0, state.validAIMoves.Count));
        }

        List<Task> tasks = new List<Task>();
        List<State> states = new List<State>();
        for(int i = 0; i < state.validAIMoves.Count; i++)
        {
            State next = new State(state);
            next.PlayTile(state.validAIMoves[i]);
            states.Add(next);
            if (makeThreads)
            {
                tasks.Add(Task.Run(() => ControlHelper(next, turns, moves)));
            }
            else
            {
                ControlHelper(next, turns, moves);
            }
        }

        if (makeThreads)
        {
            Task.WaitAll(tasks.ToArray());
        }

        for (int i = 0; i < state.validAIMoves.Count; i++)
        {
            float nextHeuristic = MiniMaxAI.Heuristic(states[i]);
            if (state.thisPlayer == 0)
            {
                if (nextHeuristic > heuristic)
                {
                    move = state.validAIMoves[i];
                    heuristic = nextHeuristic;
                }
            }
            else
            {
                if (nextHeuristic < heuristic)
                {
                    move = state.validAIMoves[i];
                    heuristic = nextHeuristic;
                }
            }
        }

        state.tileIndex = move;
    }

    private static void SelectDiscard(State state, int turns, int moves, bool makeThreads = false)
    {
        Player player = state.players[state.thisPlayer];
        int card = -1;
        float heuristic = 0;
        if (state.thisPlayer == 0)
        {
            heuristic = -1000000000;
        }
        else
        {
            heuristic = 1000000000;
        }

        List<Task> tasks = new List<Task>();
        List<State> states = new List<State>();
        for (int i = 0; i < player.hand.Count; i++)
        {
            int index = player.hand[i];
            State next = new State(state);
            next.SetCard(index);
            next.DiscardCard();
            states.Add(next);
            if (makeThreads)
            {
                tasks.Add(Task.Run(() => ControlHelper(next, turns, moves)));
            }
            else
            {
                ControlHelper(next, turns, moves);
            }
        }

        if (makeThreads)
        {
            Task.WaitAll(tasks.ToArray());
        }

        for (int i = 0; i < player.hand.Count; i++)
        {
            float nextHeuristic = MiniMaxAI.Heuristic(states[i]);
            if (state.thisPlayer == 0)
            {
                if (nextHeuristic > heuristic)
                {
                    card = player.hand[i];
                    heuristic = nextHeuristic;
                }
            }
            else
            {
                if (nextHeuristic < heuristic)
                {
                    card = player.hand[i];
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

    private static Task FastDelay(int delay)
    {
        int thisTime = (int)Time.time;
        if (thisTime - lastTime < delay)
        {
            return Task.Delay(delay - (thisTime - lastTime));
        }
        return Task.Delay(0);
    }
}