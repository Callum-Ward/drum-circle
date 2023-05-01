using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public class UISelectMenu : MonoBehaviour
{    
    private VisualElement selectMenu;
    private MessageListener messageListener;
    private float mTimer = 0f;
    private int buttonSelection = 0;
    private int[] drumInputStrengths;
    private int playerCount = 3;
    public string[] sections;
    private int noteNumberOffset = 21;

    private Color originalColor;
    private Color highlightColor = Color.white;
    private Color confirmColor = Color.green;

    VisualElement forestButton;
    VisualElement mountainButton;
    VisualElement beachButton;
    VisualElement[] buttons;

    private MidiHandler midiHandler;

    public void Awake() {
        selectMenu = GameObject.Find("UIMissionSelect").GetComponent<UIDocument>().rootVisualElement;
        // messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
        midiHandler = GameObject.Find("MidiHandler").GetComponent<MidiHandler>();
        drumInputStrengths = new int[playerCount*2];
    }

    void Start() {
        forestButton = selectMenu.Q<VisualElement>("ForestButton");
        mountainButton = selectMenu.Q<VisualElement>("MountainsButton");
        beachButton = selectMenu.Q<VisualElement>("BeachButton");
        buttons = new VisualElement[] {forestButton, mountainButton, beachButton};
        // originalColor = forestButton.resolvedStyle.backgroundColor;
        originalColor = Color.grey;
        forestButton.style.backgroundColor = highlightColor;
    }

    public void missionChoice(string mission) {
        SceneManager.LoadScene(mission);
    }

    public void Update() {
    
        //handleDrumInput();


        if (mTimer <= 0) {
            mTimer = 0f;
        }
        else {
            mTimer -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Alpha1)) {
            missionChoice("Forest");
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            missionChoice("Mountains");
        }
        if (Input.GetKey(KeyCode.Alpha3)) {
            missionChoice("Beach");
        }

        
        if ((drumInputStrengths[0] > 0 || midiHandler.midiInputVelocities[0] > 0.0f || Input.GetKeyDown(KeyCode.LeftArrow)) && mTimer == 0f)
        {
            midiHandler.clearMidiInputVelocities(0);
            buttons[buttonSelection].style.backgroundColor = originalColor;
            if(buttonSelection == 2) {
                buttonSelection = 0;
            }
            else {
                buttonSelection += 1;
            }
            buttons[buttonSelection].style.backgroundColor = highlightColor;
            Debug.Log("CurrentSelection: " + buttonSelection);
        }
        else if ((drumInputStrengths[1] > 0 || midiHandler.midiInputVelocities[1] > 0.0f || Input.GetKeyDown(KeyCode.RightArrow)) && mTimer == 0f)
        {
            midiHandler.clearMidiInputVelocities(1);
            buttons[buttonSelection].style.backgroundColor = confirmColor;
                    if (buttonSelection == 0) {
                        missionChoice("Forest");
                    }
                    if (buttonSelection == 1) {
                        missionChoice("Mountains");
                    }
                    if (buttonSelection == 2) {
                        missionChoice("Beach");
                    }
        }
        
        forestButton.RegisterCallback<ClickEvent>(evt => missionChoice("Forest"));
        mountainButton.RegisterCallback<ClickEvent>(evt => missionChoice("Mountains"));
        beachButton.RegisterCallback<ClickEvent>(evt => missionChoice("Beach"));
    }

    private void handleDrumInput()
    {
        
        //Checks if there was an input in the data stream
        for(int i = 0; i < playerCount*2; i++)
        {
            drumInputStrengths[i] = 0;
        }
        
        string message = messageListener.message;
        if (message != null)
        {
            sections = message.Split(":");
            //Debug.Log(message);
            if (sections[0] == "on")
            {
                drumInputStrengths[Int32.Parse(sections[1])] = Int32.Parse(sections[3]);
            }
            messageListener.message = null;
        }
    }
}
