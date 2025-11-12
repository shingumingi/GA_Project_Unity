using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomMaze : MonoBehaviour
{
    [SerializeField] int mapX;
    [SerializeField] int mapY;

    bool[,] visited;
    int[,] map;

    [SerializeField] GameObject WallPrefab;
    [SerializeField] GameObject routePrefab;

    Vector2Int goal;
    Vector2Int[] dirs = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
    List<GameObject> obj = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeMaze();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Search(1, 1);
        }
    }

    void MakeMaze()
    {
        foreach(GameObject q in obj.ToList())
        {
            Destroy(q);
        }
        obj.Clear();

        map = new int[mapX, mapY];

        for (int x = 0; x < mapY; x++)
        {
            for (int y = 0; y < mapX; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);

                if (x == 0 || y == 0 || x == mapX - 1 || y == mapY - 1)
                {
                    map[x, y] = 1;
                    obj.Add(Instantiate(WallPrefab, pos, default));
                }
                else
                {
                    map[x, y] = Random.Range(0, 10);
                    if (map[x,y] == 1)
                        obj.Add(Instantiate(WallPrefab, pos, default));
                }
            }
        }

        goal = new Vector2Int(mapX - 2, mapY - 2);
        visited = new bool[mapX, mapY];
    }

    bool Search(int x, int y)
    {
        if (x < 0 || y < 0 || x >= mapX || y >= mapY) return false;
        if (map[x, y] == 1 || visited[x, y]) return false;

        visited[x, y] = true;

         if (x == goal.x && y == goal.y) return true;

        foreach (var d in dirs)
            if (Search(x + d.x, y + d.y))
            {
                obj.Add(Instantiate(routePrefab, new Vector3(x, 0.1f, y), default));
                return true;
            }
        return false;
    }
}
