using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Player
{
    public Plant plant;
    public List<int> draw = new List<int>();
    public List<int> hand = new List<int>();
    public List<int> discard = new List<int>();
    public int water = 2;
    public int points = 0;
    public int rootMoves = 1;
    public char root;
    public char baseRoot;
    public char deadRoot;
    public char strongFire;
    public char weakFire;
    public List<int> strongFireIndex = new List<int>();
    public List<int> weakFireIndex = new List<int>();
    public Tilemap rootMap;
    public TextMeshProUGUI waterDisp;

    public Player(char root, char baseRoot, char deadRoot, char strongFire, char weakFire, Tilemap rootMap, Plant plant, TextMeshProUGUI waterDisp)
    {
        this.root = root;
        this.baseRoot = baseRoot;
        this.deadRoot = deadRoot;
        this.strongFire = strongFire;
        this.weakFire = weakFire;
        this.rootMap = rootMap;
        this.waterDisp = waterDisp;
        this.plant = plant;
        for (int i = 0; i < plant.GetCards().Length; i++)
        {
            draw.Add(i);
        }
    }

    public void DrawCard()
    {
        if (draw.Count == 0)
        {
            RestoreDiscard();
        }
        int cardIndex = UnityEngine.Random.Range(0, draw.Count);
        hand.Add(draw[cardIndex]);
        draw.RemoveAt(cardIndex);
    }

    public void RestoreDiscard()
    {
        for (int i = 0; i < discard.Count; i++)
        {
            draw.Add(discard[i]);
        }
        discard.Clear();
    }
}
