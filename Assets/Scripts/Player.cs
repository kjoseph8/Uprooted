using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Player
{
    public int water = 2;
    public int points = 0;
    public int rootMoves = 1;
    public char root;
    public char baseRoot;
    public char deadRoot;
    public char strongFire;
    public List<int> strongFireIndex = new List<int>();
    public char weakFire;
    public List<int> weakFireIndex = new List<int>();
    public Tilemap rootMap;
    public TextMeshProUGUI waterDisp;
    public TextMeshProUGUI pointsDisp;

    public Player(char root, char baseRoot, char deadRoot, char strongFire, char weakFire, string rootMapTag, TextMeshProUGUI waterDisp, TextMeshProUGUI pointsDisp)
    {
        this.root = root;
        this.baseRoot = baseRoot;
        this.deadRoot = deadRoot;
        this.strongFire = strongFire;
        this.weakFire = weakFire;
        this.rootMap = GameObject.FindGameObjectWithTag(rootMapTag).GetComponent<Tilemap>();
        this.waterDisp = waterDisp;
        this.pointsDisp = pointsDisp;
    }
}
