using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    Rigidbody2D rb;
    public float seeRadius;
    public float loseRadius;
    public float maxHealth;
    public float speed;
    float currentHealth;
    enum EnemyState {
        IDLE,
        CHASING
    };

    EnemyState enemyState = EnemyState.IDLE;
    // Start is called before the first frame update

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {

    }




}
