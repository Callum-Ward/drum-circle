using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class RhythmSpawner : MonoBehaviour

{
    public GameObject leftTargetForeground;
    public GameObject leftTargetBase;
    public GameObject rightTargetForeground;
    public GameObject rightTargetBase;
    public GameObject beatTrack;
    GameObject leftBeatParent;  
    GameObject rightBeatParent;
    public GameObject beat;
    private Vector3 startSpawn;
    private AudioAnalyser audioAnalyser;
    private BeatManager beatManager;
    private BeatmapScript beatmapScript;
    private const float spawnScale = 1.25f;

    private const int beatmapWidth = 5;
    private const int midiGridOffset = 10;
    public float windowtime = 0.3f;
    public float window = 0f;
    public float delay = 2.0f;
    private long ticks = 123;
    
    private int prevTimeInMillis = 0;
    private int[] prevTimesInMillis;
    private int[] prevTimesInIndices;

    private Color[] trackColors = {new Color(0f,0f,0f), new Color(0.27f,0.08f,0.38f), new Color(0.04f,0.21f,0.10f)};
    private Color colorFreestyleMode = new Color(0.5f, 0.5f, 0.5f);

    private int playerCount;

    private GameObject enaTree;

    private GameObject[] targetAreas;
    private BeatTransfer beatTransfer;
    private bool soloFlag;

    // Start is called before the first frame update
    void Start()
    {
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatmapScript = GameObject.Find("RhythmLogic").GetComponent<BeatmapScript>();

        window = beatmapScript.window;
        windowtime = beatmapScript.windowtime;
        delay = beatmapScript.delay;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Sets the total number of players from main scene script
    public void setPlayerCount(int playerCount)
    {
        this.playerCount = playerCount;

        this.prevTimesInMillis = new int[this.playerCount];
        this.prevTimesInIndices = new int[this.playerCount];
        for(int i = 0; i < this.playerCount; i++)
        {
            this.prevTimesInMillis[i] = 0;
            this.prevTimesInIndices[i] = 0;
        }
    }

    public void setFreestyleMode(string mode, BeatTransfer? newBeatTransfer, int soloistIndex)
    {
        if(mode != "transfer" && mode != "solo" && mode != "none")
        {
            return;
        }

        int index = 0;

        if(mode == "transfer")
        {
            if(newBeatTransfer != null)
            {
                index = newBeatTransfer.getProvider();
            } 
            else {
                this.beatTransfer.getProvider();
            }

            this.soloFlag = false;
            this.beatTransfer = newBeatTransfer;
        }
        else if(mode == "solo")
        {
            index = soloistIndex;
            this.soloFlag = true;
            this.beatTransfer = null;
        }
        else
        {
            this.soloFlag = false;
            this.beatTransfer = null;
        }
    }

    public void spawn(int pos, int left, int size, int oneShotIndex = 0, string type = "falling")
    {
        //pos: 1,2,3,4,5
        //if single player central position will be used to spawn 3
        //if two players then 2 and 4 
        //if three players then 1,3,5 used
        //left: each player has 2 beat lines, if 1 then left beat spawn else right
   
        if (left==1)
        {
            GameObject newBeat = Instantiate(beat);
            newBeat.GetComponent<MoveBeatUI>().Startup(true, 2 * (pos - 1), size, type);

            if(type == "falling")
            {
                beatManager.AddToQueue(2 * (pos - 1), newBeat, oneShotIndex);
            }
        }
        else
        {
            GameObject newBeat = Instantiate(beat);
            newBeat.GetComponent<MoveBeatUI>().Startup(false, 2 * (pos - 1) + 1, size, type);

            if(type == "falling")
            {
                beatManager.AddToQueue(2 * (pos - 1) + 1, newBeat, oneShotIndex);
            }
        }
    }

    public IEnumerator spawnWithDelayCoroutine(int pos, int left, int oneShotIndex, int size, float delay)
    {
        yield return new WaitForSeconds(delay);
        spawn(pos, left, size, oneShotIndex);
    }

    public void spawnWithDelay(int pos, int left, int oneShotIndex, int size, float delay)
    {
        StartCoroutine(spawnWithDelayCoroutine(pos, left, oneShotIndex, size, delay));
    }

    private int spawnFromMidi(int timeInMills, int playerIndex)
    {
        int lb = timeInMills > prevTimesInMillis[playerIndex] + midiGridOffset ? timeInMills - midiGridOffset : prevTimesInMillis[playerIndex] + 1;
        int ub =  timeInMills + midiGridOffset;

        for(int j = lb; j <= ub; j++)
        {
            TimestampedNote? note = audioAnalyser.playerMidis[playerIndex].timestampedNotes[j];
            if(note != null)
            {
                spawn(playerIndex + 1, note.left, note.noteSize);
                prevTimesInMillis[playerIndex] = j + midiGridOffset / 2;
                audioAnalyser.playerMidis[playerIndex].timestampedNotes[j] = null;
                return (playerIndex + 1) - note.left;
            }
        }

        return -1;
    }

    private int spawnFromJson(int index, int playerIndex)
    {
        List<AudioTimestamp> timestampedOnsets = audioAnalyser.playerJson[playerIndex].timestampedOnsets;

        int lb = index < beatmapWidth ? 0 : (index - beatmapWidth);
        int ub =  index >= (timestampedOnsets.Count - beatmapWidth) ? (timestampedOnsets.Count - 1) : (index + beatmapWidth);

        for(int i = lb; i <= ub; i++)
        {
            int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;    
            if(timestampedOnsets[i].isBeat)
            {
                StartCoroutine(WindowDelay(delay - windowtime/2));
                spawn(playerIndex + 1, 1, size);
                timestampedOnsets[i].isBeat = false;
                this.prevTimesInIndices[playerIndex] = i;
                return playerIndex;
            }
            if(timestampedOnsets[i].isOnset)
            {
                StartCoroutine(WindowDelay(delay - windowtime/2));
                spawn(playerIndex + 1, 0, size);
                timestampedOnsets[i].isOnset = false;
                this.prevTimesInIndices[playerIndex] = i;
                return playerIndex + 1;
            }
        }
        return -1;
    }

    public int spawnOnTime(float time, bool useMidi = false)
    {
        if(this.soloFlag)
        {
            return - 1;
        }

        int index = (int)(Math.Round(time, 2) * 100);
        int timeInMills = (int)Math.Ceiling(time * 1000);
        
        for(int i = 0; i < this.playerCount; i++)
        {
            if(this.beatTransfer != null)
            {
                if(this.beatTransfer.nextPlayer(this.playerCount) != i)
                {
                    continue;
                }
            }

            if(useMidi)
            {
                if(timeInMills <= prevTimesInMillis[i])
                {
                    continue;
                }

                int queueIndex = spawnFromMidi(timeInMills, i);
                if(queueIndex >= 0)
                {
                    return queueIndex;
                }
            }
             else
            {
                if(index <= prevTimesInIndices[i])
                {
                    continue;
                }
                
                int queueIndex = spawnFromJson(index, i);
                if(queueIndex >= 0)
                {
                    return queueIndex;
                }
            }
        }

        return -1;
    }

    //Coroutine function for delaying hit-window
    IEnumerator WindowDelay(float time)
    {
        yield return new WaitForSeconds(time);

        window = windowtime;
    }


    public void colorFadeTargetComponent(GameObject obj, Color color, float alpha)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        newMaterial.SetFloat("_Mode", 2f);
        newMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        newMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        newMaterial.SetInt("_ZWrite", 0);
        newMaterial.DisableKeyword("_ALPHATEST_ON");
        newMaterial.EnableKeyword("_ALPHABLEND_ON");
        newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        newMaterial.renderQueue = 3000;
        color.a = alpha;
        newMaterial.color = color;
        renderer.material = newMaterial;
    }
}