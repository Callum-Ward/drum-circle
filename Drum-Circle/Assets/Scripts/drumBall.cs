using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using UnityEngine;

public class drumBall : MonoBehaviour
{
    // Start is called before the first frame update
    //WINDOWS
    SerialPort data_stream = new SerialPort("COM3", 9600);
    //MAC
    //SerialPort data_stream = new SerialPort("/dev/cu.usbmodem141301", 9600);
    public string receivedString;
    public Rigidbody rb;
    public float speed=5;
    public string[] sections;

    public int streak = 0;
    public int streakCombo = 0;

    void Start() {
        try
        {
            data_stream.Open();
        }
        catch (System.Exception ex)
        {
            print(ex.ToString());
        }

    }



    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.velocity = new Vector3(0, 5f, 0);

            FindObjectOfType<AudioManager>().Play("layer1");
            FindObjectOfType<AudioManager>().Play("layer2");
            FindObjectOfType<AudioManager>().Volume("layer2", 0f);
            FindObjectOfType<AudioManager>().Play("layer3");
            FindObjectOfType<AudioManager>().Volume("layer3", 0f);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector3(0, 5f, 0);

            FindObjectOfType<AudioManager>().Play("drumTap");

            streakCombo++;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rb.velocity = new Vector3(0, 5f, 0);

            FindObjectOfType<AudioManager>().Play("tapFail");
            if (streak > 0)
                streak--;
            else
                streak = 0;
        }

        Layers();

        try
        {
            receivedString = data_stream.ReadLine();
            sections = receivedString.Split(":");
            if (sections[0] == "on")
            {
                rb.velocity = new Vector3(0, speed, 0);
            }
        }
        catch (System.Exception e)
        {
            print(e.ToString());

        }




        //if (receivedString != "")
        //{
        //Debug.Log(receivedString);
        //string[] datas = receivedString.Split(":");
        //rb.velocity = new Vector3(0, 5f, 0);
        //}



    }

    void Layers()
    {
        if (streakCombo == 5)
        {
            streak++;
            streakCombo = 0;
        }

        if (streak == 0)
            FindObjectOfType<AudioManager>().Volume("layer2", 0f);

        if (streak == 1)
            FindObjectOfType<AudioManager>().Volume("layer3", 0f);

        if (streak >= 1)
            FindObjectOfType<AudioManager>().FadeIn("layer2");

        if (streak >= 2)
            FindObjectOfType<AudioManager>().FadeIn("layer3");

        if (streak > 2)
            streak = 2;
     }
}
