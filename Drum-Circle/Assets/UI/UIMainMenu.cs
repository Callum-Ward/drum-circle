using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class UIMainMenu : MonoBehaviour
{
    private VisualElement mainMenu;
    private MessageListener messageListener;
    private LoadScreen loadScreen;
    private int[] drumInputStrengths;
    private int playerCount = 3;
    public string[] sections;
    private int noteNumberOffset = 21;
    float startGameV = 0;
    bool fadeIn = false;
    float value = 0;

    private MidiHandler midiHandler;

    public void Awake() {
        mainMenu = GameObject.Find("UIMainMenu").GetComponent<UIDocument>().rootVisualElement;
        midiHandler = GameObject.Find("MidiHandler").GetComponent<MidiHandler>();
        loadScreen = GameObject.Find("LoadScreen").GetComponent<LoadScreen>();
        // messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();

        drumInputStrengths = new int[playerCount*2];
    }    

    // Start is called before the first frame update
    void Start()
    {
        drumInputStrengths = new int[playerCount*2];
    }

    private void OnButtonClick() {
        StartCoroutine(sceneSwitch());
    }

    public void Update() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            StartCoroutine(sceneSwitch());
        }

        for(int i = 0; i < 6; i++) {
            if(drumInputStrengths[i] > 0 || midiHandler.midiInputVelocities[i] > 0.0f) {
                midiHandler.clearMidiInputVelocities(i);
                StartCoroutine(sceneSwitch());
            }
        }

    }

    IEnumerator sceneSwitch() {
        loadScreen.LoadScreenFadeIn();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("2MissionSelect");
    }

    private void handleDrumInput()
    {        
        //Checks if there was an input in the data stream
        for(int i = 0; i < playerCount*2; i++)
        {
            drumInputStrengths[i] = 0;
        }
        
        // string message = messageListener.message;
        // if (message != null)
        // {
        //     sections = message.Split(":");
        //     //Debug.Log(message);
        //     if (sections[0] == "on")
        //     {
        //         drumInputStrengths[Int32.Parse(sections[1])] = Int32.Parse(sections[3]);
        //     }
        //     messageListener.message = null;
        // }
    }


}
