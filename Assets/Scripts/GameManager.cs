using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TileBase rootTile;
    [SerializeField] private TileBase deadRootTile;
    [SerializeField] private TileBase thornTile;
    [SerializeField] private TileBase strongFireTile;
    [SerializeField] private TileBase weakFireTile;
    [SerializeField] private TextMeshProUGUI[] waterDisps;
    [SerializeField] private TextMeshProUGUI[] pointDisps;
    [SerializeField] private TextMeshProUGUI turnDisp;
    [SerializeField] private GameObject turnChange;
    [SerializeField] private GameObject phaseChange;
    [SerializeField] private Button[] player1CardButtons;
    [SerializeField] private Button[] player2CardButtons;
    [SerializeField] private int[] costs;
    [SerializeField] private Button[] skipButtons;
    [SerializeField] private TextMeshProUGUI winnerDisp;

    // Start is called before the first frame update
    void Start()
    {
        State.InitState(rootTile, deadRootTile, thornTile, strongFireTile, weakFireTile, waterDisps, pointDisps);
        turnChange.GetComponent<Animator>().Play("Sweep");
    }

    // Update is called once per frame
    void Update()
    {
        turnDisp.text = $"Turn {State.turn} / {State.maxTurns}";
        int[] coord = State.MouseToCoord();
        if (State.turn <= State.maxTurns)
        {
            if (State.card != null)
            {
                if (State.card.IsInstant())
                {
                    string oldName = State.card.GetName();
                    State.card.Action(coord[0], coord[1]);
                    if (oldName != "root" && State.card.GetName() == "root")
                    {
                        ChangePhase();
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
                            ChangeTurn();
                        }
                        else
                        {
                            ChangePhase();
                            if (oldName != "root" && State.card.GetName() == "root")
                            {
                                phaseChange.GetComponent<Animator>().Play("Sweep");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            turnDisp.text = $"Turn {State.maxTurns} / {State.maxTurns}";
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
                ChangeTurn();
            }
            else
            {
                State.card = new RootCard();
                ChangePhase();
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
        else if (name == "thorns")
        {
            State.card = new ThornsCard();
        }
        else if (name == "sunshine")
        {
            State.card = new SunshineCard();
        }
    }

    private void ChangeTurn()
    {
        if (State.player == 1)
        {
            State.turn++;
        }

        for (int i = State.players[State.player].weakFireIndex.Count - 1; i >= 0; i--)
        {
            State.SpreadFire(State.players[State.player].weakFireIndex[i], '-', null);
            State.players[State.player].weakFireIndex.RemoveAt(i);
        }
        for (int i = State.players[State.player].strongFireIndex.Count - 1; i >= 0; i--)
        {
            State.SpreadFire(State.players[State.player].strongFireIndex[i], State.players[State.player].weakFire, State.weakFireTile);
            State.players[State.player].strongFireIndex.RemoveAt(i);
        }

        skipButtons[State.player].interactable = false;
        State.player = (State.player + 1) % 2;
        State.card = null;
        if (State.turn <= State.maxTurns)
        {
            turnChange.GetComponent<TextMeshProUGUI>().text = $"Player {State.player + 1}'s Turn";
            turnChange.GetComponent<Animator>().Play("Sweep");
            Button[] cardButtons;
            if (State.player == 0)
            {
                cardButtons = player1CardButtons;
            }
            else
            {
                cardButtons = player2CardButtons;
            }
            for (int i = 0; i < 4; i++)
            {
                if (State.players[State.player].water >= costs[i])
                {
                    cardButtons[i].interactable = true;
                }
            }
            skipButtons[State.player].interactable = true;
        }
    }

    private void ChangePhase(bool sweep = true)
    {
        Button[] cardButtons;
        if (State.player == 0)
        {
            cardButtons = player1CardButtons;
        }
        else
        {
            cardButtons = player2CardButtons;
        }
        foreach (Button button in cardButtons)
        {
            button.interactable = false;
        }
    }
}
