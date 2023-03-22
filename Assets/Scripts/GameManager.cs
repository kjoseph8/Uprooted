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
    [SerializeField] private Plant[] plants;
    [SerializeField] private TextMeshProUGUI[] waterDisps;
    [SerializeField] private TextMeshProUGUI phaseDisp;
    [SerializeField] private TextMeshProUGUI turnDisp;
    [SerializeField] private GameObject turnChange;
    [SerializeField] private Button[] p1Hand;
    [SerializeField] private Button[] p2Hand;
    [SerializeField] private Button[] rootButtons;
    [SerializeField] private Button[] endButtons;
    [SerializeField] private Button[] playButtons;
    [SerializeField] private Image[] selectedImages;
    [SerializeField] private GameObject validCursor;
    [SerializeField] private GameObject invalidCursor;
    [SerializeField] private TextMeshProUGUI winnerDisp;
    [HideInInspector] public State state;
    private string phase;
    private int[] hoveredIndexes = new int[] { -1, -1 };

    // Start is called before the first frame update
    void Start()
    {
        state = new State();
        state.InitState(rootTile, deadRootTile, thornTile, strongFireTile, weakFireTile, plants, waterDisps);
        ChangeTurn();
    }

    // Update is called once per frame
    void Update()
    {
        validCursor.transform.position = new Vector3(-40, 0, 0);
        invalidCursor.transform.position = new Vector3(-40, 0, 0);
        turnDisp.text = $"Turn {state.turn} / {state.maxTurns}";
        playButtons[state.thisPlayer].gameObject.SetActive(false);
        UpdateCards();
        int[] coord = state.MouseToCoord();
        int index = state.CoordToIndex(coord[0], coord[1]);
        if (state.turn <= state.maxTurns)
        {
            if (state.card != null)
            {
                phaseDisp.text = state.card.GetName();
                if (state.card.GetNumActions(state) == 0)
                {
                    playButtons[state.thisPlayer].gameObject.SetActive(true);
                }
                else if (state.card.Validation(state, index))
                {
                    validCursor.transform.position = state.CoordToWorld(coord[0], coord[1]);
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (state.numActions == state.card.GetNumActions(state))
                        {
                            Player player = state.players[state.thisPlayer];
                            player.water -= state.card.GetCost(state);
                            player.hand.Remove(state.cardIndex);
                            player.discard.Add(state.cardIndex);
                        }
                        state.card.Action(state, index);
                        state.numActions--;
                        if (state.numActions == 0)
                        {
                            state.card = null;
                            state.cardIndex = -1;
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

        state.thisPlayer = state.otherPlayer;
        state.otherPlayer = 1 - state.thisPlayer;
        rootButtons[state.otherPlayer].interactable = false;
        endButtons[state.otherPlayer].interactable = false;
        playButtons[state.otherPlayer].gameObject.SetActive(false);
        state.card = null;
        state.cardIndex = -1;
        state.players[state.thisPlayer].rootMoves = 1;
        if (state.turn <= state.maxTurns)
        {
            turnChange.GetComponent<TextMeshProUGUI>().text = $"Player {state.thisPlayer + 1}'s Turn";
            turnChange.GetComponent<Animator>().Play("Sweep");
            Player player = state.players[state.thisPlayer];
            if (player.hand.Count < 5)
            {
                for (int i = player.hand.Count; i < 5; i++)
                {
                    player.DrawCard();
                }
            }
            else
            {
                player.DrawCard();
                phase = "Discard";
            }
            rootButtons[state.thisPlayer].interactable = true;
            endButtons[state.thisPlayer].interactable = true;
        }
    }

    private void UpdateCards()
    {
        Button[] cardButtons;
        Button[] otherButtons;
        if (state.thisPlayer == 0)
        {
            cardButtons = p1Hand;
            otherButtons = p2Hand;
        }
        else
        {
            cardButtons = p2Hand;
            otherButtons = p1Hand;
        }

        List<int> hand = state.players[state.thisPlayer].hand;
        for (int i = 0; i < cardButtons.Length; i++)
        {
            otherButtons[i].interactable = false;
            if (i < hand.Count)
            {
                cardButtons[i].gameObject.SetActive(true);
                cardButtons[i].image.sprite = state.players[state.thisPlayer].plant.GetSprites()[hand[i]];
                int defaultI = 2 * (i % 5);
                int rotI = hand.Count - 1;
                if (i >= 5)
                {
                    rotI -= 5;
                }
                else if (rotI > 4)
                {
                    rotI = 4;
                }
                float angle = (defaultI - rotI) * Mathf.PI / 48;
                cardButtons[i].transform.rotation = new Quaternion(0, 0, Mathf.Sin(angle), Mathf.Cos(angle));
                if (state.players[state.thisPlayer].water < state.players[state.thisPlayer].plant.GetCards()[hand[i]].GetCost(state))
                {
                    cardButtons[i].interactable = false;
                }
                else
                {
                    cardButtons[i].interactable = true;
                }
            }
            else
            {
                cardButtons[i].gameObject.SetActive(false);
            }
        }

        if (hoveredIndexes[state.thisPlayer] != -1)
        {
            selectedImages[state.thisPlayer].sprite = state.players[state.thisPlayer].plant.GetSprites()[hoveredIndexes[state.thisPlayer]];
            selectedImages[state.thisPlayer].gameObject.SetActive(true);
        }
        else if (state.cardIndex != -1)
        {
            selectedImages[state.thisPlayer].sprite = state.players[state.thisPlayer].plant.GetSprites()[state.cardIndex];
            selectedImages[state.thisPlayer].gameObject.SetActive(true);
        }
        else
        {
            selectedImages[state.thisPlayer].gameObject.SetActive(false);
        }

        if (hoveredIndexes[state.otherPlayer] != -1)
        {
            selectedImages[state.otherPlayer].sprite = state.players[state.otherPlayer].plant.GetSprites()[hoveredIndexes[state.otherPlayer]];
            selectedImages[state.otherPlayer].gameObject.SetActive(true);
        }
        else
        {
            selectedImages[state.otherPlayer].gameObject.SetActive(false);
        }
    }

    public void SetCard(int i)
    {
        if (i >= 0 && i < 10)
        {
            Player player = state.players[state.thisPlayer];
            state.card = player.plant.GetCards()[player.hand[i]];
            state.cardIndex = player.hand[i];
            state.numActions = state.card.GetNumActions(state);
        }
        else
        {
            state.card = new RootCard();
            state.cardIndex = -1;
            state.numActions = state.card.GetNumActions(state);
        }
    }

    public void PlayCard()
    {
        playButtons[state.thisPlayer].gameObject.SetActive(false);
        Player player = state.players[state.thisPlayer];
        player.water -= state.card.GetCost(state);
        player.hand.Remove(state.cardIndex);
        player.discard.Add(state.cardIndex);
        state.card.Action(state, 0);
        state.card = null;
        state.cardIndex = -1;
    }

    public void SetP1HoverIndex(int index)
    {
        if (index != -1)
        {
            hoveredIndexes[0] = state.players[0].hand[index];
        }
        else
        {
            hoveredIndexes[0] = -1;
        }
    }

    public void SetP2HoverIndex(int index)
    {
        if (index != -1)
        {
            hoveredIndexes[1] = state.players[1].hand[index];
        }
        else
        {
            hoveredIndexes[1] = -1;
        }
    }
}
