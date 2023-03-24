using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0,2f)]
    [SerializeField] private float waypointSize = 1f; //adjust size of waypoint spheres
    [SerializeField] private bool loop = false;

    private void OnDrawGizmos()
    {
        foreach (Transform t in transform) //loop through all waypoint child objects 
        {
            Gizmos.color = Color.blue; //colour of waypoint spheres
            Gizmos.DrawWireSphere(t.position, waypointSize); //draw waypoint sphere in scene viewer only, doesn't translate into game view
        }
        Gizmos.color = Color.yellow; //colour of lines connecting waypoints
        for (int i=0;i<transform.childCount-1;i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position); //loop through children in order and draw lines between
        }
       
    }
    public Transform GetNextWaypoint(Transform currentWaypoint) //objects send their current waypoint and function returns the next in the sequence
    {   
        if (currentWaypoint == null)
        {
            return transform.GetChild(0); //if object doesn't have a current waypoint then assign it to the starting waypoint in sequence
        }
        if (currentWaypoint.GetSiblingIndex() < transform.childCount - 1)
        {
            return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
        } else if (loop)
        {
            return transform.GetChild(0);
        }
        else return null;
        
    }
}
