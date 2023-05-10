using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class printuv : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log(mesh.vertices);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
