using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public class BeatmapScript : MonoBehaviour
{
    public RhythmSpawner spawner;
    public float timer = 0.0f;
    public float window = 0f;
    public float windowtime = 0.3f;
    public float delay = 2.0f;
    public float inputDelay = 0f;
    public float beatTargetLocation = 0.3f;
    private int noteNumberOffset = 44;
    private int[] drumInputStrengths;
    private float[] midiInputVelocities;
    private bool hitL = false;
    private bool hitR = false;

    public int terrainBeatStage = 1;
    public float glowPower = 5.0f;
    private float glowRate = 1.1f;
    private int treeStage = 0;
    private int treeScoreRatio = 1500;

    private float windIncreaseRate = 0.05f;
    private float windDecreaseRate = 0.001f;
    public float windFactor = 0.1f;

    private BeatTransfer beatTransfer;

    public ScoreManager scoreManager;
    public AudioAnalyser audioAnalyser;
    public AudioManager audioManager;
    public BeatManager beatManager;
    public RhythmSpawner beatSpawner;
    public TreeSpawnerForest treeSpawner;
    public MessageListener messageListener;
    public Terrain terrain;
    public TutorialScript tutorialScript;
    public BeatUI beatUI;
    public TreeManager treeManager;
    public string[] sections;
    public string receivedString;
    private const int beatmapWidth = 10;

    public int playerCount = 3;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatSpawner = GameObject.Find("BeatSpawner").GetComponent<RhythmSpawner>();
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        treeSpawner = GameObject.Find("TreeSpawner").GetComponent<TreeSpawnerForest>();
        messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
        tutorialScript = GameObject.Find("TutorialLogic").GetComponent<TutorialScript>();
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>();
        // treeManager = GameObject.Find("Tree Manager").GetComponent<TreeManager>();

        beatManager.setPlayerCount(this.playerCount);
        beatSpawner.setPlayerCount(this.playerCount);
        beatUI.setPlayerCount(this.playerCount);
        treeSpawner.setPlayers(this.playerCount);

        drumInputStrengths = new int[this.playerCount*2];
        midiInputVelocities = new float[this.playerCount*2];

        terrain.terrainData.wavingGrassSpeed = 0.5f;
        terrain.terrainData.wavingGrassStrength = 0.5f;
        terrain.terrainData.wavingGrassAmount = 0.5f;

        beatTransfer = new BeatTransfer(0, 1);
        
    }

    private void registerHit(int queueIndex, MoveBeatUI beat)
    {
        scoreManager.Hit((windowtime / 2) - Mathf.Abs((windowtime / 2) - beat.windowScore));
        beatManager.BeatDelete(queueIndex, true);
        audioManager.FadeIn("drums", "fast");
        beat.dontDelete = true;
        treeManager.SetHitStatus(true);
    }

    private void registerMiss(int queueIndex, MoveBeatUI beat)
    {
        scoreManager.Miss();
        audioManager.Play("tapFail", null);
        audioManager.SetActive("drums");
        audioManager.Volume("drums", 0f);
        if (beat.timer >= (delay * 0.85))
        {
            beatManager.BeatDelete(queueIndex, false);
        }
        treeManager.SetHitStatus(false);
    }

    //Coroutine function for delaying hit-window
    IEnumerator WindowDelay(float time)
    {
        yield return new WaitForSeconds(time);

        window = windowtime;
    }

    private void setEnvironmentTriggers(int drumIndex)
    {
        if(drumIndex % 2 == 0 && terrainBeatStage <= 1)
        {   
            terrainBeatStage += 1;
        }
                            
        if(treeStage == 0)
        {
            treeSpawner.spawnTreeAtLocation(1, new Vector2(293, 38), true);
            treeStage += 1;
        }
        else if(Math.Floor(scoreManager.Score / treeScoreRatio) >= treeStage)
        {
            treeStage += 1;
            treeSpawner.spawnTree(drumIndex + 1, 2);
        }
    }

    private void handleTerrainBeatResponse()
    {
         /*GameObject enaTree = GameObject.Find("tree_afsTREE_xao_xlprl");
            MeshRenderer renderer = enaTree.GetComponent<MeshRenderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.c
            renderer.SetPropertyBlock(block);*/
        try{
            MeshRenderer renderer;

            //Material newMaterial = new Material(Shader.Find("Shader Graphs/glowing shader"));
            //newMaterial.SetFloat("_Power", glowPower);

           /* IEnumerable<GameObject> glowingLayers = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "branch");
            foreach(GameObject obj in glowingLayers)
            {
                renderer = obj.GetComponent<MeshRenderer>();
                renderer.material.SetFloat("_Power", glowPower);
            }*/

            IEnumerable<GameObject> glowingLayers = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "glowingLayer");
            foreach(GameObject obj in glowingLayers)
            {
                renderer = obj.GetComponent<MeshRenderer>();
                renderer.material.SetFloat("_Power", glowPower);
            }
        } catch {

        }

        terrain.terrainData.wavingGrassStrength = windFactor;
        terrain.terrainData.wavingGrassSpeed = windFactor;
        //terrain.terrainData.wavingGrassAmount = windFactor;
        
        if(terrainBeatStage == 3)
        {
            glowPower += glowRate * 1.5f;
            windFactor -= windDecreaseRate;
            if(windFactor <= 0.1f){
                terrainBeatStage = 1;
            }
        }
        else if(terrainBeatStage == 2)
        {
            windFactor += windIncreaseRate;
            if(windFactor >= 0.8f){
                terrainBeatStage += 1;
            }
        }
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
                /* Debug.Log(string.Format(
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

    private bool checkCorrectDrumHit(int drumIndex)
    {
        if (beatManager.beatQueues[drumIndex].Count > 0) 
        {
            try{
                var beatL = beatManager.beatQueues[drumIndex].Peek().GetComponent<MoveBeatUI>();
                beatHit((drumIndex), beatL);
                return true;
            } catch {
            }
        }
        return false;
    }

    private void checkDrumHit()
    {
        for(int i = 0; i < playerCount; i++)
            {
                //Register left drum hit and perform code
                if ((drumInputStrengths[i*2] > 0 || midiInputVelocities[i*2] > 0.0f || Input.GetKeyDown(KeyCode.LeftArrow)))
                {
                    if (checkCorrectDrumHit(i*2))
                    {
                        setEnvironmentTriggers(i*2);
                    }
                    if (beatTransfer != null)
                    {
                        StartCoroutine(beatTransfer.TransferWithDelay(beatSpawner, i, 0, 0.0f, 1.0f));
                    }
                    midiInputVelocities[i * 2] = 0.0f;
                }

                //Register right drum hit and perform code
                if ((drumInputStrengths[i*2 + 1] > 0 || midiInputVelocities[i*2 + 1] > 0.0f || Input.GetKeyDown(KeyCode.RightArrow)))
                {
                    if (checkCorrectDrumHit(i*2 + 1))
                    {
                        // Enviroment triggers etc. right drum hit on target
                    }
                    if (beatTransfer != null)
                    {
                        StartCoroutine(beatTransfer.TransferWithDelay(beatSpawner, i, 1, 0.0f, 1.0f));
                    }
                    midiInputVelocities[i * 2 + 1] = 0.0f;
                }
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<RhythmSpawner>();
        addMidiHandler();
    }

    // Update is called once per frame
    void Update()
    {
        //13
        handleTerrainBeatResponse();
        handleDrumInput();

        //Start the timer
        if (timer <= delay && audioManager.activeSource == null && tutorialScript.tutorialComplete == true)
        {
            Debug.Log("timer starts");
            beatSpawner.spawnOnTime(timer);
            timer += Time.deltaTime;
            beatUI.startLevelUI();
        }
        //Play all layers of music simultaneously
        else if(audioManager.activeSource == null && tutorialScript.tutorialComplete == true)
        {
            audioManager.Play("drums", audioAnalyser);
            audioManager.Play("drums2", null);
            audioManager.Play("drums3", null);
            audioManager.Play("layer1", null);
            audioManager.Play("layer2", null);
            audioManager.Volume("layer2", 1f);
            audioManager.Play("layer3", null);
            audioManager.Volume("layer3", 1f);
            audioManager.Play("layer4", null);
            audioManager.Volume("layer4", 0.75f);
            Debug.Log("Activating all layers");
        }
        //Drum hit functionality
        else if(tutorialScript.tutorialComplete == true)
        {
                bool hasSpawned = beatSpawner.spawnOnTime(audioManager.activeSource.time + delay + inputDelay, true);

                if(hasSpawned && beatTransfer != null)
                {
                    beatTransfer = null;
                }

            checkDrumHit();
        }

//////////////TESTING SECTION////////////////////////////////
        if(Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift)) {
            audioManager.Volume("drums", 0f);
        };
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            audioManager.Volume("drums", 1f);
        };
        if(Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftShift)) {
            audioManager.Volume("drums2", 0f);
        };
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            audioManager.Volume("drums2", 1f);
        };
        if(Input.GetKey(KeyCode.Alpha3) && Input.GetKey(KeyCode.LeftShift)) {
            audioManager.Volume("drums3", 0f);
        };
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            audioManager.Volume("drums3", 1f);
        };
        if(Input.GetKey(KeyCode.Alpha4) && Input.GetKey(KeyCode.LeftShift)) {
            audioManager.Volume("layer1", 0f);
        };
        if(Input.GetKeyDown(KeyCode.Alpha4)) {
            audioManager.Volume("layer1", 1f);
        };
        if(Input.GetKey(KeyCode.Alpha5) && Input.GetKey(KeyCode.LeftShift)) {
            audioManager.Volume("layer2", 0f);
        };
        if(Input.GetKeyDown(KeyCode.Alpha5)) {
            audioManager.Volume("layer2", 1f);
        };
        if(Input.GetKey(KeyCode.Alpha6) && Input.GetKey(KeyCode.LeftShift)) {
            audioManager.Volume("layer3", 0f);
        };
        if(Input.GetKeyDown(KeyCode.Alpha6)) {
            audioManager.Volume("layer3", 1f);
        };
        if(Input.GetKey(KeyCode.Alpha7) && Input.GetKey(KeyCode.LeftShift)) {
            audioManager.Volume("layer4", 0f);
        };
        if(Input.GetKeyDown(KeyCode.Alpha7)) {
            audioManager.Volume("layer4", 0.6f);
        };
//////////////TESTING SECTION////////////////////////////////

    }

    void beatHit(int queueNo, MoveBeatUI beatSide) {
        if (beatSide.window == true)
        {
            registerHit(queueNo, beatSide);
        }
        else if (beatTransfer != null)
        {
            if(beatTransfer.getProvider() != queueNo / 2)
            {
                registerMiss(queueNo, beatSide);
            }
        }
        else
        {
            registerMiss(queueNo, beatSide);
        }
    }
}

