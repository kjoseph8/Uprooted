using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RuleTile rootTile;
    [SerializeField] private RuleTile deadRootTile;
    [SerializeField] private TextMeshProUGUI[] waterDisps;
    [SerializeField] private TextMeshProUGUI[] pointDisps;
    [SerializeField] private TextMeshProUGUI turnDisp;
    [SerializeField] private TextMeshProUGUI winnerDisp;

    // Start is called before the first frame update
    void Start()
    {
        State.InitState(rootTile, deadRootTile, waterDisps, pointDisps);
    }

    // Update is called once per frame
    void Update()
    {
        turnDisp.text = $"Turn {State.turn} / 30";
        int[] coord = State.MouseToCoord();
        if (State.turn <= 30)
        {
            if (State.card != null)
            {
                if (State.card.isInstant())
                {
                    State.card.Action(coord[0], coord[1]);
                }
                else if (State.card.Validation(coord[0], coord[1]))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        State.card.Action(coord[0], coord[1]);
                    }
                }
            }
        }
        else
        {
            turnDisp.text = "Turn 30 / 30";
            int winner = 0;
            if (State.players[0].points > State.players[1].points)
            {
                winner = 1;
            }
            else if (State.players[0].points < State.players[1].points)
            {
                winner = 2;
            }
            if (winner == 0)
            {
                winnerDisp.text = "It's a Draw.";
            }
            else
            {
                winnerDisp.text = $"Player {winner} wins!";
            }
            winnerDisp.enabled = true;
        }
        foreach (Player player in State.players)
        {
            player.waterDisp.text = $"{player.water}";
            player.pointsDisp.text = $"{player.points}";
        }
    }

    public void SetCard(string name)
    {
        if (name == "root")
        {
            State.card = new RootCard();
        }
        else if (name == "fertilizer")
        {
            State.card = new FertilizerCard();
        }
        else if (name == "aphid")
        {
            State.card = new AphidCard();
        }
    }
}
