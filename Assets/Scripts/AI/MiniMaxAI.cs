using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MiniMaxAI: MonoBehaviour
{
    private int lastTime;

    void Start()
    {
        lastTime = (int)Time.time;
    }

    public IEnumerator Control(State state, GameManager manager)
    {
        bool finished = false;
        int thisTime = (int)Time.time;
        if (thisTime - lastTime < 1000)
        {
            yield return new WaitForSeconds((1000 - (thisTime - lastTime)) / 1000.0f);
        }
        yield return new WaitForSeconds(1);
        lastTime = (int)Time.time;
        if (state.discardPhase)
        {
            yield return StartCoroutine(SelectDiscard(state, 2));
            yield return new WaitForSeconds(1);
            state.DiscardCard();
        }
        else if (state.tilePhase)
        {
            yield return StartCoroutine(SelectTile(state, 2));
            if (state.tileIndex == -1)
            {
                if (state.cardIndex == 0)
                {
                    manager.ChangeTurn(true);
                    finished = true;
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
            yield return StartCoroutine(SelectCard(state, 2));
            if (state.card == null || state.cardIndex == -1)
            {
                manager.ChangeTurn(true);
                finished = true;
            }
            else if (state.cardIndex != 0)
            {
                lastTime = (int)Time.time;
                yield return new WaitForSeconds(1);
                state.PlayCard();
            }
        }
        if (!finished)
        {
            yield return StartCoroutine(Control(state, manager));
        }
    }

    IEnumerator ControlHelper(State state, int turns)
    {
        bool finished = false;
        if (state.discardPhase)
        {
            yield return StartCoroutine(SelectDiscard(state, turns));
            state.DiscardCard();
        }
        else if (state.tilePhase)
        {
            yield return StartCoroutine(SelectTile(state, turns));
            if (state.tileIndex == -1)
            {
                if (state.cardIndex == 0)
                {
                    finished = true;
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
            turns--;
            if (turns > 0)
            {
                yield return StartCoroutine(SelectCard(state, turns));
            }
            if (state.card == null || state.cardIndex == -1)
            {
                finished = true;
            }
            else if (state.cardIndex != 0)
            {
                state.PlayCard();
            }
        }
        if (!finished)
        {
            yield return StartCoroutine(ControlHelper(state, turns));
        }
    }

    IEnumerator SelectCard(State state, int turns)
    {
        yield return null;
        Player player = state.players[state.thisPlayer];
        int card = 0;
        State stateClone = new State(state);
        int heuristic = Heuristic(stateClone);

        for (int i = 0; i < player.hand.Count; i++)
        {
            int index = player.hand[i];
            
            if (State.collection.cards[index].GetCost(state) <= player.water && State.collection.cards[index].AIValidation(state))
            {
                State next = new State(state);
                next.SetCard(index);
                next.PlayCard();
                yield return StartCoroutine(ControlHelper(next, turns));
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

    IEnumerator SelectTile(State state, int turns)
    {
        yield return null;
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

        List<int> validMoves = state.card.GetValidAIMoves(state);

        foreach (int index in validMoves)
        {
            State next = new State(state);
            next.PlayTile(index);
            yield return StartCoroutine(ControlHelper(next, turns));
            int nextHeuristic = Heuristic(next);
            if (state.thisPlayer == 0)
            {
                if (nextHeuristic > heuristic)
                {
                    move = index;
                    heuristic = nextHeuristic;
                }
            }
            else
            {
                if (nextHeuristic < heuristic)
                {
                    move = index;
                    heuristic = nextHeuristic;
                }
            }
        }

        state.tileIndex = move;
    }

    IEnumerator SelectDiscard(State state, int turns)
    {
        yield return null;
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
            yield return StartCoroutine(ControlHelper(next, turns));
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

    public static int Heuristic(State state)
    {
        state.ChangeTurn();
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
