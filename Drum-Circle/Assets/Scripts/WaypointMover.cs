using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints; //refernece to waypoints list
    [SerializeField] private float switchDistance = 0.1f;
    [SerializeField] private float musicDuration;

    private Transform currentWaypoint;
    private Transform nextWaypoint;

    private bool start = false;
    private float moveSpeed = 0f;
    private float moveSpeedMax;


    // Start is called before the first frame update

    void Start()
    {
        //Assign initial waypoint to first waypoint object
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint); 
        //Move to first waypoint
        transform.position = currentWaypoint.position;
        //Find next waypoint to move to
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
       
        //nextWaypoint = waypoints.GetNextWaypoint(currentWaypoint);

        //Iterate through waypoints to find total distance, then divide it by song duration
        float totalDistance = 0f;
        Transform moveSpeedTransform = currentWaypoint;
        Transform moveSpeedTransformNext = waypoints.GetNextWaypoint(currentWaypoint);
        while(moveSpeedTransformNext != null)
        {
            totalDistance += Vector3.Distance(moveSpeedTransform.position, moveSpeedTransformNext.position);
            moveSpeedTransform = moveSpeedTransformNext;
            moveSpeedTransformNext = waypoints.GetNextWaypoint(moveSpeedTransformNext);
        }

        moveSpeedMax = totalDistance / musicDuration;

    }

    // Update is called once per frame
    void Update()
    {
        if(start == true && currentWaypoint != null)
        {
            if(moveSpeed < moveSpeedMax)
            {
                moveSpeed += 0.05f;
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position,currentWaypoint.position) < switchDistance)
            {
                currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);

            }
        }
    }

    public void startMove() {
        start = true;
    }
}
