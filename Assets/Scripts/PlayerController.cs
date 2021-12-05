using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rect = RogueLike.Dungeons.Rect;

public class PlayerController : MonoBehaviour
{

    public float movementSpeed = 3.0f;
    public float maxHealth = 5;
    public float timeInvincible = 2.0f;

    public float points = 0.0f;
    public bool isAlive= true;

    public float health { get { return currentHealth; }}
    float currentHealth;
    GameController gc;

    Animator animator;
    Vector2 moveDirection = new Vector2(1,0);
    Vector2 lookDirection = new Vector2(1,0);

    public AudioClip[] damageClips;
    public AudioClip deadClip;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    public GameObject Weapon;
    GameObject usedWeapon;
    AudioSource audioSource;

    WeaponController weaponController;

    public void GivePoints(float point)
    {
        gc.SetPoints(point);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        gc = GameController.instance;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        usedWeapon = Instantiate(Weapon, rigidbody2d.position, Quaternion.identity);
        weaponController = usedWeapon.GetComponent<WeaponController>();
        currentHealth = maxHealth;
        HUDController.instance.SetHealth(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
            return;
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(horizontal, vertical);


        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        lookDirection = GetMouseVectorFromPlayer();

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (weaponController != null)
        {
        weaponController.UpdateWeaponRender(rigidbody2d.position, lookDirection);
        }

        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            Launch();
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1)) {
            Throw();
        }
    }

    public void AddWeapon(GameObject weapon)
    {
        Weapon = weapon;
        Destroy(usedWeapon);
        usedWeapon = Instantiate(Weapon, rigidbody2d.position, Quaternion.identity);
        weaponController = usedWeapon.GetComponent<WeaponController>();
    }

    float Angle(Vector2 p_vector2)
    {
        return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
    }

    void UpdateWeaponRender() {
        usedWeapon.transform.position = rigidbody2d.position;
        float angle = Angle(lookDirection);
        usedWeapon.transform.rotation = Quaternion.Euler(0,0, 90- angle);
        Vector3 a = Vector3.one;
        if (angle > 0) {
            a.y = 1f;
        } else {
            a.y = -1f;
        }
        usedWeapon.transform.localScale = a;
    }


    Vector2 GetMouseVectorFromPlayer() {
        Vector3 mousePos = GetMouseWorldPosition();
        Vector2 playerPos = transform.position;
        return new Vector2( mousePos.x - playerPos.x ,  mousePos.y - playerPos.y);

    }

    Vector3 GetMouseWorldPosition() {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
    }

    void FixedUpdate()
    {
        Vector2 position = transform.position;
        position.x = position.x + movementSpeed * horizontal * Time.deltaTime;
        position.y = position.y + movementSpeed * vertical * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount) {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }
    public void ResetPlayer()
    {
        isAlive = true;
        currentHealth = maxHealth;
        HUDController.instance.SetHealth(1);
    }

    public void Damage(float damage)
    {
        if (!isAlive)
            return;
        currentHealth -= damage;
        HUDController.instance.SetHealth(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            isAlive = false;
            gc.PlayerDied();
            PlaySound(deadClip);

        }
        PlaySound(damageClips[Random.Range(0, damageClips.Length)]);
    }

    public void Launch()
    {

        WeaponController weaponController = usedWeapon.GetComponent<WeaponController>();
        weaponController.Attack(lookDirection.normalized, rigidbody2d.position);
        PlaySound(weaponController.launchClip);

    }
    //TODO: sound clip
    public void Throw()
    {

        WeaponController weaponController = usedWeapon.GetComponent<WeaponController>();
        PlaySound(weaponController.launchClip);
        weaponController.Throw(lookDirection.normalized, rigidbody2d.position);

    }
}
