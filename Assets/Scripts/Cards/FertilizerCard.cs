using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilizerCard: Card
{

    public static string type = "instant";

    public static bool Validation(int x, int y)
    {
        return true;
    }

    public static void Action(int x, int y)
    {

    }
}
