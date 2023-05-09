using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject platform;
    public GameObject fireball;
    public GameObject drum;
    public GameObject cam;

    private GameObject[] drums = new GameObject[3];
    private Vector3[] platformOffsets = {new Vector3(-1f, 2f, 2f), new Vector3(0f, 2f, 0f), new Vector3(3f, 1f, 2f)};

    // Start is called before the first frame update
    void Start()
    {
        /* for(int i = 0; i < platformOffsets.Length; i++)
        {
            Vector3 position = new Vector3(
                platform.transform.position.x + platformOffsets[i].x,
                platform.transform.position.y + platformOffsets[i].y,
                platform.transform.position.z + platformOffsets[i].z
            );

            GameObject d = Instantiate(drum, position, transform.rotation) as GameObject;
            d.transform.localScale = new Vector3(15f, 15f, 15f);
            drums[i] = d;
        } */
    }

    void Update()
    {
        /* for(int i = 0; i < platformOffsets.Length; i++)
        {
            Vector3 position = new Vector3(
                platform.transform.position.x + platformOffsets[i].x,
                platform.transform.position.y + platformOffsets[i].y,
                platform.transform.position.z + platformOffsets[i].z
            );
            drums[i].transform.position = position; 
        } */
    }

    public void spawn(int playerIndex)
    {
        Vector3 pos = new Vector3(
                platform.transform.position.x + platformOffsets[playerIndex].x,
                platform.transform.position.y + platformOffsets[playerIndex].y,
                platform.transform.position.z + platformOffsets[playerIndex].z
        );
        GameObject newFireball = Instantiate(fireball, pos, cam.transform.rotation) as GameObject;
        newFireball.GetComponent<Fireball>().StartUp(playerIndex);
    }
}