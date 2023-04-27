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
    public List<int> freeCards = new List<int>();
    public bool ai;
    public string plant;
    public int points = 0;
    public int water = 0;
    public int rockCount = 0;
    public int completeRockCount = 0;
    public int rootCount = 0;
    public int rootMoves = 0;
    public int wormTurns = 0;
    public int scentTurns = 0;
    public bool bloom = false;
    public int festivals = 0;
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
    public SpriteRenderer plantSprite;
    public Sprite dormantSprite;
    public Sprite bloomSprite;

    public Player(bool ai,int plant, int color, char root, char fortifiedRoot, char invincibleRoot, char baseRoot, char deadRoot, char deadFortifiedRoot, char deadInvincibleRoot, char thorn, char strongFire, char weakFire, Tilemap rootMap, SpriteRenderer plantSprite, CardCollection collection)
    {
        this.ai = ai;
        this.plant = PlantConfigs.plantNames[plant];
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
        this.plantSprite = plantSprite;
        int[] deck = null;
        if (plant == 1)
        {
            deck = CardCollection.cherryBlossom;
            dormantSprite = collection.cherryBlossomSprites[0];
            bloomSprite = collection.cherryBlossomSprites[1];
        }
        else
        {
            deck = CardCollection.rose;
            dormantSprite = collection.roseSprites[0];
            bloomSprite = collection.roseSprites[1];
        }
        plantSprite.sprite = dormantSprite;
        plantSprite.color = PlantConfigs.plantColors[plant][color];
        rootMap.color = PlantConfigs.rootColors[plant][color];
        for (int i = 0; i < deck.Length; i++)
        {
            draw.Add(deck[i]);
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
        wormTurns = parent.wormTurns;
        scentTurns = parent.scentTurns;
        bloom = parent.bloom;
        festivals = parent.festivals;
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
        plantSprite = parent.plantSprite;
        dormantSprite = parent.dormantSprite;
        bloomSprite = parent.bloomSprite;
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
        foreach (int i in parent.freeCards)
        {
            freeCards.Add(i);
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

    public void DiscardCard(int index)
    {
        int i = 0;
        while (i < freeCards.Count)
        {
            if (freeCards[i] == index)
            {
                freeCards.RemoveAt(i);
                continue;
            }
            else if (freeCards[i] > index)
            {
                freeCards[i]--;
            }
            i++;
        }
        discard.Add(hand[index]);
        hand.RemoveAt(index);
    }
}
