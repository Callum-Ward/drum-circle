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
        rightCam.transform.position = transform.position;

        Vector3 leftRotation = new Vector3(0, transform.eulerAngles.y - sideCamRot, 0);
        Vector3 rightRotation = new Vector3(0, transform.eulerAngles.y + sideCamRot, 0);
        leftCam.transform.rotation = Quaternion.Euler(leftRotation);
        rightCam.transform.rotation = Quaternion.Euler(rightRotation);
    }
}
