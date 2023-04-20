using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Player
{
    public List<int> draw = new List<int>();
    public List<int> hand = new List<int>();
    public List<int> discard = new List<int>();
    public bool ai;
    public string plant;
    public int points = 0;
    public int water = 0;
    public int rockCount = 0;
    public int completeRockCount = 0;
    public int rootCount = 0;
    public int rootMoves = 0;
    public int scentTurns = 0;
    public int wormTurns = 0;
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

    public Player(bool ai, string plant, char root, char fortifiedRoot, char invincibleRoot, char baseRoot, char deadRoot, char deadFortifiedRoot, char deadInvincibleRoot, char thorn, char strongFire, char weakFire, Tilemap rootMap)
    {
        this.ai = ai;
        this.plant = plant;
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
        for (int i = 0; i < CardCollection.rose.Length; i++)
        {
            draw.Add(CardCollection.rose[i]);
        }
    }

    public Player(Player parent)
    {
        ai = parent.ai;
        plant = parent.plant;
        points = parent.points;
        water = parent.water;
        rockCount = parent.rockCount;
        completeRockCount = parent.completeRockCount;
        rootCount = parent.rootCount;
        rootMoves = parent.rootMoves;
        scentTurns = parent.scentTurns;
        wormTurns = parent.wormTurns;
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
        int cardIndex = new System.Random().Next(0, draw.Count);
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
