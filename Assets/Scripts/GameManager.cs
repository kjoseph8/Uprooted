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
    [SerializeField] private TileBase woodShieldTile;
    [SerializeField] private TileBase metalShieldTile;
    [SerializeField] private TileBase thornTile;
    [SerializeField] private TileBase strongFireTile;
    [SerializeField] private TileBase weakFireTile;
    [SerializeField] private Plant[] plants;
    [SerializeField] private TextMeshProUGUI[] waterDisps;
    [SerializeField] private TextMeshProUGUI[] rootCountDisps;
    [SerializeField] private TextMeshProUGUI[] rockCountDisps;
    [SerializeField] private TextMeshProUGUI[] rootMoveDisps;
    [SerializeField] private TextMeshProUGUI phaseDisp;
    [SerializeField] private TextMeshProUGUI turnDisp;
    [SerializeField] private GameObject turnChange;
    [SerializeField] private Button[] p1Hand;
    [SerializeField] private Button[] p2Hand;
    [SerializeField] private Button[] rootButtons;
    [SerializeField] private Button[] endButtons;
    [SerializeField] private Button[] playButtons;
    [SerializeField] private Button[] discardButtons;
    [SerializeField] private Button[] cancelButtons;
    [SerializeField] private Button[] compostButtons;
    [SerializeField] private Image[] selectedImages;
    [SerializeField] private GameObject validCursor;
    [SerializeField] private GameObject invalidCursor;
    [SerializeField] private TextMeshProUGUI winnerDisp;
    [SerializeField] private Tilemap highlightMap;
    [SerializeField] private Tile highlightTile;
    [HideInInspector] public State state;
    private bool discardPhase = false;
    private bool tilePhase = false;
    private int[] hoveredIndexes = new int[] { -1, -1 };

    // Start is called before the first frame update
    void Start()
    {
        state = new State();
        state.InitState(rootTile, deadRootTile, woodShieldTile, metalShieldTile, thornTile, strongFireTile, weakFireTile, plants);
        ChangeTurn();
    }

    // Update is called once per frame
    void Update()
    {
        validCursor.transform.position = new Vector3(-40, 0, 0);
        invalidCursor.transform.position = new Vector3(-40, 0, 0);
        turnDisp.text = $"Turn {state.turn} / {state.maxTurns}";
        UpdateCards();
        UpdateHighlights();
        int[] coord = state.MouseToCoord();
        int index = state.CoordToIndex(coord[0], coord[1]);
        if (state.turn <= state.maxTurns)
        {
            if (state.card != null && !discardPhase && tilePhase)
            {
                if (state.card.Validation(state, index))
                {
                    validCursor.transform.position = state.CoordToWorld(coord[0], coord[1]);
                    if (Input.GetMouseButtonDown(0))
                    {
                        state.card.Action(state, index);
                        state.numActions--;
                        if (state.numActions == 0)
                        {
                            state.card = null;
                            state.cardIndex = -1;
                            tilePhase = false;
                        }
                    }
                }
                else
                {
                    invalidCursor.transform.position = state.CoordToWorld(coord[0], coord[1]);
                }
            }
        }
        else
        {
            EndGame();
        }

        for (int playerIndex = 0; playerIndex < 2; playerIndex++)
        {
            Player player = state.players[playerIndex];
            player.rootCount = 0;
            player.rockCount = 0;
            for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
            {
                if (state.board[i] == player.root || state.board[i] == player.fortifiedRoot || state.board[i] == player.invincibleRoot || state.board[i] == player.baseRoot)
                {
                    player.rootCount++;
                }
                else if (state.board[i] == 'R' && state.CheckRock(i, player))
                {
                    player.rockCount++;
                }
            }
            waterDisps[playerIndex].text = $"{player.water}";
            rootCountDisps[playerIndex].text = $"{player.rootCount}";
            rockCountDisps[playerIndex].text = $"{player.rockCount}";
            rootMoveDisps[playerIndex].text = $"{player.rootMoves}";
        }
    }

    public void PrintDebug()
    {
        Debug.Log(state.StateToString());
    }

    public void ChangeTurn()
    {
        if (state.thisPlayer == 1)
        {
            state.turn++;
        }

        ForestFireCard.UpdateFire(state);
        tilePhase = false;
        state.thisPlayer = state.otherPlayer;
        state.otherPlayer = 1 - state.thisPlayer;
        rootButtons[state.otherPlayer].interactable = false;
        endButtons[state.otherPlayer].interactable = false;
        playButtons[state.otherPlayer].gameObject.SetActive(false);
        cancelButtons[state.otherPlayer].gameObject.SetActive(false);
        compostButtons[state.otherPlayer].gameObject.SetActive(false);
        state.card = null;
        state.cardIndex = -1;

        if (state.turn == state.maxTurns && state.thisPlayer == 0)
        {
            for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
            {
                int[] coords = state.IndexToCoord(i);
                int x = coords[0];
                int y = coords[1];

                if (state.board[i] == state.players[0].root || state.board[i] == state.players[0].fortifiedRoot)
                {
                    state.board[i] = state.players[0].invincibleRoot;
                    State.otherMap.SetTile(new Vector3Int(x, y), metalShieldTile);
                }
                else if (state.board[i] == state.players[1].root || state.board[i] == state.players[1].fortifiedRoot)
                {
                    state.board[i] = state.players[1].invincibleRoot;
                    State.otherMap.SetTile(new Vector3Int(x, y), metalShieldTile);
                }
                else if (state.board[i] == state.players[0].deadRoot || state.board[i] == state.players[0].deadFortifiedRoot)
                {
                    state.board[i] = state.players[0].deadInvincibleRoot;
                    State.otherMap.SetTile(new Vector3Int(x, y), metalShieldTile);
                }
                else if (state.board[i] == state.players[1].deadRoot || state.board[i] == state.players[1].deadFortifiedRoot)
                {
                    state.board[i] = state.players[1].deadInvincibleRoot;
                    State.otherMap.SetTile(new Vector3Int(x, y), metalShieldTile);
                }
            }
        }

        if (state.turn <= state.maxTurns)
        {
            state.players[state.thisPlayer].water += (state.turn - 1) / 5 + 1;
            state.players[state.thisPlayer].rootMoves = (state.turn - 1) / 5 + 1;

            if (state.players[state.thisPlayer].scentTurns > 0)
            {
                state.players[state.thisPlayer].water++;
                state.players[state.thisPlayer].scentTurns--;
            }

            turnChange.GetComponent<TextMeshProUGUI>().text = $"Player {state.thisPlayer + 1}'s Turn";
            turnChange.GetComponent<Animator>().Play("Sweep");
            Player player = state.players[state.thisPlayer];
            for (int i = player.hand.Count; i < 5; i++)
            {
                player.DrawCard();
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
                cardButtons[i].interactable = true;
                cardButtons[i].image.sprite = state.players[state.thisPlayer].plant.GetSprites()[hand[i]];
                int defaultI = 2 * i;
                int rotI = hand.Count - 1;
                float angle = (defaultI - rotI) * Mathf.PI / 36;
                cardButtons[i].transform.rotation = new Quaternion(0, 0, Mathf.Sin(angle / 2), Mathf.Cos(angle / 2));
            }
            else
            {
                cardButtons[i].gameObject.SetActive(false);
            }
        }

        if (discardPhase)
        {
            phaseDisp.text = "Too many cards. Discard a card";
            rootButtons[state.thisPlayer].interactable = false;
            endButtons[state.thisPlayer].interactable = false;
            playButtons[state.thisPlayer].gameObject.SetActive(false);
            discardButtons[state.thisPlayer].gameObject.SetActive(true);
            cancelButtons[state.thisPlayer].gameObject.SetActive(true);
            compostButtons[state.thisPlayer].gameObject.SetActive(false);
        }
        else if (tilePhase)
        {
            if (state.card != null)
            {
                phaseDisp.text = state.card.GetName();
            }
            else
            {
                phaseDisp.text = "Select a Tile";
            }
            rootButtons[state.thisPlayer].interactable = state.players[state.thisPlayer].rootMoves > 0;
            endButtons[state.thisPlayer].interactable = true;
            playButtons[state.thisPlayer].gameObject.SetActive(false);
            discardButtons[state.thisPlayer].gameObject.SetActive(false);
            cancelButtons[state.thisPlayer].gameObject.SetActive(false);
            compostButtons[state.thisPlayer].gameObject.SetActive(false);
        }
        else if (state.card != null)
        {
            phaseDisp.text = state.card.GetName();
            rootButtons[state.thisPlayer].interactable = state.players[state.thisPlayer].rootMoves > 0;
            endButtons[state.thisPlayer].interactable = true;
            playButtons[state.thisPlayer].gameObject.SetActive(true);
            if (state.card.GetNumActions(state) == 0)
            {
                playButtons[state.thisPlayer].interactable = state.players[state.thisPlayer].water >= state.card.GetCost(state) && state.card.Validation(state, 0);
            }
            else
            {
                playButtons[state.thisPlayer].interactable = state.players[state.thisPlayer].water >= state.card.GetCost(state);
            }
            discardButtons[state.thisPlayer].gameObject.SetActive(false);
            cancelButtons[state.thisPlayer].gameObject.SetActive(true);
            compostButtons[state.thisPlayer].gameObject.SetActive(true);
            compostButtons[state.thisPlayer].interactable = state.players[state.thisPlayer].water > 0;
        }
        else
        {
            phaseDisp.text = "Select a Card or Place a Root";
            rootButtons[state.thisPlayer].interactable = state.players[state.thisPlayer].rootMoves > 0;
            endButtons[state.thisPlayer].interactable = true;
            playButtons[state.thisPlayer].gameObject.SetActive(false);
            discardButtons[state.thisPlayer].gameObject.SetActive(false);
            cancelButtons[state.thisPlayer].gameObject.SetActive(false);
            compostButtons[state.thisPlayer].gameObject.SetActive(false);
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
        if (i >= 0)
        {
            Player player = state.players[state.thisPlayer];
            state.card = player.plant.GetCards()[player.hand[i]];
            state.cardIndex = player.hand[i];
            tilePhase = false;
        }
        else
        {
            state.card = new RootCard();
            state.cardIndex = -1;
            tilePhase = true;
        }
        state.numActions = state.card.GetNumActions(state);
    }

    public void PlayCard()
    {
        Player player = state.players[state.thisPlayer];
        player.water -= state.card.GetCost(state);
        player.hand.Remove(state.cardIndex);
        player.discard.Add(state.cardIndex);
        if (state.card.GetNumActions(state) == 0)
        {
            state.numActions = 0;
            state.card.Action(state, 0);
            if (state.numActions == 0)
            {
                state.card = null;
                state.cardIndex = -1;
            }
            else
            {
                tilePhase = true;
            }
        }
        else
        {
            tilePhase = true;
        }
    }

    public void DiscardCard()
    {
        Player player = state.players[state.thisPlayer];
        player.hand.Remove(state.cardIndex);
        player.discard.Add(state.cardIndex);
        state.card = null;
        state.cardIndex = -1;
        discardPhase = false;
    }

    public void CancelCard()
    {
        state.card = null;
        state.cardIndex = -1;
    }

    public void CompostCard()
    {
        Player player = state.players[state.thisPlayer];
        player.water--;
        player.hand.Remove(state.cardIndex);
        player.discard.Add(state.cardIndex);
        state.card = null;
        state.cardIndex = -1;
        player.DrawCard();
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

    private void UpdateHighlights()
    {
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);
            int x = coords[0];
            int y = coords[1];

            if (!discardPhase && state.card != null && state.card.GetNumActions(state) > 0 && state.card.Validation(state, i))
            {
                highlightMap.SetTile(new Vector3Int(x, y), highlightTile);
            }
            else
            {
                highlightMap.SetTile(new Vector3Int(x, y), null);
            }
        }
    }

    private void EndGame()
    {
        turnDisp.text = $"Turn {state.maxTurns} / {state.maxTurns}";

        int p1Points = state.players[0].rockCount;
        int p2Points = state.players[1].rockCount;

        if (state.players[0].rootCount > state.players[1].rootCount)
        {
            p1Points += 2;
        }
        else if (state.players[0].rootCount < state.players[1].rootCount)
        {
            p2Points += 2;
        }

        if (state.players[0].water > state.players[1].water)
        {
            p1Points += 1;
        }
        else if (state.players[0].water < state.players[1].water)
        {
            p2Points += 1;
        }

        int winner = 0;
        if (p1Points > p2Points)
        {
            winner = 1;
        }
        else if (p1Points < p2Points)
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
}
