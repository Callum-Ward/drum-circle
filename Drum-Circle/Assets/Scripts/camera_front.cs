using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_front : MonoBehaviour
{
    public GameObject platform;
    public GameObject leftCam;
    public GameObject rightCam;
    public float sideCamRot = 80f;
    public Vector3 centre;
    public int platformDistance = 4;
    public int yOffset = 4;
    private Vector3 currentWaypoint;
    private float moveSpeed = 5;


    // Start is called before the first frame update
    void Start()
    {
     
    }
    // Update is called once per frame
    void Update()
    {
        currentWaypoint = (platform.transform.position + (platformDistance * Vector3.Normalize(platform.transform.position - centre)));
        transform.position = new Vector3(currentWaypoint.x,currentWaypoint.y + yOffset, currentWaypoint.z); //Vector3.MoveTowards(transform.position, platform.transform.position, moveSpeed * Time.deltaTime);
        transform.LookAt(centre);
        leftCam.transform.position = transform.position;
        leftCam.transform.rotation = Quaternion.Euler(transform.rotation.z, transform.rotation.y - sideCamRot, transform.rotation.x);
        //leftCam.transform.rotation = transform.rotation;
        //Debug.Log("A " + leftCam.transform.rotation);
        leftCam.transform.Rotate(0, -sideCamRot, 0);
        //Debug.Log("B " + leftCam.transform.rotation);
        rightCam.transform.position = transform.position;
        rightCam.transform.rotation = transform.rotation;//Quaternion.Euler(transform.rotation.x, transform.rotation.y + rightCamRot, transform.rotation.z);
        rightCam.transform.Rotate(0, sideCamRot, 0);
    }
}
