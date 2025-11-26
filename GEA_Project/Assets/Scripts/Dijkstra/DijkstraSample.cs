using System.Collections.Generic;
using UnityEngine;

public class DijkstraSample : MonoBehaviour
{
    [SerializeField] int mapX = 21;
    [SerializeField] int mapY = 21;

    int[,] map;
    GameObject[,] tileObjs;
    List<GameObject> pathObjs = new List<GameObject>();

    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject pathPrefab;
    [SerializeField] Transform tileRoot;

    [SerializeField] Color wallColor = new Color(0.15f, 0.07f, 0.02f);
    [SerializeField] Color groundColor = Color.white;
    [SerializeField] Color forestColor = new Color(0.6f, 1.0f, 0.6f);
    [SerializeField] Color mudColor = new Color(1.0f, 0.7f, 0.7f);
    [SerializeField] Color startColor = Color.blue;
    [SerializeField] Color goalColor = Color.red;
    [SerializeField] Color pathColor = Color.green;

    readonly Vector2Int[] dirs =
    {
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
        new Vector2Int( 0,-1),
    };

    void Start()
    {
       
    }

    void Update()
    {
            
    }

    public void ShowMaze()
    {
        MakeMaze();
    }

    public void ShowPath()
    {
        ShowShortestPath();
    }

    public void MakeMaze()
    {
        if (mapX < 3) mapX = 3;
        if (mapY < 3) mapY = 3;

        start = new Vector2Int(1, 1);
        goal = new Vector2Int(mapX - 2, mapY - 2);

        const int maxTry = 50;
        int tryCount = 0;
        bool ok = false;

        while (!ok && tryCount < maxTry)
        {
            GenerateRandomMapOnce();
            ok = CanEscapeDFS();
            tryCount++;
        }

        BuildVisual();
    }

    public void ShowShortestPath()
    {
        if (map == null) return;

        var path = Dijkstra(map, start, goal);
        if (path == null) return;

        ClearPathVisual();
        RefreshTileColors();

        foreach (var p in path)
        {
            if (p == start || p == goal) continue;

            if (tileObjs != null && tileObjs[p.x, p.y] != null)
            {
                var rend = tileObjs[p.x, p.y].GetComponent<Renderer>();
                if (rend != null) rend.material.color = pathColor;
            }

            if (pathPrefab != null)
            {
                var pos = new Vector3(p.x, 0.5f, p.y);
                var o = Instantiate(pathPrefab, pos, Quaternion.identity, tileRoot);
                pathObjs.Add(o);
            }
        }
    }

