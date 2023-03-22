using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    public abstract string GetName();

    public abstract int GetCost(State state);

    public abstract int GetNumActions(State state);

    public abstract bool Validation(State state, int index);

    public List<int> GetValidMoves(State state)
    {
        List<int> validMoves = new List<int>();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (Validation(state, i))
            {
                validMoves.Add(i);
            }
        }
        return validMoves;
    }

    public abstract void Action(State state, int index);
}
