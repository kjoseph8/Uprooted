using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rose : Plant
{
    [SerializeField] private Sprite[] sprites;

    public override Card[] GetCards()
    {
        return new Card[]
        {
            new FertilizerCard(),
            new FertilizerCard(),
            new AphidCard(),
            new AphidCard(),
            new BirdCard(),
            new BirdCard(),
            new ThickRootCard(),
            new ThickRootCard(),
            new ForestFireCard(),
            new ForestFireCard(),
            new BeeCard(),
            new BeeCard(),
            new LoveCard(),
            new LoveCard(),
            new ThornsCard(),
            new ThornsCard(),
            new ScentCard(),
            new ScentCard(),
            new RoseWarCard(),
            new RoseWarCard()
        };
    }

    public override Sprite[] GetSprites()
    {
        return sprites;
    }
}
