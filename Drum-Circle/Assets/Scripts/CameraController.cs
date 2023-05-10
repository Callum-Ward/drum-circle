using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject platform;
    GameObject[] cameras;

    // Start is called before the first frame update
    void Start()
    {        
        platform = GameObject.Find("Platform_Skull_03");
        var CameraM = GameObject.Find("MainCamera");
        var CameraL = GameObject.Find("Camera2");
        var CameraR = GameObject.Find("Camera3");
        cameras = new GameObject[] {CameraM, CameraL, CameraR};

        for (int i = 0; i < 3; i++) {
            cameras[i].transform.SetParent(platform.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
