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

    public void clearMidiInputVelocities(int index)
    {
        midiInputVelocities[index] = 0.0f;
    }

}