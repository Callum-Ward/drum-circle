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
    public GameObject leftBeat;
    public GameObject rightBeat;
    private Vector3 startSpawn;
    private AudioAnalyser audioAnalyser;
    private BeatManager beatManager;
    private const float spawnScale = 1.25f;

    private const int beatmapWidth = 10;
    private const int midiGridOffset = 10;
    public float windowtime = 0.3f;
    public float window = 0f;
    public float delay = 2.0f;
    private long ticks = 123;
    private int prevTimeInMillis = 0;

    private Color[] trackColors = {new Color(0f,0f,0f), new Color(0.27f,0.08f,0.38f), new Color(0.04f,0.21f,0.10f)};
    private int playerCount;

    private GameObject enaTree;

    // Start is called before the first frame update
    void Start()
    {
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();

        enaTree = GameObject.Find("tree_afsTREE_xao_xlprl");
        //set beat spawner location with respect to camera position
        transform.position = Camera.main.transform.position + new Vector3(0f,2.5f,4f);
        //transform.rotation = Camera.main.transform.rotation;
        //set left most spawn location with respect to spawner postion
        startSpawn = transform.position + new Vector3(-3.5f, 0.35f, 0f);

        for(int i = 0; i < playerCount; i++){
            GameObject newLeftTarget = Instantiate(leftTargetForeground, startSpawn + new Vector3((i * 2.5f) + 0.25f, -4.25f, 0f), transform.rotation);
            GameObject newRightTarget = Instantiate(rightTargetForeground, startSpawn + new Vector3((i * 2.5f) + 1.24f, -4.25f, 0f), transform.rotation);
            
            newLeftTarget.transform.localScale *= spawnScale;
            newRightTarget.transform.localScale *= spawnScale;


            GameObject newLeftTargetBase = Instantiate(leftTargetBase, startSpawn + new Vector3((i * 2.5f) + 0.24f, -4.26f, 0.01f), transform.rotation);
            GameObject newRightTargetBase = Instantiate(rightTargetBase, startSpawn + new Vector3((i * 2.5f) + 1.24f, -4.26f, 0.01f), transform.rotation);
            //GameObject newLeftTrack = Instantiate(beatTrack, startSpawn + new Vector3((i * 2.5f) - 0.01f, -4.26f, 0.01f), transform.rotation);
            //GameObject newRightTrack = Instantiate(beatTrack, startSpawn + new Vector3((i * 2.5f) + 1.24f, -4.26f, 0.01f), transform.rotation);

            colorFadeTargetComponent(newLeftTargetBase, trackColors[i], 1.0f);
            colorFadeTargetComponent(newRightTargetBase, trackColors[i], 1.0f);
            //colorFadeTargetComponent(newLeftTrack, trackColors[i], 0.4f);
            //colorFadeTargetComponent(newRightTrack, trackColors[i], 0.4f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Sets the total number of players from main scene script
    public void setPlayerCount(int playerCount)
    {
        this.playerCount = playerCount;
    }

    public void spawn(int pos, int left, int size)
    {
        //pos: 1,2,3,4,5
        //if single player central position will be used to spawn 3
        //if two players then 2 and 4 
        //if three players then 1,3,5 used
        //left: each player has 2 beat lines, if 1 then left beat spawn else right
        Vector3 spawnLoc = startSpawn + new Vector3(((pos-1)*1.25f) + (pos*1.25f)-(left*1.25f), 0f, 0f);
        Vector3 spawnScaleAddition = new Vector3((size - 1)*0.1f, (size-1)*0.1f, 0f);
   
        if (left==1)
        {
            spawnLoc = spawnLoc + new Vector3(0.25f, 0f, 0f);
            GameObject newBeat = Instantiate(leftBeat, spawnLoc, transform.rotation);
            newBeat.transform.localScale = spawnScale * newBeat.transform.localScale + spawnScaleAddition;
            beatManager.AddToQueue(2 * (pos - 1), newBeat);
        }
        else
        {
            GameObject newBeat = Instantiate(rightBeat, spawnLoc, transform.rotation);
            newBeat.transform.localScale = spawnScale * newBeat.transform.localScale + spawnScaleAddition;
            beatManager.AddToQueue(2 * (pos - 1) + 1, newBeat);
        }


    }


        //Function for spawning beats based on passed variable
    public bool spawnOnTime(float time, bool useMidi = false)
    {
            int index = (int)(Math.Round(time, 2) * 100);
            List<AudioTimestamp> timestampedOnsets = audioAnalyser.activeAnalysis.timestampedOnsets;

            int timeInMills = (int)Math.Ceiling(time * 1000);
            if(useMidi && timeInMills <= prevTimeInMillis)
            {
                return false;
            }
    
            if(index < timestampedOnsets.Count){
                int lb = useMidi ? (timeInMills > prevTimeInMillis + midiGridOffset ? timeInMills - midiGridOffset : prevTimeInMillis + 1) : (index  < beatmapWidth ? 0 : index - beatmapWidth);
                int ub =  useMidi ? (timeInMills + midiGridOffset) : (index  >= timestampedOnsets.Count - beatmapWidth ? timestampedOnsets.Count - 1 : index + beatmapWidth);
                for(int i = lb; i <= ub; i++)
                {
                    if(useMidi)
                    {
                        MetricTimeSpan metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(ticks, audioAnalyser.activeMidi.tempoMap);
                        IEnumerable<Note> notes = audioAnalyser.activeMidi.midiFile.GetNotes().AtTime(new MetricTimeSpan(0, 0, 0, i), audioAnalyser.activeMidi.tempoMap);
                        Note? note = notes.FirstOrDefault();
                        if(note != null)
                        {
                            Debug.Log("NOTARY " + note.NoteNumber);
                            for(int j = 0; j < playerCount; j++){
                                spawn(j + 1, note.NoteNumber == 44 ? 1 : 0, 1);
                            }
                            prevTimeInMillis = i + midiGridOffset / 2;
                            return true;
                        }
                    }
                    else
                    {
                        if(timestampedOnsets[i].isBeat)
                        {
                            int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;
                            StartCoroutine(WindowDelay(delay - windowtime/2));
                            for(int j = 0; j < playerCount; j++){
                                spawn(j + 1, 1, size);
                            }
                            timestampedOnsets[i].isBeat = false;
                            return true;
                        }
                        if(timestampedOnsets[i].isOnset)
                            {
                            int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;    
                            StartCoroutine(WindowDelay(delay - windowtime/2));
                            for(int j = 0; j < playerCount; j++){
                                spawn(j + 1, 0, size);
                            }
                            timestampedOnsets[i].isOnset = false;
                            return true;
                        }
                    }
                }
            } 
            return false;
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