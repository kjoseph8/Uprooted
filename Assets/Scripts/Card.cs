using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card: MonoBehaviour
{
    [HideInInspector] public GameManager manager;

    public void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public abstract string GetName();

    public abstract bool IsInstant();

    public abstract bool Validation(int x, int y);

    public abstract void Action(int x, int y);

    public abstract void SetCard();
}
