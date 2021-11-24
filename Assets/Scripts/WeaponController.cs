using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public AudioClip launchClip;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(Vector2 normalizedLookDirection, Vector2 position) {
    GameObject projectileObject = Instantiate(
        ProjectilePrefab,
        position + Vector2.up * 0.5f,
        Quaternion.identity);

    Projectile projectile = projectileObject.GetComponent<Projectile>();
    projectile.Launch(normalizedLookDirection.normalized, 300);
    }
}
