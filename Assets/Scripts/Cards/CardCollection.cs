using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    public Sprite[] sprites;
    public AudioClip[] sounds;
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
        new WarCard(),
        new BloomCard(),
        new DormantCard(),
        new FestivalCard(),
        new PetalCard(),
        new SeasonCard()
    };

    public Sprite[] roseSprites;
    public Sprite[] cherryBlossomSprites;

    public static int[] rose = new int[20] { 1, 1, 1, 2, 2, 3, 4, 5, 6, 7, 8, 9, 9, 10, 10, 11, 11, 11, 12, 13 };
    public static int[] cherryBlossom = new int[20] { 1, 1, 1, 2, 2, 3, 4, 5, 6, 7, 8, 14, 14, 15, 15, 16, 17, 17, 18, 18 };
}
