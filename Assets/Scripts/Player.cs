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
    public int water = 1;
    public int rockCount = 0;
    public int rootCount = 0;
    public int rootMoves = 0;
    public int scentTurns = 0;
    public char root;
    public char fortifiedRoot;
    public char invincibleRoot;
    public char baseRoot;
    public char deadRoot;
    public char deadFortifiedRoot;
    public char deadInvincibleRoot;
    public char thorn;
    public char strongFire;
    public char weakFire;
    public Tilemap rootMap;

    public Player(char root, char fortifiedRoot, char invincibleRoot, char baseRoot, char deadRoot, char deadFortifiedRoot, char deadInvincibleRoot, char thorn, char strongFire, char weakFire, Tilemap rootMap, Plant plant)
    {
        this.root = root;
        this.fortifiedRoot = fortifiedRoot;
        this.invincibleRoot = invincibleRoot;
        this.baseRoot = baseRoot;
        this.deadRoot = deadRoot;
        this.deadFortifiedRoot = deadFortifiedRoot;
        this.deadInvincibleRoot = deadInvincibleRoot;
        this.thorn = thorn;
        this.strongFire = strongFire;
        this.weakFire = weakFire;
        this.rootMap = rootMap;
        this.plant = plant;
        for (int i = 0; i < plant.GetCards().Length; i++)
        {
            draw.Add(i);
        }
    }

    public Player(Player parent)
    {
        plant = parent.plant;
        water = parent.water;
        rockCount = parent.rockCount;
        rootCount = parent.rootCount;
        rootMoves = parent.rootMoves;
        scentTurns = parent.scentTurns;
        root = parent.root;
        fortifiedRoot = parent.fortifiedRoot;
        invincibleRoot = parent.invincibleRoot;
        baseRoot = parent.baseRoot;
        deadRoot = parent.deadRoot;
        deadFortifiedRoot = parent.deadFortifiedRoot;
        deadInvincibleRoot = parent.deadInvincibleRoot;
        thorn = parent.thorn;
        strongFire = parent.strongFire;
        weakFire = parent.weakFire;
        rootMap = parent.rootMap;
        foreach(int i in parent.draw)
        {
            draw.Add(i);
        }
        foreach (int i in parent.hand)
        {
            hand.Add(i);
        }
        foreach (int i in parent.discard)
        {
            discard.Add(i);
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
