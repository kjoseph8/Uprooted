using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RuleTile rootTile;
    [HideInInspector] public char[] state = new char[31*31];
    private Plane _tilePlane;
    private Tilemap waterMap;
    private Tilemap roots1Map;
    private Tilemap roots2Map;
    private int player = 1;

    // Start is called before the first frame update
    void Start()
    {
        _tilePlane = new Plane(Vector3.back, Vector3.zero);
        waterMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
        roots1Map = GameObject.FindGameObjectWithTag("Roots1").GetComponent<Tilemap>();
        roots2Map = GameObject.FindGameObjectWithTag("Roots2").GetComponent<Tilemap>();
        Tilemap outlineMap = GameObject.FindGameObjectWithTag("Outline").GetComponent<Tilemap>();
        
        BoundsInt bounds = outlineMap.cellBounds;
        TileBase[] outlineTiles = outlineMap.GetTilesBlock(bounds);
        TileBase[] waterTiles = waterMap.GetTilesBlock(bounds);
        TileBase[] roots1Tiles = roots1Map.GetTilesBlock(bounds);
        TileBase[] roots2Tiles = roots2Map.GetTilesBlock(bounds);
        for (int i = 0; i < 31*31; i++)
        {
            if (waterTiles[i] != null)
            {
                state[i] = 'W';
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int[] coord = MouseToCoord();
            int index = CoordToIndex(coord[0], coord[1]);
            if (index != -1 && state[index] != 0 && state[index] != 1 && state[index] != 2)
            {
                if (player == 1)
                {
                    if (AStar(coord[0], coord[1], '1', '!'))
                    {
                        state[index] = '1';
                        roots1Map.SetTile(new Vector3Int(coord[0], coord[1]), rootTile);
                        player = 2;
                    }
                }
                else
                {
                    if (AStar(coord[0], coord[1], '2', '@'))
                    {
                        state[index] = '2';
                        roots2Map.SetTile(new Vector3Int(coord[0], coord[1]), rootTile);
                        player = 1;
                    }
                }
            }
        }
    }

    // For Debug Purposes: Get a String Representation of Game State
    string StateToString()
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
    int[] MouseToCoord()
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
    int CoordToIndex(int x, int y)
    {
        if (x < 0 || x > 30 || y < 0 || y > 30)
        {
            return -1;
        }
        return x + y * 31;
    }

    int[] IndexToCoord(int i)
    {
        int[] coord = new int[2];
        coord[0] = i % 31;
        coord[1] = i / 31;
        return coord;
    }

    // Get state at specific coordinate
    char GetCoordState(int x, int y)
    {
        int i = CoordToIndex(x, y);
        if (i == -1)
        {
            return '0';
        }
        return state[i];
    }

    // Get state of element at mouse position
    char GetMouseState()
    {
        int[] coord = MouseToCoord();
        return GetCoordState(coord[0], coord[1]);
    }

    bool AStar(int x, int y, char path, char goal)
    {
        Queue<int> queue = new Queue<int>();
        ArrayList visited = new ArrayList();
        int i = CoordToIndex(x, y);
        if (i != -1 && state[i] == goal)
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
            int[,] dirs = { { x-1, y }, { x+1, y }, { x, y-1 }, { x, y+1 } };
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
