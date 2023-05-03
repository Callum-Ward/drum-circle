using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject platform;
    public GameObject fireball;
    public float speed = 10f;

    public void spawn(Vector3 heading)
    {
        Vector3 pos = platform.transform.position + new Vector3(0f, 2f, 0f);
        GameObject newFireball = Instantiate(fireball, pos, transform.rotation) as GameObject;
        Rigidbody rb = newFireball.GetComponent<Rigidbody>();
        rb.velocity = platform.transform.forward * speed;
        newFireball.GetComponent<Fireball>().setHeading(heading);
        Debug.Log("fireball at " + platform.transform.position);
    }
}