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
    [SerializeField] private GameObject turnChange;
    [SerializeField] private GameObject phaseChange;
    [SerializeField] private TextMeshProUGUI winnerDisp;

    // Start is called before the first frame update
    void Start()
    {
        State.InitState(rootTile, deadRootTile, waterDisps, pointDisps);
        turnChange.GetComponent<Animator>().Play("Sweep");
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
                if (State.card.IsInstant())
                {
                    string oldName = State.card.GetName();
                    State.card.Action(coord[0], coord[1]);
                    if (oldName != "root" && State.card.GetName() == "root")
                    {
                        phaseChange.GetComponent<Animator>().Play("Sweep");
                    }
                }
                else if (State.card.Validation(coord[0], coord[1]))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        string oldName = State.card.GetName();
                        State.card.Action(coord[0], coord[1]);
                        if (State.card == null)
                        {
                            if (State.player == 1)
                            {
                                State.turn++;
                            }
                            State.player = (State.player + 1) % 2;
                            if (State.turn <= 30)
                            {
                                turnChange.GetComponent<TextMeshProUGUI>().text = $"Player {State.player + 1}'s Turn";
                                turnChange.GetComponent<Animator>().Play("Sweep");
                            }
                        }
                        else if (oldName != "root" && State.card.GetName() == "root")
                        {
                            phaseChange.GetComponent<Animator>().Play("Sweep");
                        }
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
            if (State.card != null && State.card.GetName() == "root")
            {
                if (State.player == 1)
                {
                    State.turn++;
                }
                State.player = (State.player + 1) % 2;
                State.card = null;
                if (State.turn <= 30)
                {
                    turnChange.GetComponent<TextMeshProUGUI>().text = $"Player {State.player + 1}'s Turn";
                    turnChange.GetComponent<Animator>().Play("Sweep");
                }
            }
            else
            {
                State.card = new RootCard();
                phaseChange.GetComponent<Animator>().Play("Sweep");
            }
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
