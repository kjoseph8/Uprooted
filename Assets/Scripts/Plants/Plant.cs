using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Plant : MonoBehaviour
{
    public abstract Sprite[] GetSprites();

    public abstract Card[] GetCards();
}
