using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    public abstract string GetName();

    public abstract int GetCost(State state);

    public abstract int GetNumActions(State state);

    public abstract bool Validation(State state, int index);

    public abstract void Action(State state, int index);
}
