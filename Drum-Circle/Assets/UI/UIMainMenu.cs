using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public class UIMainMenu : MonoBehaviour
{
    private VisualElement mainMenu;
    private MessageListener messageListener;
    private int[] drumInputStrengths;
    private float[] midiInputVelocities;
    private int playerCount = 3;
    public string[] sections;
    private int noteNumberOffset = 44;

    public void Awake() {
            mainMenu = GameObject.Find("UIMainMenu").GetComponent<UIDocument>().rootVisualElement;
            messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
    }

    public void startGame() {
        SceneManager.LoadScene("2MissionSelect");
    }

    private void OnButtonClick() {
        startGame();
    }

    public void Update() {
        VisualElement startButton = mainMenu.Q<VisualElement>("StartButton");
        if (Input.GetKey(KeyCode.Alpha1)) {
            startGame();
        }

        for(int i = 0; i < 6; i++) {
            if(drumInputStrengths[i] > 0 || midiInputVelocities[i] > 0.0f) {
                startGame();
            }
        }

        startButton.RegisterCallback<ClickEvent>(evt => OnButtonClick());
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
                /*  Debug.Log(string.Format(
                    "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    velocity,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                )); */

                midiInputVelocities[note.noteNumber - noteNumberOffset] = velocity;
            };

            midiDevice.onWillNoteOff += (note) => {
                /*Debug.Log(string.Format(
                    "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));

                midiInputVelocities[note.noteNumber - noteNumberOffset] = -midiInputVelocities[note.noteNumber - noteNumberOffset];*/
            };
        };
    }
}
