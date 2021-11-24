using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    SpriteRenderer spriteRenderer;

    public float speed;
    public float duration = 3.0f;
    float currentDuration;
    public float spinningSpeed = 0;
    bool rotateClockwise = true;


    void OnCollisionEnter2D(Collision2D other)
    {

        /*
        EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            Debug.Log("Hit enemy");
        }
        */

        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentDuration = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
        currentDuration -= Time.deltaTime;
        if (currentDuration < 0) {
            Destroy(gameObject);
        }

    }
    void FixedUpdate()
    {
        if (spinningSpeed > 0) {
            transform.Rotate(0,0, rotateClockwise? spinningSpeed: -spinningSpeed);
        }
    }
    public void Launch(Vector2 direction, float force)
    {
       rigidbody2d.AddForce(direction*force);
       float angle = Angle(direction);
       if (angle > 0) {
           spriteRenderer.flipY = true;
           rotateClockwise = false;
       }
       transform.rotation = Quaternion.Euler(0, 0, -90 - angle);

    }
    float Angle(Vector2 p_vector2)
    {
      return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
    }
}
