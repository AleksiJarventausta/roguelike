using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public GameObject ThrowablePrefab;
    public AudioClip launchClip;
    public bool ranged;
    public float range;
    public float width;
    public float damage;
    public float force;
    public float throwDamage;
    public float throwForce;
    public float damageTime;
    public float attackTime;
    public float meleeKnockback;
    public float meleeKnockbackTime;
    bool isAttacking;
    string currentAnimation;


    const string SHOOT="Shoot";
    const string IDLE="Idle";
    const string HIT="Hit";

    Animator animator;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void stopAttacking() {
        isAttacking = false;
    }

    public void UpdateWeaponRender(Vector2 position, Vector2 lookDirection) {
        transform.position = position;
        float angle = Angle(lookDirection);
        transform.rotation = Quaternion.Euler(0,0, 90 - angle);
        Vector3 a = Vector3.one;
        if (angle > 0) {
            a.y = 1f;
        } else {
            a.y = -1f;
        }
        transform.localScale = a;
    }

    float Angle(Vector2 p_vector2)
    {
        return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
    }

    public void Attack(Vector2 normalizedLookDirection, Vector2 position)
    {
        if (ranged)
        {
            Launch(normalizedLookDirection,position, ProjectilePrefab);
        } else {
            Melee(normalizedLookDirection,position);
        }
    }

    public void Throw(Vector2 normalizedLookDirection, Vector2 position)
    {
        Launch(normalizedLookDirection,position, ThrowablePrefab);
        Destroy(gameObject);
    }

    void Melee(Vector2 normalizedLookDirection, Vector2 position)
    {
        gameObject.GetComponentInChildren<Animator>().Play("Hit", -1, 0f);
        StartCoroutine(DealDamage(normalizedLookDirection, position));
    }

    IEnumerator DealDamage(Vector2 normalizedLookDirection, Vector2 position)
    {
        yield return new WaitForSeconds(damageTime);
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, range, LayerMask.NameToLayer("enemy"));
        if (hits == null) yield return null;
        for (int i = 0; i < hits.Length; i++) {
            Collider2D hit = hits[i];
            EnemyAI controller = hit.GetComponent<EnemyAI>();
            if (controller != null)
            {
                controller.Damage(damage, position, meleeKnockback, meleeKnockbackTime);
            }
        }
        yield return null;
    }

    void Launch(Vector2 normalizedLookDirection, Vector2 position, GameObject prefab) {
      GameObject projectileObject = Instantiate(
          prefab,
          position + Vector2.up * 0.5f,
          Quaternion.identity);

      Projectile projectile = projectileObject.GetComponent<Projectile>();
      projectile.Launch(normalizedLookDirection.normalized, 300);
    }


}
