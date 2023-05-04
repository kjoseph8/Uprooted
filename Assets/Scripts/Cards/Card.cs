using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    public abstract string GetName(State state);

    public abstract int GetCost(State state);

    public abstract int GetNumActions(State state);

    public abstract bool Validation(State state, int index);

    public abstract void Action(State state, int index);

    public abstract float GetVolume(State state);

    public virtual bool AIValidation(State state)
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

    public virtual void UpdateValidAIMoves(State state)
    {
        state.validAIMoves.Clear();
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            if (Validation(state, i))
            {
                state.validAIMoves.Add(i);
            }
        }
    }

    public virtual string GetDisabledMessage(State state)
    {
        return "If you see this message, contact the developer.";
    }

    public virtual bool OverrideHighlight(State state, int index)
    {
        return false;
    }

    public virtual string GetWarningMessage(State state)
    {
        return null;
    }

    public virtual string GetCardTips(State state)
    {
        return null;
    }
}
