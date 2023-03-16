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
    [SerializeField] private TextMeshProUGUI phaseDisp;
    [SerializeField] private TextMeshProUGUI turnDisp;
    [SerializeField] private GameObject turnChange;
    public GameObject phaseChange;
    [SerializeField] private Button[] player1CardButtons;
    [SerializeField] private Button[] player2CardButtons;
    [SerializeField] private int[] costs;
    [SerializeField] private Button[] skipButtons;
    [SerializeField] private GameObject validCursor;
    [SerializeField] private GameObject invalidCursor;
    [SerializeField] private TextMeshProUGUI winnerDisp;
    [HideInInspector] public State state;

    // Start is called before the first frame update
    void Start()
    {
        state = new State();
        state.InitState(rootTile, deadRootTile, thornTile, strongFireTile, weakFireTile, waterDisps, pointDisps);
        turnChange.GetComponent<Animator>().Play("Sweep");
    }

    // Update is called once per frame
    void Update()
    {
        validCursor.transform.position = new Vector3(-40, 0, 0);
        invalidCursor.transform.position = new Vector3(-40, 0, 0);
        turnDisp.text = $"Turn {state.turn} / {state.maxTurns}";
        int[] coord = state.MouseToCoord();
        if (state.turn <= state.maxTurns)
        {
            if (state.card != null)
            {
                phaseDisp.text = state.card.GetName();
                if (state.card.IsInstant())
                {
                    string oldName = state.card.GetName();
                    state.card.Action(coord[0], coord[1]);
                    if (oldName != "Root Phase" && state.card.GetName() == "Root Phase")
                    {
                        ChangePhase();
                        phaseChange.GetComponent<Animator>().Play("Sweep");
                    }
                }
                else if (state.card.Validation(coord[0], coord[1]))
                {
                    validCursor.transform.position = state.CoordToWorld(coord[0], coord[1]);
                    if (Input.GetMouseButtonDown(0))
                    {
                        string oldName = state.card.GetName();
                        state.card.Action(coord[0], coord[1]);
                        if (state.card == null)
                        {
                            ChangeTurn();
                        }
                        else
                        {
                            ChangePhase();
                            if (oldName != "Root Phase" && state.card.GetName() == "Root Phase")
                            {
                                phaseChange.GetComponent<Animator>().Play("Sweep");
                            }
                        }
                    }
                }
                else
                {
                    invalidCursor.transform.position = state.CoordToWorld(coord[0], coord[1]);
                }
            }
            else
            {
                phaseDisp.text = "Choose a Card";
            }
        }
        else
        {
            turnDisp.text = $"Turn {state.maxTurns} / {state.maxTurns}";
            int winner = 0;
            if (state.players[0].points > state.players[1].points)
            {
                winner = 1;
            }
            else if (state.players[0].points < state.players[1].points)
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
        foreach (Player player in state.players)
        {
            player.waterDisp.text = $"{player.water}";
            player.pointsDisp.text = $"{player.points}";
        }
    }

    public void ChangeTurn()
    {
        if (state.thisPlayer == 1)
        {
            state.turn++;
        }

        for (int i = state.players[state.thisPlayer].weakFireIndex.Count - 1; i >= 0; i--)
        {
            state.SpreadFire(state.players[state.thisPlayer].weakFireIndex[i], '-', null);
            state.players[state.thisPlayer].weakFireIndex.RemoveAt(i);
        }
        for (int i = state.players[state.thisPlayer].strongFireIndex.Count - 1; i >= 0; i--)
        {
            state.SpreadFire(state.players[state.thisPlayer].strongFireIndex[i], state.players[state.thisPlayer].weakFire, State.weakFireTile);
            state.players[state.thisPlayer].strongFireIndex.RemoveAt(i);
        }

        skipButtons[state.thisPlayer].interactable = false;
        state.thisPlayer = state.otherPlayer;
        state.otherPlayer = 1 - state.thisPlayer;
        state.card = null;
        if (state.turn <= state.maxTurns)
        {
            turnChange.GetComponent<TextMeshProUGUI>().text = $"Player {state.thisPlayer + 1}'s Turn";
            turnChange.GetComponent<Animator>().Play("Sweep");
            Button[] cardButtons;
            if (state.thisPlayer == 0)
            {
                cardButtons = player1CardButtons;
            }
            else
            {
                cardButtons = player2CardButtons;
            }
            for (int i = 0; i < 4; i++)
            {
                if (state.players[state.thisPlayer].water >= costs[i])
                {
                    cardButtons[i].interactable = true;
                }
            }
            skipButtons[state.thisPlayer].interactable = true;
        }
    }

    public void ChangePhase(bool sweep = true)
    {
        Button[] cardButtons;
        if (state.thisPlayer == 0)
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
