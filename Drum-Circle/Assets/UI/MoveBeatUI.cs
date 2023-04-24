using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.Experimental;

public class MoveBeatUI : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float timer = 0f;
    private float windowtime = 0f;
    public bool window = false;
    public bool dontDelete = false;
    public bool delete = false;
    private float deleteTime;
    public float windowScore = 0f;
    private float alpha = 1.0f;
    public bool fade = false;
    public bool highlight = false;
    public bool test = false;
    private float beatHeight;
    private float screenHeight;
    private bool start = false;
    private float beatTargetLocation;

    public ScoreManager scoreManager;
    public BeatManager beatManager;
    public BeatmapScript beatmapScript;
    public AudioManager audioManager;

    VisualElement beatUI;
    UIDocument beatSpawnUI;
    MeshRenderer beatRenderer;
    public TemplateContainer beatSpawnContainer;
    public VisualTreeAsset beatSpawnTemplate;
    public VisualElement container;
    VisualElement element;
    
    public VisualElement Lane1L, Lane1R, Lane2L, Lane2R, Lane3L, Lane3R;
    public VisualElement[] Lanes;

    private Vector3 baseScale;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatmapScript = GameObject.Find("Rhythm Logic").GetComponent<BeatmapScript>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<UIDocument>().rootVisualElement;
        screenHeight = GameObject.Find("BeatSpawnUI").GetComponent<BeatUI>().screenHeight;
        deleteTime = beatManager.deleteDelay;
    }

    void Start()
    {
        
        
    }

    void OnEnable() {        
        Lane1L = beatUI.Q<VisualElement>("Lane1L");
        Lane1R = beatUI.Q<VisualElement>("Lane1R");
        Lane2L = beatUI.Q<VisualElement>("Lane2L");
        Lane2R = beatUI.Q<VisualElement>("Lane2R");
        Lane3L = beatUI.Q<VisualElement>("Lane3L");
        Lane3R = beatUI.Q<VisualElement>("Lane3R");
        Lanes = new VisualElement[] {Lane1L, Lane1R, Lane2L, Lane2R, Lane3L, Lane3R};
        beatTargetLocation = beatmapScript.beatTargetLocation;
        
    }

    public void Startup(bool left, int drumNo, string type) {
        if (start == false) {
            beatSpawnContainer = beatSpawnTemplate.Instantiate();
            container = new VisualElement();    //Create seperate container for the beat icon so that we can set absolute height, 
            container.Add(beatSpawnContainer);  //Container then goes into lane so beat inherits center position of lane.
            container.style.position = Position.Absolute;
            container.style.top = new Length(Mathf.RoundToInt(-(screenHeight*beatTargetLocation)));
            element = beatSpawnContainer.Q<VisualElement>("beat");

           // beatHeight = type == "rising" ? beatTargetLocation : beatHeight;
            //moveSpeed = type == "rising" ? -1f : 1f;

            //Debug.Log("DrumNo: " + drumNo);
            
            if(left == true) {
                Lanes[drumNo].Add(container);
            }
            else {
                Lanes[drumNo].Add(container);
            }

            start = true;
        }
    }

    //Checks if beat needs to be deleted each frame and calculates score for hitting beat at this time.
    void Update()
    {
        if(start == false) {

        }
        else
        {
            timer += Time.deltaTime;
            beatHeight += ((screenHeight * Time.deltaTime) / beatmapScript.delay) * moveSpeed;
            windowtime = beatmapScript.windowtime;
            beatSpawnContainer.style.top = new StyleLength(Mathf.RoundToInt(beatHeight));

            if (timer > (beatmapScript.delay + (windowtime/2)) && dontDelete == false)
            {
                delete = true;
                audioManager.FadeOutDrumTrack(0);
            }

            else if (timer >= (beatmapScript.delay - windowtime/2))
            {
                window = true;
                windowScore = Mathf.Abs(timer - beatmapScript.delay);
            }

            else if (timer < (beatmapScript.delay - (windowtime/2))) 
            {
                window = false;
            }

            if (fade == true) {
                moveSpeed = moveSpeed*0.1f;
                StartCoroutine(FadeCoroutine());
            }
        }   
    }

    IEnumerator FadeCoroutine() {
        float startF = 1f;
        float endF = 0f;
        float startW = beatSpawnContainer.resolvedStyle.width;
        float startH = beatSpawnContainer.resolvedStyle.height;
        float endW = startW*2;
        float endH = startH*2;
        float cTimer = 0f;
        float incrementF = (endF - startF)/deleteTime;
        float incrementW = (endW - startW)/deleteTime;
        float incrementH = (endH - startH)/deleteTime;
        long intervalInTicks = (long)(0.05 * TimeSpan.TicksPerSecond);
        Color color = Color.red;
        if (highlight) {
            color = Color.green;
        }

        element.schedule.Execute(() =>
        {
            element.style.unityBackgroundImageTintColor = color;
        }).StartingIn(0).Every(intervalInTicks).Until(() => CancelAnimation());


        while (cTimer < deleteTime) {
            cTimer += Time.deltaTime;
            float newOpacity = startF+(incrementF*cTimer);
            float newWidth = startW+(incrementW*cTimer);
            float newHeight = startH+(incrementH*cTimer);
            
            beatSpawnContainer.style.opacity = newOpacity;
            element.style.width = new StyleLength(newWidth);
            element.style.height = new StyleLength(newHeight);
            yield return null;
        }
    }
    
    private bool CancelAnimation() {
        return Time.time >= deleteTime;
    }
}
