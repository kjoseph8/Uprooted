using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class State
{
    public char[] board;
    public int boardHeight;
    public int boardWidth;
    public int thisPlayer = 0;
    public int otherPlayer = 1;
    public int turn = 1;
    public int maxTurns = 50;
    public Card card = null;
    public Player[] players;
    public static Tilemap outlineMap;
    public static Tilemap waterMap;
    public static Tilemap pointsMap;
    public static Tilemap otherMap;
    public static TileBase rootTile;
    public static TileBase deadRootTile;
    public static TileBase thornTile;
    public static TileBase strongFireTile;
    public static TileBase weakFireTile;

    public void InitState(TileBase rt, TileBase drt, TileBase tt, TileBase sft, TileBase wft, TextMeshProUGUI[] waterDisps, TextMeshProUGUI[] pointDisps)
    {
        rootTile = rt;
        deadRootTile = drt;
        thornTile = tt;
        strongFireTile = sft;
        weakFireTile = wft;
        players = new Player[2] {
            new Player('1', '!', 'I', 'M', 'm', "Roots1", waterDisps[0], pointDisps[0]),
            new Player('2', '@', 'Z', 'F', 'f', "Roots2", waterDisps[1], pointDisps[1]),
        };
        outlineMap = GameObject.FindGameObjectWithTag("Outline").GetComponent<Tilemap>();
        waterMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
        pointsMap = GameObject.FindGameObjectWithTag("Points").GetComponent<Tilemap>();
        otherMap = GameObject.FindGameObjectWithTag("Other").GetComponent<Tilemap>();
        Tilemap roots1Map = GameObject.FindGameObjectWithTag("Roots1").GetComponent<Tilemap>();
        Tilemap roots2Map = GameObject.FindGameObjectWithTag("Roots2").GetComponent<Tilemap>();

        BoundsInt bounds = outlineMap.cellBounds;
        boardHeight = bounds.size.y;
        boardWidth = bounds.size.x;
        board = new char[boardHeight * boardWidth];

        TileBase[] outlineTiles = outlineMap.GetTilesBlock(bounds);
        TileBase[] waterTiles = waterMap.GetTilesBlock(bounds);
        TileBase[] pointsTiles = pointsMap.GetTilesBlock(bounds);
        TileBase[] roots1Tiles = roots1Map.GetTilesBlock(bounds);
        TileBase[] roots2Tiles = roots2Map.GetTilesBlock(bounds);
        for (int i = 0; i < boardHeight * boardWidth; i++)
        {
            if (waterTiles[i] != null)
            {
                board[i] = 'W';
            }
            else if (pointsTiles[i] != null)
            {
                board[i] = 'P';
            }
            else if (roots1Tiles[i] != null)
            {
                board[i] = '!';
            }
            else if (roots2Tiles[i] != null)
            {
                board[i] = '@';
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
    public int CountNeighbors(int x, int y, char neighbor)
    {
        int count = 0;
        if (GetCoordState(x - 1, y) == neighbor)
        {
            count++;
        }
        if (GetCoordState(x + 1, y) == neighbor)
        {
            count++;
        }
        if (GetCoordState(x, y - 1) == neighbor)
        {
            count++;
        }
        if (GetCoordState(x, y + 1) == neighbor)
        {
            count++;
        }
        return count;
    }

    // Check if any of the neighboring tiles for (x,y) is a specific element
    public bool HasNeighbor(int x, int y, char neighbor)
    {
        return CountNeighbors(x, y, neighbor) != 0;
    }

    // Checks if path along an element exisits from (x,y) to a goal
    public bool AStar(int x, int y, char path, char goal)
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
                if (index != -1 && board[index] == path && !visited.Contains(index))
                {
                    queue.Enqueue(index);
                }
            }
        }
        return false;
    }

    public void Spread(int x, int y, char oldState, char newState, TileBase tile, Tilemap rootMap)
    {
        int index = CoordToIndex(x, y);
        board[index] = newState;
        rootMap.SetTile(new Vector3Int(x, y), tile);
        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = CoordToIndex(dirX, dirY);
            if (dirI != -1 && board[dirI] == oldState)
            {
                Spread(dirX, dirY, oldState, newState, tile, rootMap);
            }
        }
    }

    public void SpreadFire(int index, char fire, TileBase tile)
    {
        int[] coord = IndexToCoord(index);
        int x = coord[0];
        int y = coord[1];
        board[index] = '-';
        otherMap.SetTile(new Vector3Int(x, y), null);
        int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        for (int i = 0; i < 4; i++)
        {
            int dirX = dirs[i, 0];
            int dirY = dirs[i, 1];
            int dirI = CoordToIndex(dirX, dirY);
            if (dirI != -1 && (board[dirI] == players[0].root || board[dirI] == players[0].deadRoot || board[dirI] == players[1].root || board[dirI] == players[1].deadRoot || board[dirI] == 'T'))
            {
                SpreadFireHelper(dirI, fire, tile);
            }
        }
    }

    private void SpreadFireHelper(int index, char fire, TileBase tile)
    {
        int target = -1;
        if (board[index] == players[0].root)
        {
            target = 0;
        }
        else if (board[index] == players[1].root)
        {
            target = 1;
        }
        int[] coord = IndexToCoord(index);
        int x = coord[0];
        int y = coord[1];
        board[index] = fire;
        players[0].rootMap.SetTile(new Vector3Int(x, y), null);
        players[1].rootMap.SetTile(new Vector3Int(x, y), null);
        otherMap.SetTile(new Vector3Int(x, y), tile);
        if (tile != null)
        {
            players[thisPlayer].weakFireIndex.Add(index);
        }
        if (target != -1)
        {
            int[,] dirs = { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
            for (int i = 0; i < 4; i++)
            {
                int dirX = dirs[i, 0];
                int dirY = dirs[i, 1];
                int dirI = CoordToIndex(dirX, dirY);
                if (dirI != -1 && target == 0 && board[dirI] == players[0].root &&
                !AStar(dirX, dirY, players[0].root, players[0].baseRoot))
                {
                    Spread(dirX, dirY, players[0].root, players[0].deadRoot, State.deadRootTile, players[0].rootMap);
                }
                else if (dirI != -1 && target == 1 && board[dirI] == players[1].root &&
                !AStar(dirX, dirY, players[1].root, players[1].baseRoot))
                {
                    Spread(dirX, dirY, players[1].root, players[1].deadRoot, State.deadRootTile, players[1].rootMap);
                }
            }
        }
    }
}
