using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    public abstract string GetName();

    public abstract int GetCost(State state);

    public abstract int GetNumActions(State state);

    public abstract bool Validation(State state, int index);

    public abstract bool AIValidation(State state);

    public abstract List<int> GetValidAIMoves(State state);

    public abstract void Action(State state, int index);

    public abstract string GetDisabledMessage();

    public abstract bool OverrideHighlight(State state, int index);
}
