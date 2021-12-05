using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueLike.Systems;
using Pathfinding;
using RogueLike.Dungeons;
using Rect = RogueLike.Dungeons.Rect;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }
    DungeonGenerator mapGenerator;

    public GameObject spawnerPrefab;
    public GameObject player;
    public int level = 0;
    public float points = 0.0f;
    public float scaleUpTime = 15.0f;
    public float scaleUpTimer = 0.0f;

    List<GameObject> spawners = new List<GameObject>();

    // Start is called before the first frame update

    public void SetPoints(float point)
    {
        points += point;
        HUDController.instance.SetPoints(points);
    }
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        mapGenerator = GetComponent<DungeonGenerator>();
        StartGame();
    }

    public void StartGame()
    {
        player.GetComponent<PlayerController>().ResetPlayer();
        HUDController.instance.ShowDeathPanel(false);
        mapGenerator.seed = GlobalSettings.seed;
        mapGenerator.GenerateDungeon();
        DestroySpawners();
        GeneratePathGrids();
        DropPlayerToRoom();
    }

    public void StartRandomGame()
    {
        int seed = Random.Range(0, 1000000);
        GlobalSettings.seed = seed;
        Random.InitState(seed);
        StartGame();
    }

    public void PlayerDied()
    {
        HUDController.instance.ShowDeathPanel(true);
    }


    void DropPlayerToRoom()
    {
        var spawner = spawners[Random.Range(0,spawners.Count -1)];
        EnemySpawner es = spawner.GetComponent<EnemySpawner>();
        es.DropPlayer(player);

    }

    void ScaleUp()
    {
        level++;
        foreach (GameObject spawner in spawners)
        {
            EnemySpawner es = spawner.GetComponent<EnemySpawner>();
            es.ScaleToLevel(level);
        }
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
    void FixedUpdate()
    {
        scaleUpTimer += Time.deltaTime;
        if (scaleUpTimer > scaleUpTime)
        {
            ScaleUp();
            scaleUpTimer -= scaleUpTime;
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

