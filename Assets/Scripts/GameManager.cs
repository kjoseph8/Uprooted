using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CardCollection collection;
    [SerializeField] private MiniMaxAI gameAI;
    [SerializeField] private TileBase rootTile;
    [SerializeField] private TileBase deadRootTile;
    [SerializeField] private TileBase seedTile;
    [SerializeField] private TileBase woodShieldTile;
    [SerializeField] private TileBase metalShieldTile;
    [SerializeField] private TileBase thornTile;
    [SerializeField] private TileBase strongFireTile;
    [SerializeField] private TileBase weakFireTile;
    [SerializeField] private TextMeshProUGUI[] waterDisps;
    [SerializeField] private TextMeshProUGUI[] rootCountDisps;
    [SerializeField] private TextMeshProUGUI[] rockCountDisps;
    [SerializeField] private TextMeshProUGUI[] pointDisps;
    [SerializeField] private TextMeshProUGUI[] rootMoveDisps;
    [SerializeField] private GameObject phaseDispBG;
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
    [SerializeField] private GameObject[] disabledMsgBackgrounds;
    [SerializeField] private TextMeshProUGUI[] disabledMessages;
    [SerializeField] private Sprite rootCardSprite;
    [SerializeField] private GameObject validCursor;
    [SerializeField] private GameObject invalidCursor;
    [SerializeField] private TextMeshProUGUI winnerDisp;
    [SerializeField] private Animator tornadoAnim;
    [SerializeField] private Animator[] defeatAnims;
    [SerializeField] private Tilemap highlightMap;
    [SerializeField] private Tile highlightTile;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private ParticleSystem rain;
    [HideInInspector] public State state;
    private bool gameEnded = false;
    private int[] hoveredIndexes = new int[] { -1, -1 };
    private float turnFactor = 0;
    private float factorSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        state = new State(collection, rootTile, deadRootTile, seedTile, woodShieldTile, metalShieldTile, thornTile, strongFireTile, weakFireTile);
        ChangeTurn(true);
    }

    // Update is called once per frame
    void Update()
    {
        validCursor.transform.position = new Vector3(-40, 0, 0);
        invalidCursor.transform.position = new Vector3(-40, 0, 0);
        turnDisp.text = $"Turn {Mathf.Min(state.turn, state.maxTurns)} / {state.maxTurns}";
        float factor = (float)state.turn / state.maxTurns;
        if (factor > turnFactor)
        {
            turnFactor += factorSpeed * Time.deltaTime;
        }
        else
        {
            turnFactor = factor;
        }
        background.color = Color.Lerp(new Color(0.65f, 0.85f, 0.95f, 1), new Color(0, 0.1f, 0.2f, 1), turnFactor);
        backgroundMusic.pitch = 0.75f * (turnFactor + 1);
        var shape = rain.shape;
        shape.rotation = new Vector3(0, 180 - 30 * turnFactor, 0);
        var emission = rain.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(25 + 475 * turnFactor);
        UpdateCards();
        UpdateHighlights();
        int[] coord = state.MouseToCoord();
        int index = state.CoordToIndex(coord[0], coord[1]);
        if (state.turn <= state.maxTurns)
        {
            if (state.card != null && !state.discardPhase && state.tilePhase && !state.players[state.thisPlayer].ai)
            {
                if (state.card.Validation(state, index))
                {
                    validCursor.transform.position = state.CoordToWorld(coord[0], coord[1]);
                    if (Input.GetMouseButtonDown(0))
                    {
                        state.PlayTile(index);
                    }
                }
                else
                {
                    invalidCursor.transform.position = state.CoordToWorld(coord[0], coord[1]);
                }
            }
        }
        else if (!gameEnded)
        {
            EndGame();
        }

        state.UpdatePoints();
        for (int playerIndex = 0; playerIndex < 2; playerIndex++)
        {
            Player player = state.players[playerIndex];
            waterDisps[playerIndex].text = $"{player.water}";
            rootCountDisps[playerIndex].text = $"{player.rootCount}";
            rockCountDisps[playerIndex].text = $"{player.rockCount}";
            pointDisps[playerIndex].text = $"{player.points}";
            rootMoveDisps[playerIndex].text = $"{player.rootMoves}";
        }
    }

    public void PrintDebug()
    {
        Debug.Log(state.StateToString());
    }

    public void ChangeTurn(bool ai)
    {
        if (ai || !state.players[state.thisPlayer].ai)
        {
            state.ChangeTurn();

            if (state.turn <= state.maxTurns)
            {
                turnChange.GetComponent<TextMeshProUGUI>().text = $"Player {state.thisPlayer + 1}'s Turn";
                turnChange.GetComponent<Animator>().Play("Sweep");
                if (state.players[state.thisPlayer].ai)
                {
                    StartCoroutine(gameAI.Control(state, this));
                }
            }
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

        rootButtons[state.otherPlayer].interactable = false;
        endButtons[state.otherPlayer].interactable = false;
        playButtons[state.otherPlayer].gameObject.SetActive(false);
        discardButtons[state.otherPlayer].gameObject.SetActive(false);
        cancelButtons[state.otherPlayer].gameObject.SetActive(false);
        compostButtons[state.otherPlayer].gameObject.SetActive(false);

        if (gameEnded)
        {
            for (int i = 0; i < cardButtons.Length; i++)
            {
                cardButtons[i].interactable = false;
                otherButtons[i].interactable = false;
            }
            rootButtons[state.thisPlayer].interactable = false;
            endButtons[state.thisPlayer].interactable = false;
            playButtons[state.thisPlayer].gameObject.SetActive(false);
            discardButtons[state.thisPlayer].gameObject.SetActive(false);
            cancelButtons[state.thisPlayer].gameObject.SetActive(false);
            compostButtons[state.thisPlayer].gameObject.SetActive(false);
            selectedImages[state.thisPlayer].gameObject.SetActive(false);
            selectedImages[state.otherPlayer].gameObject.SetActive(false);
            return;
        }

        List<int> hand = state.players[state.thisPlayer].hand;
        List<int> otherHand = state.players[state.otherPlayer].hand;
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (i < hand.Count)
            {
                cardButtons[i].gameObject.SetActive(true);
                cardButtons[i].interactable = true;
                cardButtons[i].image.sprite = State.collection.sprites[hand[i]];
                int defaultI = 2 * i;
                int rotI = hand.Count - 1;
                float angle = (defaultI - rotI) * Mathf.PI / 36;
                cardButtons[i].transform.rotation = new Quaternion(0, 0, Mathf.Sin(angle / 2), Mathf.Cos(angle / 2));
            }
            else
            {
                cardButtons[i].gameObject.SetActive(false);
            }
            if (i < otherHand.Count)
            {
                otherButtons[i].gameObject.SetActive(true);
                otherButtons[i].interactable = false;
            }
            else
            {
                otherButtons[i].gameObject.SetActive(false);
            }
        }

        if (state.discardPhase)
        {
            phaseDisp.text = "Too many cards. Discard a card";
            rootButtons[state.thisPlayer].interactable = false;
            endButtons[state.thisPlayer].interactable = false;
            playButtons[state.thisPlayer].gameObject.SetActive(false);
            if (state.card != null)
            {
                discardButtons[state.thisPlayer].gameObject.SetActive(true);
                cancelButtons[state.thisPlayer].gameObject.SetActive(true);
            }
            else
            {
                discardButtons[state.thisPlayer].gameObject.SetActive(false);
                cancelButtons[state.thisPlayer].gameObject.SetActive(false);
            }
            compostButtons[state.thisPlayer].gameObject.SetActive(false);
        }
        else if (state.tilePhase)
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
            if (state.cardIndex == 0)
            {
                cancelButtons[state.thisPlayer].gameObject.SetActive(true);
            }
            else
            {
                cancelButtons[state.thisPlayer].gameObject.SetActive(false);
            }
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
                bool moveExists = false;
                for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
                {
                    if (state.card.Validation(state, i))
                    {
                        moveExists = true;
                        break;
                    }
                }
                playButtons[state.thisPlayer].interactable = state.players[state.thisPlayer].water >= state.card.GetCost(state) && moveExists;
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
            selectedImages[state.thisPlayer].sprite = State.collection.sprites[hoveredIndexes[state.thisPlayer]];
            selectedImages[state.thisPlayer].gameObject.SetActive(true);
        }
        else if (state.card != null)
        {
            selectedImages[state.thisPlayer].sprite = State.collection.sprites[state.cardIndex];
            selectedImages[state.thisPlayer].gameObject.SetActive(true);
        }
        else
        {
            selectedImages[state.thisPlayer].gameObject.SetActive(false);
        }

        if (hoveredIndexes[state.otherPlayer] != -1)
        {
            selectedImages[state.otherPlayer].sprite = State.collection.sprites[hoveredIndexes[state.otherPlayer]];
            selectedImages[state.otherPlayer].gameObject.SetActive(true);
        }
        else
        {
            selectedImages[state.otherPlayer].gameObject.SetActive(false);
        }
    }

    public void SetCard(int i)
    {
        Player player = state.players[state.thisPlayer];
        if (player.ai)
        {
            return;
        }
        if (i >= 0)
        {
            state.SetCard(player.hand[i]);
        }
        else
        {
            state.SetCard(0);
        }
    }

    public void PlayCard()
    {
        if (!state.players[state.thisPlayer].ai)
        {
            state.PlayCard();
        }
    }

    public void DiscardCard()
    {
        if (!state.players[state.thisPlayer].ai)
        {
            state.DiscardCard();
        }
    }

    public void CancelCard()
    {
        if (!state.players[state.thisPlayer].ai)
        {
            state.CancelCard();
        }
    }

    public void CompostCard()
    {
        if (!state.players[state.thisPlayer].ai)
        {
            state.CompostCard();
        }
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

    public void PlayButtonHover(bool activate)
    {
        if (activate && state.card != null && !playButtons[state.thisPlayer].interactable)
        {
            if (state.players[state.thisPlayer].water < state.card.GetCost(state))
            {
                disabledMessages[state.thisPlayer].text = "You don't have enough water to play this card.";
            }
            else
            {
                disabledMessages[state.thisPlayer].text = state.card.GetDisabledMessage();
            }
            disabledMsgBackgrounds[state.thisPlayer].SetActive(true);
        }
        else
        {
            disabledMsgBackgrounds[state.thisPlayer].SetActive(false);
        }
    }

    private void UpdateHighlights()
    {
        for (int i = 0; i < state.boardHeight * state.boardWidth; i++)
        {
            int[] coords = state.IndexToCoord(i);
            int x = coords[0];
            int y = coords[1];

            highlightMap.SetTile(new Vector3Int(x, y), null);
            if (!state.discardPhase && state.card != null)
            {
                if (state.card.GetNumActions(state) > 0)
                {
                    if (state.card.Validation(state, i))
                    {
                        highlightMap.SetTile(new Vector3Int(x, y), highlightTile);
                    }
                }
                else
                {
                    if (state.card.OverrideHighlight(state, i))
                    {
                        highlightMap.SetTile(new Vector3Int(x, y), highlightTile);
                    }
                }
            }
        }
    }

    private void EndGame()
    {
        gameEnded = true;
        phaseDispBG.SetActive(false);
        int p1Points = state.players[0].points;
        int p2Points = state.players[1].points;

        int winner = -1;
        if (p1Points > p2Points)
        {
            winner = 0;
        }
        else if (p1Points < p2Points)
        {
            winner = 1;
        }

        tornadoAnim.enabled = true;
        if (winner == -1)
        {
            winnerDisp.text = "It's a Draw.";
            defeatAnims[0].enabled = true;
            defeatAnims[1].enabled = true;
        }
        else
        {
            winnerDisp.text = $"Player {winner + 1} wins!";
            defeatAnims[1 - winner].enabled = true;
        }
        StartCoroutine(EndGameHelper());
    }

    IEnumerator EndGameHelper()
    {
        yield return new WaitForSeconds(5);
        winnerDisp.enabled = true;
    }
}
