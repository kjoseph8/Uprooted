using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class State
{
    public bool absolute;
    public char[] board;
    public int boardHeight;
    public int boardWidth;
    public int thisPlayer = 1;
    public int otherPlayer = 0;
    public int turn = 0;
    public int maxTurns = 20;
    public bool discardPhase = false;
    public bool tilePhase = false;
    public Card card = null;
    public int cardIndex = -1;
    public int tileIndex = -1;
    public int numActions = 0;
    public Player[] players;
    public List<int> oasisIndexes;
    public int loveCardPartnerIndex = -1;
    public List<int> validAIMoves;
    public static CardCollection collection;
    public static Tilemap outlineMap;
    public static Tilemap waterMap;
    public static Tilemap rockMap;
    public static Tilemap otherMap;
    public static Tilemap oasisMap;
    public static Tilemap lavaMap;
    public static TileBase rootTile;
    public static TileBase deadRootTile;
    public static TileBase rockTile;
    public static TileBase seedTile;
    public static TileBase woodShieldTile;
    public static TileBase metalShieldTile;
    public static TileBase thornTile;
    public static TileBase strongFireTile;
    public static TileBase weakFireTile;

    public State(CardCollection collection, TileBase rootTile, TileBase deadRootTile, TileBase rockTile, TileBase seedTile, TileBase woodShieldTile, TileBase metalShieldTile, TileBase thornTile, TileBase strongFireTile, TileBase weakFireTile)
    {
        absolute = true;
        State.collection = collection;
        State.rootTile = rootTile;
        State.deadRootTile = deadRootTile;
        State.rockTile = rockTile;
        State.seedTile = seedTile;
        State.woodShieldTile = woodShieldTile;
        State.metalShieldTile = metalShieldTile;
        State.thornTile = thornTile;
        State.strongFireTile = strongFireTile;
        State.weakFireTile = weakFireTile;
        Tilemap roots1Map = GameObject.FindGameObjectWithTag("Roots1").GetComponent<Tilemap>();
        Tilemap roots2Map = GameObject.FindGameObjectWithTag("Roots2").GetComponent<Tilemap>();
        GameObject menuManagerObj = GameObject.FindGameObjectWithTag("MenuManager");
        bool[] ai = new bool[] { false, false };
        if (menuManagerObj != null)
        {
            ai = menuManagerObj.GetComponent<MenuManager>().ai;
        }
        players = new Player[2] {
            new Player(ai[0], "rose", '1', ',', '[', '!', 'i', 'I', '{', 'T', 'B', 'S', roots1Map),
            new Player(ai[1], "rose", '2', '.', ']', '@', 'z', 'Z', '}', 't', 'b', 's', roots2Map),
        };
        oasisIndexes = new List<int>();
        validAIMoves = new List<int>();
        outlineMap = GameObject.FindGameObjectWithTag("Outline").GetComponent<Tilemap>();
        waterMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
        rockMap = GameObject.FindGameObjectWithTag("Rocks").GetComponent<Tilemap>();
        otherMap = GameObject.FindGameObjectWithTag("Other").GetComponent<Tilemap>();
        Tilemap planetMap = GameObject.FindGameObjectWithTag("Planet").GetComponent<Tilemap>();
        GameObject oasis = GameObject.FindGameObjectWithTag("Oasis");
        oasisMap = null;
        if (oasis != null)
        {
            oasisMap = oasis.GetComponent<Tilemap>();
        }
        GameObject lava = GameObject.FindGameObjectWithTag("Lava");
        lavaMap = null;
        if (lava != null)
        {
            lavaMap = lava.GetComponent<Tilemap>();
        }

        BoundsInt bounds = outlineMap.cellBounds;
        boardHeight = bounds.size.y;
        boardWidth = bounds.size.x;
        board = new char[boardHeight * boardWidth];

        TileBase[] outlineTiles = outlineMap.GetTilesBlock(bounds);
        TileBase[] waterTiles = waterMap.GetTilesBlock(bounds);
        TileBase[] rockTiles = rockMap.GetTilesBlock(bounds);
        TileBase[] roots1Tiles = roots1Map.GetTilesBlock(bounds);
        TileBase[] roots2Tiles = roots2Map.GetTilesBlock(bounds);
        TileBase[] otherTiles = otherMap.GetTilesBlock(bounds);
        TileBase[] planetTiles = planetMap.GetTilesBlock(bounds);
        TileBase[] oasisTiles = null;
        if (oasisMap != null)
        {
            oasisTiles = oasisMap.GetTilesBlock(bounds);
        }
        TileBase[] lavaTiles = null;
        if (lavaMap != null)
        {
            lavaTiles = lavaMap.GetTilesBlock(bounds);
        }
        for (int i = 0; i < boardHeight * boardWidth; i++)
        {
            if (oasisTiles != null && oasisTiles[i] != null && planetTiles[i] == null)
            {
                oasisIndexes.Add(i);
            }
            if (lavaTiles != null && lavaTiles[i] != null)
            {
                board[i] = 'L';
            }
            else if (waterTiles[i] != null)
            {
                board[i] = 'W';
            }
            else if (rockTiles[i] != null)
            {
                board[i] = 'R';
            }
            else if (roots1Tiles[i] != null)
            {
                if (otherTiles[i] != null)
                {
                    board[i] = '!';
                }
                else
                {
                    board[i] = '1';
                }
            }
            else if (roots2Tiles[i] != null)
            {
                if (otherTiles[i] != null)
                {
                    board[i] = '@';
                }
                else
                {
                    board[i] = '2';
                }
            }
            else if (outlineTiles[i] != null)
            {
                board[i] = '-';
            }
            else
            {
                board[i] = '0';
            }
        }
    }

    public State(State parent)
    {
        absolute = false;
        boardHeight = parent.boardHeight;
        boardWidth = parent.boardWidth;
        thisPlayer = parent.thisPlayer;
        otherPlayer = parent.otherPlayer;
        turn = parent.turn;
        maxTurns = parent.maxTurns;
        discardPhase = parent.discardPhase;
        tilePhase = parent.tilePhase;
        tileIndex = parent.tileIndex;
        card = parent.card;
        cardIndex = parent.cardIndex;
        numActions = parent.numActions;
        loveCardPartnerIndex = parent.loveCardPartnerIndex;
        players = new Player[2] { new Player(parent.players[0]), new Player(parent.players[1]) };
        validAIMoves = new List<int>();
        board = new char[parent.board.Length];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = parent.board[i];
        }
        oasisIndexes = new List<int>();
        foreach (int i in parent.oasisIndexes)
        {
            oasisIndexes.Add(i);
        }
    }

    // For Debug Purposes: Get a String Representation of Game Board
    public string StateToString()
    {
        string output = "";
        for (int y = boardHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                output += board[CoordToIndex(x, y)];
            }
            output += '\n';
        }
        return output;
    }

    // Get Mouse Position as Index of board array
    public int[] MouseToCoord()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cellPosition = outlineMap.WorldToCell(mousePosition);
        int[] coord = { cellPosition.x, cellPosition.y };
        return coord;
    }

    public Vector3 CoordToWorld(int x, int y)
    {
        Vector3 worldPosition = outlineMap.CellToWorld(new Vector3Int(x, y, 0));
        worldPosition.x += 0.5f;
        worldPosition.y += 0.5f;
        return worldPosition;
    }

    // 2D to 1D index
    public int CoordToIndex(int x, int y)
    {
        if (x < 0 || x > boardWidth - 1 || y < 0 || y > boardHeight - 1)
        {
            return -1;
        }
        return x + y * boardWidth;
    }

    public int[] IndexToCoord(int i)
    {
        int[] coord = new int[2];
        coord[0] = i % boardWidth;
        coord[1] = i / boardWidth;
        return coord;
    }

    // Get state at specific coordinate
    public char GetCoordState(int x, int y)
    {
        int i = CoordToIndex(x, y);
        if (i == -1)
        {
            return '0';
        }
        return board[i];
    }

    // Get state of element at mouse position
    public char GetMouseState()
    {
        int[] coord = MouseToCoord();
        return GetCoordState(coord[0], coord[1]);
    }

    // Count number of neighboring tiles for (x,y) that is a specific element
    public int CountNeighbors(int x, int y, char[] neighbors)
    {
        int count = 0;
        if (Array.IndexOf(neighbors, GetCoordState(x - 1, y)) != -1)
        {
            count++;
        }
        if (Array.IndexOf(neighbors, GetCoordState(x + 1, y)) != -1)
        {
            count++;
        }
        if (Array.IndexOf(neighbors, GetCoordState(x, y - 1)) != -1)
        {
            count++;
        }
        if (Array.IndexOf(neighbors, GetCoordState(x, y + 1)) != -1)
        {
            count++;
        }
        return count;
    }

    public int CountAllNeighbors(int index, char[] neighbors)
    {
        int count = 0;
        int[] coords = IndexToCoord(index);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                if (Array.IndexOf(neighbors, GetCoordState(coords[0] + x, coords[1] + y)) != -1)
                {
                    count++;
                }
            }
        }
        return count;
    }

    // Check if any of the neighboring tiles for (x,y) is a specific element
    public bool HasNeighbor(int x, int y, char[] neighbors)
    {
        return CountNeighbors(x, y, neighbors) != 0;
    }

    public bool HasAnyNeighbor(int index, char[] neighbors)
    {
        return CountAllNeighbors(index, neighbors) != 0;
    }

    // BFS algorithm from points in start along path to goal
    public int BFS(List<int> start, char[] path, char[] goal, string output = "dist", int timeout = -1)
    {
        Queue<int> queue = new Queue<int>();
        Queue<int> dists = new Queue<int>();
        List<int> visited = new List<int>();
        int dist = 0;
        Player player = players[thisPlayer];
        foreach (int i in start)
        {
            if (i != -1)
            {
                queue.Enqueue(i);
                int[] coords = IndexToCoord(i);
                if (output == "rootAI" && CountNeighbors(coords[0], coords[1], new char[] { player.deadRoot, player.deadFortifiedRoot, player.deadInvincibleRoot }) == 0)
                {
                    dists.Enqueue(dist + 1);
                }
                else
                {
                    dists.Enqueue(dist);
                }
                visited.Add(i);
            }
        }
        int count = 0;
        if (output == "rootAI")
        {
            validAIMoves.Clear();
        }
        while (queue.Count != 0)
        {
            int i = queue.Dequeue();
            dist = dists.Dequeue();
            if (timeout != -1 && dist > timeout)
            {
                if (output == "count")
                {
                    return count;
                }
                else if (output == "rootAI")
                {
                    return validAIMoves.Count;
                }
                else
                {
                    return -1;
                }
            }
            if (output == "oasis")
            {
                if (oasisIndexes.Contains(i))
                {
                    return dist;
                }
            }
            else if (output == "rootAI")
            {
                if (collection.cards[0].Validation(this, i))
                {
                    timeout = dist;
                    validAIMoves.Add(i);
                }
            }
            else if (Array.IndexOf(goal, board[i]) != -1)
            {
                if (output == "count")
                {
                    count++;
                }
                else
                {
                    return dist;
                }
            }
            int[] coords = IndexToCoord(i);
            int x = coords[0];
            int y = coords[1];
            int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
            for (int j = 0; j < 4; j++)
            {
                x = dirs[j, 0];
                y = dirs[j, 1];
                int index = CoordToIndex(x, y);
                if (index != -1 && (Array.IndexOf(path, board[index]) != -1 || Array.IndexOf(goal, board[index]) != -1) && !visited.Contains(index))
                {
                    queue.Enqueue(index);
                    dists.Enqueue(dist + 1);
                    visited.Add(index);
                }
            }
        }
        if (output == "count")
        {
            return count;
        }
        else
        {
            return -1;
        }
    }

    public void KillRoots(int x, int y, Player player)
    {
        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = CoordToIndex(dirX, dirY);
            if (dirI != -1)
            {
                if (board[dirI] == player.root)
                {
                    board[dirI] = player.deadRoot;
                    if (absolute)
                    {
                        player.rootMap.SetTile(new Vector3Int(dirX, dirY), State.deadRootTile);
                    }
                    KillRoots(dirX, dirY, player);
                }
                else if (board[dirI] == player.fortifiedRoot)
                {
                    board[dirI] = player.deadFortifiedRoot;
                    if (absolute)
                    {
                        player.rootMap.SetTile(new Vector3Int(dirX, dirY), State.deadRootTile);
                    }
                    KillRoots(dirX, dirY, player);
                }
                else if (board[dirI] == player.invincibleRoot)
                {
                    board[dirI] = player.deadInvincibleRoot;
                    if (absolute)
                    {
                        player.rootMap.SetTile(new Vector3Int(dirX, dirY), State.deadRootTile);
                    }
                    KillRoots(dirX, dirY, player);
                }
            }
        }
    }

    public void ResurrectRoots(int x, int y, Player player)
    {
        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = CoordToIndex(dirX, dirY);
            if (dirI != -1)
            {
                if (board[dirI] == player.deadRoot)
                {
                    board[dirI] = player.root;
                    if (absolute)
                    {
                        player.rootMap.SetTile(new Vector3Int(dirX, dirY), State.rootTile);
                    }
                    ResurrectRoots(dirX, dirY, player);
                }
                else if (board[dirI] == player.deadFortifiedRoot)
                {
                    board[dirI] = player.fortifiedRoot;
                    if (absolute)
                    {
                        player.rootMap.SetTile(new Vector3Int(dirX, dirY), State.rootTile);
                    }
                    ResurrectRoots(dirX, dirY, player);
                }
                else if (board[dirI] == player.deadInvincibleRoot)
                {
                    board[dirI] = player.invincibleRoot;
                    if (absolute)
                    {
                        player.rootMap.SetTile(new Vector3Int(dirX, dirY), State.rootTile);
                    }
                    ResurrectRoots(dirX, dirY, player);
                }
            }
        }
    }

    public void UpdatePoints()
    {
        for (int playerIndex = 0; playerIndex < 2; playerIndex++)
        {
            Player player = players[playerIndex];
            player.rootCount = 0;
            player.rockCount = 0;
            player.completeRockCount = 0;
            for (int i = 0; i < boardHeight * boardWidth; i++)
            {
                if (board[i] == player.root || board[i] == player.fortifiedRoot || board[i] == player.invincibleRoot || board[i] == player.baseRoot)
                {
                    player.rootCount++;
                }
                else if (board[i] == 'R')
                {
                    int rockCount = CountAllNeighbors(i, new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot });
                    player.rockCount += rockCount;
                    if (rockCount == 8)
                    {
                        player.completeRockCount++;
                    }
                }
            }
            player.points = player.rootCount + player.rockCount + 5 * player.completeRockCount + player.water;
        }
    }

    public void SetCard(int i)
    {
        card = collection.cards[i];
        cardIndex = i;
        numActions = card.GetNumActions(this);
        if (i == 0)
        {
            tilePhase = true;
        }
        else
        {
            tilePhase = false;
        }
        UpdatePoints();
    }

    public void PlayCard()
    {
        Player player = players[thisPlayer];
        player.water -= card.GetCost(this);
        player.hand.Remove(cardIndex);
        player.discard.Add(cardIndex);
        if (card.GetNumActions(this) == 0)
        {
            numActions = 0;
            card.Action(this, 0);
            card = null;
            cardIndex = -1;
            tilePhase = false;
        }
        else
        {
            tilePhase = true;
        }

        if (players[thisPlayer].hand.Count > 5)
        {
            discardPhase = true;
        }
        UpdatePoints();
    }

    public void PlayTile(int index)
    {
        card.Action(this, index);
        numActions--;
        if (numActions == 0)
        {
            tilePhase = false;
            card = null;
            cardIndex = -1;
        }
        UpdatePoints();
    }

    public void DiscardCard()
    {
        Player player = players[thisPlayer];
        player.hand.Remove(cardIndex);
        player.discard.Add(cardIndex);
        card = null;
        cardIndex = -1;
        discardPhase = false;
        UpdatePoints();
    }

    public void CancelCard()
    {
        card = null;
        cardIndex = -1;
        numActions = 0;
        tilePhase = false;
        UpdatePoints();
    }

    public void CompostCard()
    {
        Player player = players[thisPlayer];
        player.water--;
        player.hand.Remove(cardIndex);
        player.discard.Add(cardIndex);
        card = null;
        cardIndex = -1;
        player.DrawCard();
        UpdatePoints();
    }

    public void StartTurn()
    {
        if (turn == maxTurns && thisPlayer == 0)
        {
            for (int i = 0; i < boardHeight * boardWidth; i++)
            {
                int[] coords = IndexToCoord(i);
                int x = coords[0];
                int y = coords[1];

                if (board[i] == players[0].root || board[i] == players[0].fortifiedRoot)
                {
                    board[i] = players[0].invincibleRoot;
                    if (absolute)
                    {
                        otherMap.SetTile(new Vector3Int(x, y), metalShieldTile);
                    }
                }
                else if (board[i] == players[1].root || board[i] == players[1].fortifiedRoot)
                {
                    board[i] = players[1].invincibleRoot;
                    if (absolute)
                    {
                        otherMap.SetTile(new Vector3Int(x, y), metalShieldTile);
                    }
                }
                else if (board[i] == players[0].deadRoot || board[i] == players[0].deadFortifiedRoot)
                {
                    board[i] = players[0].deadInvincibleRoot;
                    if (absolute)
                    {
                        otherMap.SetTile(new Vector3Int(x, y), metalShieldTile);
                    }
                }
                else if (board[i] == players[1].deadRoot || board[i] == players[1].deadFortifiedRoot)
                {
                    board[i] = players[1].deadInvincibleRoot;
                    if (absolute)
                    {
                        otherMap.SetTile(new Vector3Int(x, y), metalShieldTile);
                    }
                }
            }
        }

        Player player = players[thisPlayer];
        player.water += (turn - 1) / 10 + 2;
        player.rootMoves = 2;

        if (player.scentTurns > 0)
        {
            player.water++;
            player.scentTurns--;
        }

        List<int> lava = new List<int>();
        for (int i = 0; i < boardHeight * boardWidth; i++)
        {
            if (board[i] == player.baseRoot)
            {
                List<int> start = new List<int>();
                start.Add(i);
                if (BFS(start, new char[] { player.root, player.fortifiedRoot, player.invincibleRoot, player.baseRoot }, new char[0], "oasis") != -1)
                {
                    player.water++;
                }
            }
        }

        for (int i = player.hand.Count; i < 5; i++)
        {
            player.DrawCard();
        }
    }

    public void ChangeTurn()
    {
        EndTurn();

        thisPlayer = otherPlayer;
        otherPlayer = 1 - thisPlayer;

        if (turn <= maxTurns)
        {
            StartTurn();
        }
        UpdatePoints();
    }

    public void EndTurn()
    {
        if (thisPlayer == 1)
        {
            turn++;
        }

        ForestFireCard.UpdateFire(this);

        if (players[thisPlayer].wormTurns > 0)
        {
            MrWormCard.GrowRandomRoot(this);
            players[thisPlayer].wormTurns--;
        }

        card = null;
        cardIndex = -1;
        tilePhase = false;
    }
}
