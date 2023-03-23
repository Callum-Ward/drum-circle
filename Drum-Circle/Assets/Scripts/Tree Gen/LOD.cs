using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LOD : MonoBehaviour
{
    float distance;

    Camera camera;

    public int lod;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        distance = Vector3.Distance(this.transform.position, camera.transform.position);

        lod = 0;

        if (distance > 10) lod = 1;

        if (distance > 20) lod = 2;

        if (distance > 30) lod = 3;

        if (distance > 60) lod = -1;
    }
}
