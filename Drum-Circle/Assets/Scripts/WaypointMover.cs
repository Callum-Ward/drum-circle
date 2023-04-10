using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints; //refernece to waypoints list
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float switchDistance = 0.1f;

    private Transform currentWaypoint;
    private Transform nextWaypoint;

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

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position,currentWaypoint.position) < switchDistance)
        {
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        
            //nextWaypoint = waypoints.GetNextWaypoint(nextWaypoint);
        }
        
    }
}
