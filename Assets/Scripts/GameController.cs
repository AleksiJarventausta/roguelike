using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueLike.Systems;
using Pathfinding;
using RogueLike.Dungeons;
using Rect = RogueLike.Dungeons.Rect;

public class GameController : MonoBehaviour
{
    DungeonGenerator mapGenerator;
    public GameObject spawnerPrefab;
    public GameObject player;

    List<GameObject> spawners = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = GetComponent<DungeonGenerator>();
        mapGenerator.seed = GlobalSettings.seed;
        mapGenerator.GenerateDungeon();
        GeneratePathGrids();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            mapGenerator.GenerateDungeon();
            GeneratePathGrids();
        }
    }

    void DestroySpawners()
    {
        foreach (GameObject spawner in spawners)
        {
            if (spawner != null)
                Destroy(spawner);
        }
        spawners.Clear();

    }

    void GeneratePathGrids()
    {
        var rooms = mapGenerator.GetRooms();


        AstarData data = AstarPath.active.data;
        foreach (NavGraph graph in data.graphs)
        {
            if( graph != null)
              data.RemoveGraph(graph);
        }

        foreach(Room room in rooms)
        {
            Rect r = room.GetRect();
            GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;
            Vector3 centerPoint = new Vector3(r.LowerLeftPos.x + r.Width/2, r.LowerLeftPos.y + r.Height/2, 0);
            gg.is2D = true;
            gg.collision.use2D=true;
            gg.center = centerPoint;
            gg.SetDimensions(r.Width, r.Height, 1);
            GameObject es = Instantiate(spawnerPrefab, new Vector3(r.LowerLeftPos.x, r.LowerLeftPos.y, 0), Quaternion.identity);
            es.transform.position = centerPoint;
            es.transform.localScale = new Vector3(r.Width, r.Height, 1);
            EnemySpawner ensp = es.GetComponent<EnemySpawner>();
            ensp.rect = r;
            spawners.Add(es);

        }
        AstarPath.active.Scan();
    }
}

