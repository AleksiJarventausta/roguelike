using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform targetPosition;

    private Seeker seeker;
    private EnemyController controller;

    public Path path;

    public float speed = 2;
    public float speedScaling;
    public float damageRange;
    public float damage;
    public float damageScaling;
    public float maxHealth;
    public float maxHealthScaling;
    private float scaledHealth;
    public float damageTime;
    public float damagePointMultiplier;
    public float killPoints;
    private float damageTimer = 0.0f;

    public int level = 0;
    
    private bool damaging;
    GameController gc;

    public float pointScaling;
    private float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    private float repathRate = 0.5f;
    private float lastRepath = float.NegativeInfinity;

    private bool reachedEndOfPath;

    Rigidbody2D rb;
    public float seeRadius;
    public float loseRadius;
    public bool canMove = true;
    private float currentHealth;
    enum EnemyState {
        IDLE,
        CHASING
    };

    EnemyState enemyState = EnemyState.IDLE;
    // Start is called before the first frame update

    public void Awake() {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        scaledHealth = maxHealth + maxHealth*maxHealthScaling*level;
        currentHealth = scaledHealth;
        gc = GameController.instance;
        // If you are writing a 2D game you can remove this line
        // and use the alternative way to move sugggested further below.
        controller = GetComponent<EnemyController>();
    }

    public void OnPathComplete (Path p) {
        p.Claim(this);
        if (!p.error) {
            if (path != null) path.Release(this);
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        } else {
            p.Release(this);
        }
    }

    public void Move(Vector2 direction, float speedFactor)
    {
        rb.velocity = direction*(speedFactor*speed + speedFactor*speed*speedScaling*level);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            Debug.Log("Hitting Player");
            if(damaging)
            {
                if(canMove)
                {
                    damageTimer += Time.fixedDeltaTime;
                    if(damageTimer > damageTime)
                    {
                        pc.Damage(damage + damage*damageScaling*level);
                        damageTimer -= damageTime;
                    }
                }
            } 
            else
            {
                damaging = true;
            }
        }
    }
    public void ScaleToLevel(int level)
    {
        this.level = level;
        scaledHealth = maxHealth + maxHealth*maxHealthScaling*level;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            damaging = false;
            damageTimer = 0.0f;
        }
    }

    public void FixedUpdate () {
        if (Time.time > lastRepath + repathRate && seeker.IsDone() && targetPosition != null) {
            lastRepath = Time.time;

            seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
        }
        if (enemyState == EnemyState.IDLE)
        {
            SeePlayer(seeRadius);
        } else if (enemyState == EnemyState.CHASING) {
            SeePlayer(loseRadius);
        }

        if (path == null || !canMove) {
            return;
        }

        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true) {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    reachedEndOfPath = true;
                    break;
                }
            } else {
                break;
            }
        }
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;

        Move(dir, speedFactor);

    }
    bool SeePlayer(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.NameToLayer("player"));
        if (hits == null) return false;

        for (int i = 0; i < hits.Length; i++) {
            Collider2D hit = hits[i];
            PlayerController controller = hit.GetComponent<PlayerController>();
            if (controller != null)
            {
                if (enemyState == EnemyState.IDLE){
                    targetPosition = controller.transform;
                    enemyState = EnemyState.CHASING;
                    Debug.Log("Found player");
                }
                return true;
            }
        }

        if (enemyState == EnemyState.CHASING) {
            targetPosition = null;
            enemyState = EnemyState.IDLE;
        }
        return false;

    }

    public void Damage(float damage, Vector2 position, float knockback, float knocbackTime)
    {
        //TODO animation.
        currentHealth -= damage;
        PlayerController pc = targetPosition.GetComponent<PlayerController>();
        gc.SetPoints(damage * damagePointMultiplier + damage*damagePointMultiplier*pointScaling*level);
        if(currentHealth <= 0)
        {
            gc.SetPoints(killPoints + killPoints*pointScaling*level);
            Destroy(gameObject);
            return;
        }
        StartCoroutine(HandleKnockback(position, knockback, knocbackTime));
    }

    IEnumerator HandleKnockback(Vector2 position,  float knockback, float knockbackTime)
    {

        canMove = false;
        damageTimer = 0.0f;
        Vector2 direction =  rb.position-position;
        Debug.Log(direction.normalized);
        rb.AddForce(direction.normalized*knockback);
        yield return new WaitForSeconds(knockbackTime);
        canMove = true;
    }


}
