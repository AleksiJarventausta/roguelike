using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float movementSpeed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; }}
    int currentHealth;

    Animator animator;
    Vector2 moveDirection = new Vector2(1,0);
    Vector2 lookDirection = new Vector2(1,0);

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    public GameObject Weapon;
    GameObject usedWeapon;
    AudioSource audioSource;

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        usedWeapon = Instantiate(Weapon, rigidbody2d.position, Quaternion.identity);
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
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
        animator.SetBool("Mirror Idle",lookDirection.x < 0);

        UpdateWeaponRender();
        

        if(Input.GetKeyDown(KeyCode.Mouse1)) {
            Launch();
        }
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

    public void Launch()
    {

        WeaponController weaponController = usedWeapon.GetComponent<WeaponController>();
        weaponController.Launch(lookDirection.normalized, rigidbody2d.position);
        PlaySound(weaponController.launchClip);

        animator.SetTrigger("Launch");
    }
}
