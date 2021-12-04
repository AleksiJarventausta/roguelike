using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class EnemySpawner
{
    public GameObject[] enemyPrefab;
    public int2 pos;
    public int Width;
    public int Height;
    public int MaxEnemies;
    public float dropRate;

    private int enemyCount;
    private float timer = 0.0f;
    private List<GameObject> enemies = new List<GameObject>();

    void Awake()
    {
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > dropRate)
        {
            if(enemyCount < MaxEnemies)
            {
                DropEnemy();
            }
            timer -= dropRate;
        }
    }

    void DropEnemy()
    {
        int randomX = Random.Range(pos.x + 1, pos.x + Width - 1);
        int randomY = Random.Range(pos.y +1, pos.y + Height -1);

        Instantiate(enemyPrefab[0], new Vector3(randomX, randomY, 0), Quaternion.identity);

    }

}
