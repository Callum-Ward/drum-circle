using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Audio;

public class UISelectMenu : MonoBehaviour
{    
    private VisualElement selectMenu;
    private MessageListener messageListener;
    private LoadScreen loadScreen;
    private float mTimer = 0f;
    private int buttonSelection = 0;
    private int[] drumInputStrengths;
    private int playerCount = 3;
    public string[] sections;
    private int noteNumberOffset = 21;
    float audioTimer = 0;
    bool stopTrack = false;

    private Color originalColor;
    private Color highlightColor = Color.green;
    private Color confirmColor = Color.blue;

    VisualElement forestButton;
    VisualElement mountainButton;
    VisualElement beachButton;
    VisualElement[] buttons;

    private MidiHandler midiHandler;
    AudioManager audioManager;

    public void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
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
        originalColor = Color.white;
        forestButton.style.unityBackgroundImageTintColor = highlightColor;
        loadScreen = GameObject.Find("LoadScreen").GetComponent<LoadScreen>();
        loadScreen.LoadScreenFadeOut();
    }
    
    IEnumerator sceneSwitch(string mission) {
        loadScreen.LoadScreenFadeIn();
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(mission);
    }

    public void Update() {
        if (mTimer <= 0) {
            mTimer = 0f;
        }
        else {
            mTimer -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Alpha1)) {
            StartCoroutine(sceneSwitch("Forest"));
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            StartCoroutine(sceneSwitch("Mountains"));
        }
        if (Input.GetKey(KeyCode.Alpha3)) {
            StartCoroutine(sceneSwitch("Beach"));
        }

        if (stopTrack == false) {
            if(buttonSelection == 0) {
                var track = Array.Find(audioManager.drumTracks, sound => sound.Name == "Forest");
                if(audioTimer == 0f) {
                    audioManager.PlaySingle("Forest");
                    // audioManager.VolumeTrack(track, 0f);
                    audioManager.FadeInTrack(track, "slow", 0.5f);
                }
                else if(audioTimer >= 15f) {
                    audioTimer = 0f;
                    audioManager.StopSingle("Forest");
                }
                else if(audioManager.audioSource.time >= 13.5f) {
                    audioManager.FadeOutTrack(track, "slow", 0.5f);
                }
                if(audioTimer < 15f) {
                    audioTimer += Time.deltaTime;
                }
            }
            //Debug.Log("Source time: "+ audioTimer);
        }

        
        if ((drumInputStrengths[0] > 0 || midiHandler.midiInputVelocities[0] > 0.0f || Input.GetKeyDown(KeyCode.LeftArrow)) && mTimer == 0f)
        {
            midiHandler.clearMidiInputVelocities(0);
            buttons[buttonSelection].style.unityBackgroundImageTintColor = originalColor;
            if(buttonSelection == 2) {
                buttonSelection = 0;
            }
            else {
                buttonSelection += 1;
            }
            buttons[buttonSelection].style.unityBackgroundImageTintColor = highlightColor;
            Debug.Log("CurrentSelection: " + buttonSelection);
        }
        else if ((drumInputStrengths[1] > 0 || midiHandler.midiInputVelocities[1] > 0.0f || Input.GetKeyDown(KeyCode.RightArrow)) && mTimer == 0f)
        {
            midiHandler.clearMidiInputVelocities(1);
            buttons[buttonSelection].style.unityBackgroundImageTintColor = confirmColor;
                    if (buttonSelection == 0) {
                        StartCoroutine(sceneSwitch("Forest"));
                    }
                    if (buttonSelection == 1) {
                        StartCoroutine(sceneSwitch("Mountains"));
                    }
                    if (buttonSelection == 2) {
                        StartCoroutine(sceneSwitch("Beach"));
                    }
        }
        
        forestButton.RegisterCallback<ClickEvent>(evt => sceneSwitch("Forest"));
        mountainButton.RegisterCallback<ClickEvent>(evt => sceneSwitch("Mountains"));
        beachButton.RegisterCallback<ClickEvent>(evt => sceneSwitch("Beach"));
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
