using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using Rect = RogueLike.Dungeons.Rect;

public class EnemySpawner: MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] collectablePrefabs;
    public int MaxEnemies;
    public float dropRate;
    public float dropRateScaling;
    private float scaledDropRate;
    
    public Rect rect;
    public int maxCollectables = 2;
    private int scaledCollectables ;
    private int scaledEnemies;
    public int growMaxCollectableLevel= 5;
    public int growMaxEnemiesLevel = 1;
    public int level;

    public float destroyTime = 10.0f;
    public float destroyTimer = 0.0f;


    private bool playerIn;
    private int enemyCount;
    private int collectableCount;
    private float timer = 0.0f;
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> collectables = new List<GameObject>();

    public void ScaleToLevel(int gameLevel)
    {
        level = gameLevel;
        scaledCollectables = maxCollectables  + (int) Mathf.Floor(level/growMaxCollectableLevel);
        scaledEnemies = MaxEnemies + (int) Mathf.Floor(level/growMaxCollectableLevel);
        scaledDropRate = dropRate*(1 - dropRateScaling*level);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            playerIn = true;
            destroyTimer = 0.0f;
            Debug.Log("Player entered room");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            Debug.Log("Player left the room");
            playerIn = false;
        }
    }

    void DestroyEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        enemies.Clear();
    }

    public void DropPlayer(GameObject player)
    {
        int randomX = Random.Range(rect.LowerLeftPos.x + 1, rect.LowerLeftPos.x + rect.Width - 1);
        int randomY = Random.Range(rect.LowerLeftPos.y +1, rect.GetTopRight().y -1);
        player.transform.position = new Vector3(randomX, randomY,0);
    }

    void Awake()
    {
        scaledDropRate = dropRate;
        scaledEnemies = MaxEnemies;
        scaledCollectables = maxCollectables;
    }

    void FixedUpdate()
    {
        if(playerIn)
        {
          if (collectableCount < scaledCollectables)
          {
              DropCollectable();
              collectableCount++;
          }
        }
        else
        {
            destroyTimer += Time.fixedDeltaTime;
            if(destroyTimer> destroyTime)
            {
                DestroyEnemies();
                enemyCount = 0;
                DestroyCollectables();
                collectableCount = 0;
                destroyTimer = 0.0f;
            }

        }

        timer += Time.fixedDeltaTime;
        if (timer > scaledDropRate)
        {
            if(enemyCount < scaledEnemies)
            {
                if (playerIn) {
                  DropEnemy();
                  enemyCount++;
                }
            }
            timer -= dropRate;
        }
    }

    void DestroyCollectables()
    {
        foreach (GameObject collectable in collectables)
        {
            if (collectable != null)
                Destroy(collectable);
        }
        collectables.Clear();
    }

    void DropCollectable()
    {
        GameObject collectablePref = collectablePrefabs[Random.Range(0, collectablePrefabs.Length )];
        int randomX = Random.Range(rect.LowerLeftPos.x + 1, rect.LowerLeftPos.x + rect.Width - 1);
        int randomY = Random.Range(rect.LowerLeftPos.y +1, rect.GetTopRight().y -1);
        GameObject enemy = Instantiate(collectablePref, new Vector3(randomX, randomY, 0), Quaternion.identity);
        collectables.Add(enemy);
    }

    void OnDestroy()
    {
        DestroyCollectables();
        DestroyEnemies();
    }

    void DropEnemy()
    {
        GameObject enemyPref = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        int randomX = Random.Range(rect.LowerLeftPos.x + 1, rect.LowerLeftPos.x + rect.Width - 1);
        int randomY = Random.Range(rect.LowerLeftPos.y +1, rect.GetTopRight().y -1);
        GameObject enemy = Instantiate(enemyPref, new Vector3(randomX, randomY, 0), Quaternion.identity);
        enemy.GetComponent<EnemyAI>().ScaleToLevel(level);
        enemies.Add(enemy);
    }
}
