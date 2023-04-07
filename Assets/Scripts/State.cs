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
    public Card card = null;
    public int cardIndex = -1;
    public int numActions = 0;
    public Player[] players;
    public static Tilemap outlineMap;
    public static Tilemap waterMap;
    public static Tilemap otherMap;
    public static TileBase rootTile;
    public static TileBase deadRootTile;
    public static TileBase woodShieldTile;
    public static TileBase metalShieldTile;
    public static TileBase thornTile;
    public static TileBase strongFireTile;
    public static TileBase weakFireTile;

    public State(TileBase rootTile, TileBase deadRootTile, TileBase woodShieldTile, TileBase metalShieldTile, TileBase thornTile, TileBase strongFireTile, TileBase weakFireTile, Plant[] plants)
    {
        absolute = true;
        State.rootTile = rootTile;
        State.deadRootTile = deadRootTile;
        State.woodShieldTile = woodShieldTile;
        State.metalShieldTile = metalShieldTile;
        State.thornTile = thornTile;
        State.strongFireTile = strongFireTile;
        State.weakFireTile = weakFireTile;
        Tilemap roots1Map = GameObject.FindGameObjectWithTag("Roots1").GetComponent<Tilemap>();
        Tilemap roots2Map = GameObject.FindGameObjectWithTag("Roots2").GetComponent<Tilemap>();
        players = new Player[2] {
            new Player('1', ',', '[', '!', 'i', 'I', '{', 'T', 'B', 'S', roots1Map, plants[0]),
            new Player('2', '.', ']', '@', 'z', 'Z', '}', 't', 'b', 's', roots2Map, plants[1]),
        };
        outlineMap = GameObject.FindGameObjectWithTag("Outline").GetComponent<Tilemap>();
        waterMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
        Tilemap rockMap = GameObject.FindGameObjectWithTag("Rocks").GetComponent<Tilemap>();
        otherMap = GameObject.FindGameObjectWithTag("Other").GetComponent<Tilemap>();

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
        for (int i = 0; i < boardHeight * boardWidth; i++)
        {
            if (waterTiles[i] != null)
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
        card = parent.card;
        cardIndex = parent.cardIndex;
        numActions = parent.numActions;
        players = new Player[2] { new Player(parent.players[0]), new Player(parent.players[1]) };
        board = new char[parent.board.Length];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = parent.board[i];
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

    public bool CheckRock(int index, Player player)
    {
        int count = 0;
        int[] coords = IndexToCoord(index);
        for (int x = coords[0] - 1; x <= coords[0] + 1; x++)
        {
            for (int y = coords[1] - 1; y <= coords[1] + 1; y++)
            {
                char coordState = GetCoordState(x, y);
                if (coordState == player.root || coordState == player.fortifiedRoot || coordState == player.invincibleRoot || coordState == player.baseRoot)
                {
                    count++;
                }
            }
        }
        return count == 8;
    }

    // Check if any of the neighboring tiles for (x,y) is a specific element
    public bool HasNeighbor(int x, int y, char[] neighbors)
    {
        return CountNeighbors(x, y, neighbors) != 0;
    }

    // Checks if path along an element exisits from (x,y) to a goal
    public bool AStar(int x, int y, char[] path, char goal)
    {
        Queue<int> queue = new Queue<int>();
        ArrayList visited = new ArrayList();
        int i = CoordToIndex(x, y);
        if (i == -1)
        {
            return false;
        }
        if (board[i] == goal)
        {
            return true;
        }
        queue.Enqueue(i);
        while (queue.Count != 0)
        {
            i = queue.Dequeue();
            visited.Add(i);
            int[] coords = IndexToCoord(i);
            x = coords[0];
            y = coords[1];
            int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
            for (int j = 0; j < 4; j++)
            {
                x = dirs[j, 0];
                y = dirs[j, 1];
                if (GetCoordState(x, y) == goal)
                {
                    return true;
                }
                int index = CoordToIndex(x, y);
                if (index != -1 && Array.IndexOf(path, board[index]) != -1 && !visited.Contains(index))
                {
                    queue.Enqueue(index);
                }
            }
        }
        return false;
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
}
