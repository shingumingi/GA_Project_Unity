using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEditor.PlayerSettings;

public class MazeBFS : MonoBehaviour
{
    [SerializeField] int mapX;
    [SerializeField] int mapY;

    int[,] map;

    Vector2Int start = new Vector2Int(1,1);
    Vector2Int goal;
    bool[,] visited;
    Vector2Int?[,] parent;
    Vector2Int[] dirs =
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
    };

    [SerializeField] GameObject WallPrefab;
    [SerializeField] GameObject routePrefab;
    [SerializeField] GameObject farthestPrefab;
    [SerializeField] Transform player;
    [SerializeField] float moveSpeed = 3f;

    List<GameObject> obj = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void ShowMaze()
    {
        MakeMaze();
    }

    public void ShowPath()
    {
        List<Vector2Int> path = FindPathBFS();
    }

    public void AutoMove()
    {
        List<Vector2Int> path = FindPathBFS();
        if (path == null) return;

        player.transform.position = new Vector3(start.x, 0, start.y);

        StopAllCoroutines();
        StartCoroutine(MoveAlongPath(path));
    }

    void Update()
    {

    }

    void MakeMaze()
    {
        foreach (GameObject q in obj.ToList())
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
                    map[x, y] = Random.Range(0, 3);
                    map[1, 1] = 0;
                    map[mapX - 2, mapY - 2] = 0;
                    if (map[x, y] == 1)
                        obj.Add(Instantiate(WallPrefab, pos, default));
                }
            }
        }

        player.transform.position = new Vector3(start.x, 0, start.y);

        goal = new Vector2Int(mapX - 2, mapY - 2);
        visited = new bool[mapX, mapY];
    }

    List<Vector2Int> FindPathBFS()
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        visited = new bool[w, h];
        parent = new Vector2Int?[w, h];
        int[,] depth = new int[w, h];

        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(start);
        visited[start.x, start.y] = true;
        depth[start.x, start.y] = 0;

        bool goalFound = false;

        while (q.Count > 0)
        {
            Vector2Int cur = q.Dequeue();

            if (cur == goal)
            {
                goalFound = true;
            }

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(nx, ny)) continue;
                if (map[nx, ny] == 1) continue;
                if (visited[nx, ny]) continue;

                visited[nx, ny] = true;
                parent[nx, ny] = cur;
                depth[nx, ny] = depth[cur.x, cur.y] + 1;

                q.Enqueue(new Vector2Int(nx, ny));
            }
        }
        List<Vector2Int> goalPath = null;
        if (goalFound)
        {
            goalPath = ReconstructPath();
            Debug.Log("BFS : Goal Path 생성 완료");
        }

        int maxDepth = -1;
        Vector2Int farthest = start;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (visited[x, y] && depth[x, y] > maxDepth)
                {
                    maxDepth = depth[x, y];
                    farthest = new Vector2Int(x, y);
                }
            }
        }

        Vector3 pos = new Vector3(farthest.x, 0.5f, farthest.y);
        obj.Add(Instantiate(farthestPrefab, pos, default));

        return goalPath;
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
    }

    List<Vector2Int> ReconstructPath()
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = goal;

        while (cur.HasValue)
        {
            path.Add(cur.Value);
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();
        Debug.Log($"경로 길이 : {path.Count}");
        foreach(var p in path)
        {
            obj.Add(Instantiate(routePrefab, new Vector3(p.x, 0.1f, p.y), default));
        }
        return path;
    }

    IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        foreach (var p in path)
        {
            Vector3 targetPos = new Vector3(p.x, 0, p.y);

            while (Vector3.Distance(player.position, targetPos) > 0.05f)
            {
                player.position = Vector3.MoveTowards(
                    player.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            player.position = targetPos;
            yield return new WaitForSeconds(0.01f);
        }

        Debug.Log("자동 이동 완료!");
    }
}
