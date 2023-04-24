using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LOD : MonoBehaviour
{
    [SerializeField] float lod_0 = 10;
    [SerializeField] float lod_1 = 20;
    [SerializeField] float lod_2 = 30;
    [SerializeField] float lod_3 = 60;

    public int lod;

    float distance;
    Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        distance = Vector3.Distance(this.transform.position, camera.transform.position);

        lod = 0;

        if (distance > lod_0) lod = 1;

        if (distance > lod_1) lod = 2;

        if (distance > lod_2) lod = 3;

        if (distance > lod_3) lod = -1;
    }
}
