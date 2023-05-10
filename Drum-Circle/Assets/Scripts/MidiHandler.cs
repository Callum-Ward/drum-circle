using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public class MidiHandler : MonoBehaviour {
    private int playerCount = 3;
    public float[] midiInputVelocities;
    private int noteNumberOffset = 21;


    //Configures MIDI listener for drums
    void Awake()
    {

        DontDestroyOnLoad(gameObject);

        midiInputVelocities = new float[this.playerCount*2];

        InputSystem.onDeviceChange += (device, change) =>
        {
            if (change != InputDeviceChange.Added) return;

            var midiDevice = device as Minis.MidiDevice;
            if (midiDevice == null) return;

            midiDevice.onWillNoteOn += (note, velocity) => {

                //Re-configures note alignment if new MIDI device is used
                int drumIndex = note.noteNumber - noteNumberOffset;
                if(drumIndex < 0 || drumIndex > 5)
                {
                    drumIndex = 0;
                    noteNumberOffset = (int)note.noteNumber;
                }

                midiInputVelocities[drumIndex] = velocity;
            };
        };
    }


    //Clears stored MIDI input velocities
    public void clearMidiInputVelocities(int index)
    {
        midiInputVelocities[index] = 0.0f;
    }

}