using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class State
{
    public static char[] state = new char[31 * 31];
    public static int player = 0;
    public static int turn = 1;
    public static Card card = null;
    public static Player[] players;
    public static Tilemap waterMap;
    public static Tilemap pointsMap;
    public static RuleTile rootTile;
    public static RuleTile deadRootTile;
    private static Plane _tilePlane;

    public static void InitState(RuleTile rt, RuleTile drt, TextMeshProUGUI[] waterDisps, TextMeshProUGUI[] pointDisps)
    {
        _tilePlane = new Plane(Vector3.back, Vector3.zero);
        rootTile = rt;
        deadRootTile = drt;
        players = new Player[2] {
            new Player('1', '!', 'I', "Roots1", waterDisps[0], pointDisps[0]),
            new Player('2', '@', 'Z', "Roots2", waterDisps[1], pointDisps[1]),
        };
        waterMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
        pointsMap = GameObject.FindGameObjectWithTag("Points").GetComponent<Tilemap>();
        Tilemap roots1Map = GameObject.FindGameObjectWithTag("Roots1").GetComponent<Tilemap>();
        Tilemap roots2Map = GameObject.FindGameObjectWithTag("Roots2").GetComponent<Tilemap>();
        Tilemap outlineMap = GameObject.FindGameObjectWithTag("Outline").GetComponent<Tilemap>();

        BoundsInt bounds = outlineMap.cellBounds;
        TileBase[] outlineTiles = outlineMap.GetTilesBlock(bounds);
        TileBase[] waterTiles = waterMap.GetTilesBlock(bounds);
        TileBase[] pointsTiles = pointsMap.GetTilesBlock(bounds);
        TileBase[] roots1Tiles = roots1Map.GetTilesBlock(bounds);
        TileBase[] roots2Tiles = roots2Map.GetTilesBlock(bounds);
        for (int i = 0; i < 31 * 31; i++)
        {
            if (waterTiles[i] != null)
            {
                state[i] = 'W';
            }
            else if (pointsTiles[i] != null)
            {
                state[i] = 'P';
            }
            else if (roots1Tiles[i] != null)
            {
                state[i] = '!';
            }
            else if (roots2Tiles[i] != null)
            {
                state[i] = '@';
            }
            else if (outlineTiles[i] != null)
            {
                state[i] = '-';
            }
            else
            {
                state[i] = '0';
            }
        }
    }

    // For Debug Purposes: Get a String Representation of Game State
    public static string StateToString()
    {
        string output = "";
        for (int y = 30; y >= 0; y--)
        {
            for (int x = 0; x < 31; x++)
            {
                output += state[CoordToIndex(x, y)];
            }
            output += '\n';
        }
        return output;
    }

    // Get Mouse Position as Index of state array
    public static int[] MouseToCoord()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        _tilePlane.Raycast(ray, out distance);
        Vector3 worldPosition = ray.GetPoint(distance);
        int x = Mathf.FloorToInt(worldPosition.x + 15.5f);
        int y = Mathf.FloorToInt(worldPosition.y + 15.5f);
        int[] coord = { x, y };
        return coord;
    }

    // 2D to 1D index
    public static int CoordToIndex(int x, int y)
    {
        if (x < 0 || x > 30 || y < 0 || y > 30)
        {
            return -1;
        }
        return x + y * 31;
    }

    public static int[] IndexToCoord(int i)
    {
        int[] coord = new int[2];
        coord[0] = i % 31;
        coord[1] = i / 31;
        return coord;
    }

    // Get state at specific coordinate
    public static char GetCoordState(int x, int y)
    {
        int i = CoordToIndex(x, y);
        if (i == -1)
        {
            return '0';
        }
        return state[i];
    }

    // Get state of element at mouse position
    public static char GetMouseState()
    {
        int[] coord = MouseToCoord();
        return GetCoordState(coord[0], coord[1]);
    }

    // Check if any of the neighboring tiles for (x,y) is a specific element
    public static bool HasNeighbor(int x, int y, char neighbor)
    {
        return GetCoordState(x - 1, y) == neighbor || GetCoordState(x + 1, y) == neighbor || GetCoordState(x, y - 1) == neighbor || GetCoordState(x, y + 1) == neighbor;
    }

    // Checks if path along an element exisits from (x,y) to a goal
    public static bool AStar(int x, int y, char path, char goal)
    {
        Queue<int> queue = new Queue<int>();
        ArrayList visited = new ArrayList();
        int i = CoordToIndex(x, y);
        if (i == -1)
        {
            return false;
        }
        if (state[i] == goal)
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
                if (index != -1 && state[index] == path && !visited.Contains(index))
                {
                    queue.Enqueue(index);
                }
            }
        }
        return false;
    }
}
