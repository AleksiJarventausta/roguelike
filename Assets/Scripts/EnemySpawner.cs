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
    public Rect rect;
    public int maxCollectables = 2;

    private bool playerIn;
    private int enemyCount;
    private int collectableCount;
    private float timer = 0.0f;
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> collectables = new List<GameObject>();


    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            playerIn = true;
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
            DestroyEnemies();
            enemyCount = 0;
            DestroyCollectables();
            collectableCount = 0;
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

    void Awake()
    {
    }

    void FixedUpdate()
    {
        if(playerIn)
        {
          if (collectableCount < maxCollectables)
          {
              DropCollectable();
              collectableCount++;
          }
        }

        timer += Time.deltaTime;
        if (timer > dropRate)
        {
            if(enemyCount < MaxEnemies)
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
        GameObject collectablePref = collectablePrefabs[Random.Range(0, collectablePrefabs.Length -1)];
        int randomX = Random.Range(rect.LowerLeftPos.x + 1, rect.LowerLeftPos.x + rect.Width - 1);
        int randomY = Random.Range(rect.LowerLeftPos.y +1, rect.GetTopRight().y -1);
        GameObject enemy = Instantiate(collectablePref, new Vector3(randomX, randomY, 0), Quaternion.identity);
        collectables.Add(enemy);
    }


    void DropEnemy()
    {
        GameObject enemyPref = enemyPrefabs[Random.Range(0, enemyPrefabs.Length -1)];
        int randomX = Random.Range(rect.LowerLeftPos.x + 1, rect.LowerLeftPos.x + rect.Width - 1);
        int randomY = Random.Range(rect.LowerLeftPos.y +1, rect.GetTopRight().y -1);
        GameObject enemy = Instantiate(enemyPref, new Vector3(randomX, randomY, 0), Quaternion.identity);
        enemies.Add(enemy);
    }
}
