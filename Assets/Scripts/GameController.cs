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
    EnemySpawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = GetComponent<DungeonGenerator>();
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
            gg.is2D = true;
            gg.center = new Vector3(r.LowerLeftPos.x + r.Width/2, r.LowerLeftPos.y + r.Height/2, 0);
            gg.SetDimensions(r.Width, r.Height, 1);

        }
        AstarPath.active.Scan();
    }
}