    void GenerateRandomMapOnce()
    {
        map = new int[mapX, mapY];

        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                if (x == 0 || y == 0 || x == mapX - 1 || y == mapY - 1)
                {
                    map[x, y] = 0;
                }
                else
                {
                    float r = Random.value;
                    if (r < 0.20f) map[x, y] = 0;
                    else if (r < 0.55f) map[x, y] = 1;
                    else if (r < 0.80f) map[x, y] = 2;
                    else map[x, y] = 3;
                }
            }
        }

        map[start.x, start.y] = 1;
        map[goal.x, goal.y] = 1;
    }

    bool CanEscapeDFS()
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        bool[,] visited = new bool[w, h];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        stack.Push(start);
        visited[start.x, start.y] = true;

        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            if (cur == goal) return true;

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(map, nx, ny)) continue;
                if (map[nx, ny] == 0) continue;
                if (visited[nx, ny]) continue;

                visited[nx, ny] = true;
                stack.Push(new Vector2Int(nx, ny));
            }
        }

        return false;
    }

    void BuildVisual()
    {
        if (tileObjs != null)
        {
            for (int x = 0; x < tileObjs.GetLength(0); x++)
                for (int y = 0; y < tileObjs.GetLength(1); y++)
                    if (tileObjs[x, y] != null)
                        Destroy(tileObjs[x, y]);
        }
        ClearPathVisual();

        tileObjs = new GameObject[mapX, mapY];

        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);
                GameObject tile;

                if (map[x, y] == 0 && wallPrefab != null)
                    tile = Instantiate(wallPrefab, pos, Quaternion.identity, tileRoot);
                else
                    tile = Instantiate(tilePrefab, pos, Quaternion.identity, tileRoot);

                tileObjs[x, y] = tile;

                var rend = tile.GetComponent<Renderer>();
                if (rend != null) rend.material.color = GetTileColor(x, y);
            }
        }
    }

    void RefreshTileColors()
    {
        if (tileObjs == null) return;

        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                var tile = tileObjs[x, y];
                if (tile == null) continue;

                var rend = tile.GetComponent<Renderer>();
                if (rend != null) rend.material.color = GetTileColor(x, y);
            }
        }
    }

    void ClearPathVisual()
    {
        foreach (var o in pathObjs)
            if (o != null) Destroy(o);
        pathObjs.Clear();
    }

    Color GetTileColor(int x, int y)
    {
        if (x == start.x && y == start.y) return startColor;
        if (x == goal.x && y == goal.y) return goalColor;

        int t = map[x, y];
        switch (t)
        {
            case 0: return wallColor;
            case 1: return groundColor;
            case 2: return forestColor;
            case 3: return mudColor;
            default: return Color.magenta;
        }
    }

    List<Vector2Int> Dijkstra(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        int[,] dist = new int[w, h];
        bool[,] visited = new bool[w, h];
        Vector2Int?[,] parent = new Vector2Int?[w, h];

        const int INF = int.MaxValue;

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                dist[x, y] = INF;

        dist[start.x, start.y] = 0;

        PriorityQueue<Node> pq = new PriorityQueue<Node>();
        pq.Enqueue(new Node(start, 0));

        while (pq.Count > 0)
        {
            Node node = pq.Dequeue();
            Vector2Int cur = node.pos;

            if (visited[cur.x, cur.y]) continue;
            visited[cur.x, cur.y] = true;

            if (cur == goal)
                return ReconstructPath(parent, start, goal);

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(map, nx, ny)) continue;
                if (map[nx, ny] == 0) continue;

                int moveCost = TileCost(map[nx, ny]);
                if (moveCost == INF) continue;

                int newDist = dist[cur.x, cur.y] + moveCost;

                if (newDist < dist[nx, ny])
                {
                    dist[nx, ny] = newDist;
                    parent[nx, ny] = cur;
                    pq.Enqueue(new Node(new Vector2Int(nx, ny), newDist));
                }
            }
        }

        return null;
    }

    int TileCost(int tile)
    {
        switch (tile)
        {
            case 1: return 1;
            case 2: return 3;
            case 3: return 5;
            default: return int.MaxValue;
        }
    }

    bool InBounds(int[,] map, int x, int y)
    {
        return x >= 0 && y >= 0 &&
               x < map.GetLength(0) &&
               y < map.GetLength(1);
    }

    List<Vector2Int> ReconstructPath(Vector2Int?[,] parent, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = goal;

        while (cur.HasValue)
        {
            path.Add(cur.Value);
            if (cur.Value == start) break;
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();
        return path;
    }

    class Node : System.IComparable<Node>
    {
        public Vector2Int pos;
        public int dist;

        public Node(Vector2Int p, int d)
        {
            pos = p;
            dist = d;
        }

        public int CompareTo(Node other)
        {
            return dist.CompareTo(other.dist);
        }
    }

    class PriorityQueue<T> where T : System.IComparable<T>
    {
        readonly List<T> data = new List<T>();
        public int Count => data.Count;

        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1;
            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (data[ci].CompareTo(data[pi]) >= 0) break;
                (data[ci], data[pi]) = (data[pi], data[ci]);
                ci = pi;
            }
        }

        public T Dequeue()
        {
            int li = data.Count - 1;
            T frontItem = data[0];
            data[0] = data[li];
            data.RemoveAt(li);
            li--;
            int pi = 0;

            while (true)
            {
                int ci = pi * 2 + 1;
                if (ci > li) break;
                int rc = ci + 1;
                if (rc <= li && data[rc].CompareTo(data[ci]) < 0)
                    ci = rc;
                if (data[pi].CompareTo(data[ci]) <= 0) break;
                (data[pi], data[ci]) = (data[ci], data[pi]);
                pi = ci;
            }

            return frontItem;
        }
    }
}
