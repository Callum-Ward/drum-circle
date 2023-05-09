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
    public int scene;
    private Vector3 currentWaypoint;
    private float moveSpeed = 15;
    public float rotationSpeed = 5f;  // The speed of rotation



    // Start is called before the first frame update
    void Start()
    {
        currentWaypoint = (platform.transform.position + (platformDistance * Vector3.Normalize(platform.transform.position - centre)));
        transform.position = new Vector3(currentWaypoint.x,currentWaypoint.y + yOffset, currentWaypoint.z); //Vector3.MoveTowards(transform.position, platform.transform.position, moveSpeed * Time.deltaTime);
        transform.LookAt(centre);
        leftCam.transform.position = transform.position;
        rightCam.transform.position = transform.position;


        Vector3 leftRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - sideCamRot, transform.eulerAngles.z);
        Vector3 rightRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + sideCamRot, transform.eulerAngles.z);

        leftCam.transform.rotation = Quaternion.Euler(leftRotation);
        rightCam.transform.rotation = Quaternion.Euler(rightRotation);
     
    }
    // Update is called once per frame
    void Update()
    {
        currentWaypoint = (platform.transform.position + (platformDistance * Vector3.Normalize(platform.transform.position - centre)));
        Vector3 newCamPosition = new Vector3(currentWaypoint.x, currentWaypoint.y + yOffset, currentWaypoint.z);
        transform.position = Vector3.MoveTowards(transform.position, newCamPosition, moveSpeed * Time.deltaTime);//new Vector3(currentWaypoint.x,currentWaypoint.y + yOffset, currentWaypoint.z); //
        if (scene == 3)
        {
            Vector3 direction = centre - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = newRotation;
        } else
        {
            transform.LookAt(centre);
        }
        


        // leftCam.transform.position = transform.position;
        // rightCam.transform.position = transform.position;

        //Vector3 leftRotation = new Vector3(0, transform.eulerAngles.y - sideCamRot, 0);
        //Vector3 rightRotation = new Vector3(0, transform.eulerAngles.y + sideCamRot, 0);

        // leftCam.transform.rotation = Quaternion.Euler(leftRotation);
        // rightCam.transform.rotation = Quaternion.Euler(rightRotation);
    }
}
