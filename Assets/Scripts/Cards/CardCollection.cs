using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    public Sprite[] sprites;
    [HideInInspector]
    public Card[] cards = new Card[]
    {
        new RootCard(),
        new FertilizerCard(),
        new AphidCard(),
        new BirdCard(),
        new MrWormCard(),
        new SunshineCard(),
        new SaplingCard(),
        new ThickRootCard(),
        new ForestFireCard(),
        new BeeCard(),
        new LoveCard(),
        new ThornsCard(),
        new ScentCard(),
        new WarCard()
    };

    public static int[] rose = new int[] { 1, 1, 2, 2, 3, 4, 5, 6, 7, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13 };
}
