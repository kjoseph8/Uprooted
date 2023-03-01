using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Player
{
    public int water = 2;
    public int points = 0;
    public char root;
    public char baseRoot;
    public char deadRoot;
    public Tilemap rootMap;
    public TextMeshProUGUI waterDisp;
    public TextMeshProUGUI pointsDisp;

    public Player(char root, char baseRoot, char deadRoot, string rootMapTag, TextMeshProUGUI waterDisp, TextMeshProUGUI pointsDisp)
    {
        this.root = root;
        this.baseRoot = baseRoot;
        this.deadRoot = deadRoot;
        this.rootMap = GameObject.FindGameObjectWithTag(rootMapTag).GetComponent<Tilemap>();
        this.waterDisp = waterDisp;
        this.pointsDisp = pointsDisp;
    }
}
