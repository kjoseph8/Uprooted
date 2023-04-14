using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizerCard: Card
{
    public override string GetName()
    {
        return "Fertilizer";
    }

    public override int GetCost(State state)
    {
        return 1;
    }

    public override int GetNumActions(State state)
    {
        return 0;
    }

    public override bool Validation(State state, int index)
    {
        return true;
    }

    public override bool AIValidation(State state)
    {
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (Validation(state, i))
            {
                return true;
            }
        }
        return false;
    }

    public override List<int> GetValidAIMoves(State state)
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

    public override void Action(State state, int index)
    {
        state.players[state.thisPlayer].rootMoves++;
    }

    public override string GetDisabledMessage()
    {
        return "If you see this message, contact the developer.";
    }

    public override bool OverrideHighlight(State state, int index)
    {
        return false;
    }
}
