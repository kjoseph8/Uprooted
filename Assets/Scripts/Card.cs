using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Card
{
    public bool isInstant();

    public bool Validation(int x, int y);

    public void Action(int x, int y);
}
