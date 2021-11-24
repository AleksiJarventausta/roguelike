using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollectable : MonoBehaviour
{
    public GameObject Weapon;
    public AudioClip collectedClip;
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered");
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
          controller.AddWeapon(Weapon);
          Destroy(gameObject);
          controller.PlaySound(collectedClip);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
