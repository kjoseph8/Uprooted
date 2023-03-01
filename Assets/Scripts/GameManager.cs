using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RuleTile rootTile;
    [SerializeField] private TextMeshProUGUI[] waterDisps;
    [SerializeField] private TextMeshProUGUI[] pointDisps;
    [SerializeField] private TextMeshProUGUI turnDisp;
    [SerializeField] private TextMeshProUGUI winnerDisp;

    // Start is called before the first frame update
    void Start()
    {
        State.InitState(rootTile, waterDisps, pointDisps);
    }

    // Update is called once per frame
    void Update()
    {
        turnDisp.text = $"Turn {State.turn} / 30";
        int[] coord = State.MouseToCoord();
        if (State.turn <= 30)
        {
            if (Input.GetMouseButtonDown(0) && RootCard.Validation(coord[0], coord[1]))
            {
                RootCard.Action(coord[0], coord[1]);
                if (State.player == 1)
                {
                    State.turn++;
                }
                State.player = (State.player + 1) % 2;
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
}
