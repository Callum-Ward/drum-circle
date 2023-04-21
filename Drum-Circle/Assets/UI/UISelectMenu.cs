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
    private float[] midiInputVelocities;
    private int playerCount = 3;
    public string[] sections;
    private int noteNumberOffset = 44;

    private Color originalColor;
    private Color highlightColor = Color.white;
    private Color confirmColor = Color.green;

    VisualElement forestButton;
    VisualElement mountainButton;
    VisualElement beachButton;
    VisualElement[] buttons;

    public void Awake() {
        selectMenu = GameObject.Find("UIMissionSelect").GetComponent<UIDocument>().rootVisualElement;
        messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
        drumInputStrengths = new int[playerCount*2];
        midiInputVelocities = new float[playerCount*2];
    }

    void Start() {
        addMidiHandler();
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

        
        if ((drumInputStrengths[0] > 0 || midiInputVelocities[0] > 0.0f || Input.GetKeyDown(KeyCode.LeftArrow)) && mTimer == 0f)
        {
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
        else if ((drumInputStrengths[1] > 0 || midiInputVelocities[1] > 0.0f || Input.GetKeyDown(KeyCode.RightArrow)) && mTimer == 0f)
        {
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

    private void addMidiHandler()
    {
        InputSystem.onDeviceChange += (device, change) =>
        {
            if (change != InputDeviceChange.Added) return;

            var midiDevice = device as Minis.MidiDevice;
            if (midiDevice == null) return;

            midiDevice.onWillNoteOn += (note, velocity) => {
                // Note that you can't use note.velocity because the state
                // hasn't been updated yet (as this is "will" event). The note
                // object is only useful to specify the target note (note
                // number, channel number, device name, etc.) Use the velocity
                // argument as an input note velocity.
                  Debug.Log(string.Format(
                    "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    velocity,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                )); 

                midiInputVelocities[note.noteNumber - noteNumberOffset] = velocity;
            };

            midiDevice.onWillNoteOff += (note) => {
               /* Debug.Log(string.Format(
                    "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));*/

                midiInputVelocities[note.noteNumber - noteNumberOffset] = -midiInputVelocities[note.noteNumber - noteNumberOffset];
            };
        };
    }
}
