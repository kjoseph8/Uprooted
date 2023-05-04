using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMaxAI: MonoBehaviour
{
    private int lastTime;

    void Start()
    {
        lastTime = (int)Time.time;
    }

    public IEnumerator Control(State state, GameManager manager, int moves = 2, int delay = 1500)
    {
        bool finished = false;
        int thisTime = (int)Time.time;
        if (thisTime - lastTime < delay)
        {
            yield return new WaitForSeconds((delay - (thisTime - lastTime)) / 1000.0f);
        }
        lastTime = (int)Time.time;
        if (state.discardPhase)
        {
            yield return StartCoroutine(SelectDiscard(state, moves));
            yield return new WaitForSeconds(delay / 1000.0f);
            manager.DiscardCard(true);
        }
        else if (state.tilePhase)
        {
            yield return StartCoroutine(SelectTile(state, moves));
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
                manager.PlayTile(state.tileIndex);
            }
        }
        else if (state.players[state.thisPlayer].rootMoves == 0)
        {
            manager.ChangeTurn(true);
            finished = true;
        }
        else
        {
            yield return StartCoroutine(SelectCard(state, moves));
            if (state.card == null || state.cardIndex == -1)
            {
                manager.ChangeTurn(true);
                finished = true;
            }
            else if (state.cardIndex != 0)
            {
                lastTime = (int)Time.time;
                yield return new WaitForSeconds(delay / 1000.0f);
                manager.PlayCard(true);
            }
        }
        if (!finished)
        {
            yield return StartCoroutine(Control(state, manager));
        }
    }

    IEnumerator ControlHelper(State state, int moves)
    {
        bool finished = false;
        if (state.discardPhase)
        {
            yield return StartCoroutine(SelectDiscard(state, moves));
            state.DiscardCard();
        }
        else if (state.tilePhase)
        {
            yield return StartCoroutine(SelectTile(state, moves));
            if (state.tileIndex == -1)
            {
                if (state.cardIndex == 0)
                {
                    state.ChangeTurn();
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
        else if (state.players[state.thisPlayer].rootMoves == 0)
        {
            state.ChangeTurn();
            finished = true;
        }
        else
        {
            moves--;
            if (moves > 0)
            {
                yield return StartCoroutine(SelectCard(state, moves));
            }
            if (state.card == null || state.cardIndex == -1)
            {
                state.ChangeTurn();
                finished = true;
            }
            else if (state.cardIndex != 0)
            {
                state.PlayCard();
            }
        }
        if (!finished)
        {
            yield return StartCoroutine(ControlHelper(state, moves));
        }
    }

    IEnumerator SelectCard(State state, int moves)
    {
        yield return null;
        Player player = state.players[state.thisPlayer];
        int card = -1;
        State next = new State(state);
        next.SetCard(-1);
        next.tilePhase = true;
        yield return StartCoroutine(ControlHelper(next, moves));
        float heuristic = Heuristic(next);

        for (int i = 0; i < player.hand.Count; i++)
        {
            if (State.collection.cards[player.hand[i]].GetCost(state) <= player.water && State.collection.cards[player.hand[i]].AIValidation(state))
            {
                next = new State(state);
                next.SetCard(i);
                next.PlayCard();
                yield return StartCoroutine(ControlHelper(next, moves));
                float nextHeuristic = Heuristic(next);
                if (state.thisPlayer == 0)
                {
                    if (nextHeuristic >= heuristic)
                    {
                        card = i;
                        heuristic = nextHeuristic;
                    }
                }
                else
                {
                    if (nextHeuristic <= heuristic)
                    {
                        card = i;
                        heuristic = nextHeuristic;
                    }
                }
            }
        }

        state.SetCard(card);
        if (card == -1)
        {
            state.tilePhase = true;
        }
        else
        {
            state.tilePhase = false;
        }
    }

    IEnumerator SelectTile(State state, int moves)
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

        while (Mathf.Pow(state.validAIMoves.Count, state.card.GetNumActions(state)) > 5)
        {
            state.validAIMoves.RemoveAt(new System.Random().Next(0, state.validAIMoves.Count));
        }

        if (state.validAIMoves.Count == 1)
        {
            move = state.validAIMoves[0];
        }
        else
        {
            foreach (int index in state.validAIMoves)
            {
                State next = new State(state);
                next.PlayTile(index);
                yield return StartCoroutine(ControlHelper(next, moves));
                float nextHeuristic = Heuristic(next);
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
        }

        state.tileIndex = move;
    }

    IEnumerator SelectDiscard(State state, int moves)
    {
        yield return null;
        Player player = state.players[state.thisPlayer];
        int card = 0;
        float heuristic = 0;
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
            State next = new State(state);
            next.SetCard(i);
            next.DiscardCard();
            yield return StartCoroutine(ControlHelper(next, moves));
            float nextHeuristic = Heuristic(next);
            if (state.thisPlayer == 0)
            {
                if (nextHeuristic > heuristic)
                {
                    card = i;
                    heuristic = nextHeuristic;
                }
            }
            else
            {
                if (nextHeuristic < heuristic)
                {
                    card = i;
                    heuristic = nextHeuristic;
                }
            }
        }
        state.SetCard(card);
    }

    public static float Heuristic(State state)
    {
        state.UpdatePoints();
        float points = state.players[0].points - state.players[1].points;
        if (state.turn < state.maxTurns)
        {
            points += state.players[0].wormTurns;
            points -= state.players[1].wormTurns;
            points += state.players[0].scentTurns;
            points -= state.players[1].scentTurns;
            points += (state.maxTurns - state.turn) * state.players[0].festivals;
            points -= (state.maxTurns - state.turn) * state.players[1].festivals;
            for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
            {
                int[] coords = state.IndexToCoord(i);

                if (Array.IndexOf(new char[] { state.players[0].strongFire, state.players[1].strongFire }, state.board[i]) != -1)
                {
                    points -= 3 * state.CountNeighbors(coords[0], coords[1], new char[] { state.players[0].root, state.players[0].deadRoot, state.players[0].thorn });
                    points += 3 * state.CountNeighbors(coords[0], coords[1], new char[] { state.players[1].root, state.players[1].deadRoot, state.players[1].thorn });
                }
                else if (Array.IndexOf(new char[] { state.players[0].weakFire, state.players[1].weakFire }, state.board[i]) != -1)
                {
                    points -= 3 * state.CountNeighbors(coords[0], coords[1], new char[] { state.players[0].root, state.players[0].deadRoot, state.players[0].thorn });
                    points += 3 * state.CountNeighbors(coords[0], coords[1], new char[] { state.players[1].root, state.players[1].deadRoot, state.players[1].thorn });
                }
                else if (state.board[i] == state.players[0].thorn)
                {
                    points += 0.75f * state.CountNeighbors(coords[0], coords[1], new char[] { state.players[1].root, state.players[1].deadRoot });
                    points += 0.5f * state.CountNeighbors(coords[0], coords[1], new char[] { state.players[1].fortifiedRoot, state.players[1].deadFortifiedRoot });
                }
                else if (state.board[i] == state.players[1].thorn)
                {
                    points -= 0.75f * state.CountNeighbors(coords[0], coords[1], new char[] { state.players[0].root, state.players[0].deadRoot });
                    points -= 0.5f * state.CountNeighbors(coords[0], coords[1], new char[] { state.players[0].fortifiedRoot, state.players[0].deadFortifiedRoot });
                }
                else if (state.board[i] == state.players[0].deadRoot)
                {
                    points += 0.25f;
                }
                else if (state.board[i] == state.players[1].deadRoot)
                {
                    points -= 0.25f;
                }
                else if (state.board[i] == state.players[0].deadFortifiedRoot)
                {
                    points += 0.5f;
                }
                else if (state.board[i] == state.players[1].deadFortifiedRoot)
                {
                    points -= 0.5f;
                }
                else if (state.board[i] == state.players[0].fortifiedRoot)
                {
                    points += 0.5f;
                }
                else if (state.board[i] == state.players[1].fortifiedRoot)
                {
                    points -= 0.5f;
                }
                else if (state.board[i] == state.players[0].baseRoot)
                {
                    points += 1;
                    List<int> start = new List<int>();
                    start.Add(i);
                    Player player = state.players[0];
                    if (state.BFS(start, new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot }, new char[0], "oasis") != -1)
                    {
                        points += state.maxTurns - state.turn;
                    }
                }
                else if (state.board[i] == state.players[1].baseRoot)
                {
                    points -= 1;
                    List<int> start = new List<int>();
                    start.Add(i);
                    Player player = state.players[1];
                    if (state.BFS(start, new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot }, new char[0], "oasis") != -1)
                    {
                        points -= state.maxTurns - state.turn;
                    }
                }
            }
        }
        return points;
    }
}
