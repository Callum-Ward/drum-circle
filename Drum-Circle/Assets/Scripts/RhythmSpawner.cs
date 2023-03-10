using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RhythmSpawner : MonoBehaviour

{
    public GameObject leftTargetForeground;
    public GameObject leftTargetBase;
    public GameObject rightTargetForeground;
    public GameObject rightTargetBase;
    public GameObject beatTrack;
    public GameObject leftBeat;
    public GameObject rightBeat;
    private Vector3 startSpawn;
    private AudioAnalyser audioAnalyser;
    private BeatManager beatManager;
    private const float spawnScale = 1.25f;

    private const int beatmapWidth = 10;
    public float windowtime = 0.3f;
    public float window = 0f;
    public float delay = 2.0f;

    private Color[] trackColors = {new Color(0f,0f,0f), new Color(0.27f,0.08f,0.38f), new Color(0.04f,0.21f,0.10f)};
    private int playerCount;

    // Start is called before the first frame update
    void Start()
    {
        audioAnalyser = GameObject.Find("AudioAnalysis").GetComponent<AudioAnalyser>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        //set beat spawner location with respect to camera position
        transform.position = Camera.main.transform.position + new Vector3(0f,2.5f,4f);
        //transform.rotation = Camera.main.transform.rotation;
        //set left most spawn location with respect to spawner postion
        startSpawn = transform.position + new Vector3(-3.5f, 0.35f, 0f);

        for(int i = 0; i < playerCount; i++){
            GameObject newLeftTarget = Instantiate(leftTargetForeground, startSpawn + new Vector3((i * 2.5f), -4.25f, 0f), transform.rotation);
            GameObject newRightTarget = Instantiate(rightTargetForeground, startSpawn + new Vector3((i * 2.5f) + 1.24f, -4.25f, 0f), transform.rotation);
            newLeftTarget.transform.localScale *= spawnScale;
            newRightTarget.transform.localScale *= spawnScale;

            GameObject newLeftTargetBase = Instantiate(leftTargetBase, startSpawn + new Vector3((i * 2.5f) - 0.01f, -4.26f, 0.01f), transform.rotation);
            GameObject newRightTargetBase = Instantiate(rightTargetBase, startSpawn + new Vector3((i * 2.5f) + 1.24f, -4.26f, 0.01f), transform.rotation);
            GameObject newLeftTrack = Instantiate(beatTrack, startSpawn + new Vector3((i * 2.5f) - 0.01f, -4.26f, 0.01f), transform.rotation);
            GameObject newRightTrack = Instantiate(beatTrack, startSpawn + new Vector3((i * 2.5f) + 1.24f, -4.26f, 0.01f), transform.rotation);

            colorFadeTargetComponent(newLeftTargetBase, trackColors[i], 1.0f);
            colorFadeTargetComponent(newRightTargetBase, trackColors[i], 1.0f);
            colorFadeTargetComponent(newLeftTrack, trackColors[i], 0.7f);
            colorFadeTargetComponent(newRightTrack, trackColors[i], 0.7f);
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
    public void spawnOnTime(float time)
    {
            int index = (int)(Math.Round(time, 2) * 100);
            List<AudioTimestamp> timestampedOnsets = audioAnalyser.activeAnalysis.timestampedOnsets;
    
            if(index < timestampedOnsets.Count){
                int lb = index  < beatmapWidth ? 0 : index - beatmapWidth;
                int ub = index  >= timestampedOnsets.Count - beatmapWidth ? timestampedOnsets.Count - 1 : index + beatmapWidth;
                for(int i = lb; i <= ub; i++)
                {
                    if(timestampedOnsets[i].isBeat)
                    {
                        int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;
                        StartCoroutine(WindowDelay(delay - windowtime/2));
                        for(int j = 0; j < playerCount; j++){
                            spawn(j + 1, 1, size);
                        }
                        timestampedOnsets[i].isBeat = false;
                        break;
                    }
                    if(timestampedOnsets[i].isOnset)
                        {
                        int size = Convert.ToDouble(timestampedOnsets[i].strength) > 0.0 ? 2 : 1;    
                        StartCoroutine(WindowDelay(delay - windowtime/2));
                        for(int j = 0; j < playerCount; j++){
                            spawn(j + 1, 0, size);
                        }
                        timestampedOnsets[i].isOnset = false;
                        break;
                    }
                }
            } 
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