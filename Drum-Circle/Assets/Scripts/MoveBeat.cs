using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBeat : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 1;
    public float removeHeight = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        if (transform.position.y < removeHeight)
        {
            Destroy(gameObject);
        }
    }

}
