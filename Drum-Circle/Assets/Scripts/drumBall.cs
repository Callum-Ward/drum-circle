using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using UnityEngine;

public class drumBall : MonoBehaviour
{
    // Start is called before the first frame update
    SerialPort data_stream = new SerialPort("COM3", 9600);
    public string receivedString;
    public Rigidbody rb;
    public string[] datas;
    void Start()
    {
        data_stream.Open();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.velocity = new Vector3(0, 5f, 0);
        }

        receivedString = data_stream.ReadLine();
        if (receivedString != "")
        {
            Debug.Log(receivedString);
            //string[] datas = receivedString.Split(":");
            //rb.velocity = new Vector3(0, 5f, 0);
        }
        
        
        
    }
}
