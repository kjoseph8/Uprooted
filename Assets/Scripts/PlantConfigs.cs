using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantConfigs
{
    public static string[] plantNames = new string[]
    {
        "Rose",
        "Cherry Blossom"
    };

    public static Color[][] buttonColors = new Color[][]
    {
        new Color[] {
            new Color(0.6784314f, 0.1137255f, 0.3647059f),
            new Color(0, 0.1176471f, 1)
        },
        new Color[]
        {
            new Color(1, 0.7882354f, 0.9176471f),
            new Color(1, 0.937255f, 0.7333333f)
        }
    };

    public static Color[][] plantColors = new Color[][]
    {
        new Color[]
        {
            new Color(0.6784f, 0.4863f, 0.3647f),
            new Color(0, 0.5019608f, 1)
        },
        new Color[]
        {
            new Color(1, 0.8078f, 0.9255f),
            new Color(1, 1, 0.75f)
        }
    };

    public static Color[][] rootColors = new Color[][]
    {
        new Color[]
        {
            new Color(0.0941f, 0.4863f, 0.2431f),
            new Color(0.024478f, 0.4862745f, 0.4522256f)
        },
        new Color[]
        {
            new Color(0.3607843f, 0.2431373f, 0.1490196f),
            new Color(0.3607843f, 0.3098039f, 0.1137255f)
        }
    };
}
