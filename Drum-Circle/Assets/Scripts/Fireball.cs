using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameObject explosionVFX;
    public BeatmapScript bms;
    private Vector3 heading;

    public void setHeading(Vector3 heading)
    {
        this.heading = heading;
    }

    void Start()
    {
        bms = GameObject.Find("RhythmLogic").GetComponent<BeatmapScript>();
    }

    void Update()
    {
        if(heading != null)
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            var rotation = Quaternion.LookRotation(heading - transform.position);
            Debug.Log("WHOH" + rotation);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, 95f * Time.deltaTime));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        bms.ST();
    }
}
