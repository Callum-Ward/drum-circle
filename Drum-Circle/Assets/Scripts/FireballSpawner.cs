using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject platform;
    public GameObject fireball;


    public void spawn(int playerIndex)
    {
        Vector3 pos = platform.transform.position + new Vector3(0f, 2f, 0f);
        GameObject newFireball = Instantiate(fireball, pos, transform.rotation) as GameObject;
        newFireball.GetComponent<Fireball>().StartUp(playerIndex);
    }
}