using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_front : MonoBehaviour
{
    public GameObject platform;
    public GameObject leftCam;
    public GameObject rightCam;
    public float sideCamRot = 80f;
    public Vector3 centre; //position cameras should look at
    public int platformDistance = 4; //distance camera should maintain from camera
    public int yOffset = 4; //height above platform
    public int scene;
    private Vector3 currentWaypoint;
    private float moveSpeed = 15f; //moving camera around platform to maintain focus
    public float rotationSpeed = 1f;  //used for rotating camera between different targets


    //used to control the main camera, left and right cameras are set as children making them follow 


    // Start is called before the first frame update
    void Start()
    {
        currentWaypoint = (platform.transform.position + (platformDistance * Vector3.Normalize(platform.transform.position - centre)));  //use distance between platform and centre to obtain normalised vector, use this to offset camera from platform in correct direction
        transform.position = new Vector3(currentWaypoint.x,currentWaypoint.y + yOffset, currentWaypoint.z); //add yoffset to camera
        transform.LookAt(centre); //lookat function sets rotation values to aim at centre 
        leftCam.transform.position = transform.position; //set left and right camera positions to main camera positon
        rightCam.transform.position = transform.position;


        Vector3 leftRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - sideCamRot, transform.eulerAngles.z); //rotate cameras away from centre for panoramic view
        Vector3 rightRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + sideCamRot, transform.eulerAngles.z);

        leftCam.transform.rotation = Quaternion.Euler(leftRotation);
        rightCam.transform.rotation = Quaternion.Euler(rightRotation);
     
    }
    // Update is called once per frame
    void Update()
    {
        currentWaypoint = (platform.transform.position + (platformDistance * Vector3.Normalize(platform.transform.position - centre))); //upadte camera position to keep platform in focus 
        Vector3 newCamPosition = new Vector3(currentWaypoint.x, currentWaypoint.y + yOffset, currentWaypoint.z); 
        transform.position = Vector3.MoveTowards(transform.position, newCamPosition, moveSpeed * Time.deltaTime); //move towards new camera position to maintain smooth camera transitions
        if (scene == 3) //beach scene contains camera centre changes
        {
            Vector3 direction = centre - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); //rotate camera smoothly between centre points, focusing on closest island
            transform.rotation = newRotation;
        } else
        {
            transform.LookAt(centre);
        }
        
    }
}
