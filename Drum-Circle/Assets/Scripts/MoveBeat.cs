using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveBeat : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float timer = 0f;
    private float windowtime = 0f;
    public bool window = false;
    public bool dontDelete = false;
    public bool delete = false;
    public float windowScore = 0f;
    private float alpha = 1.0f;
    public bool fade = false;
    public bool highlight = false;
    public bool test = false;

    public ScoreManager scoreManager;
    public BeatManager beatManager;
    public BeatmapScript beatmapScript;
    public AudioManager audioManager;

    VisualElement beatUI;
    UIDocument beatSpawnUI;
    MeshRenderer beatRenderer;

    TemplateContainer beatSpawnContainer;
    VisualTreeAsset beatSpawnTemplate;

    private Vector3 baseScale;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatmapScript = GameObject.Find("Rhythm Logic").GetComponent<BeatmapScript>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        beatUI = GameObject.Find("BeatSpawnUI").GetComponent<UIDocument>().rootVisualElement;
        baseScale = gameObject.transform.localScale;
    }

    void Start()
    {
    }

    void OnEnable() {        
        beatSpawnUI = GetComponent<UIDocument>();

        TemplateContainer beatSpawnContainer = beatSpawnTemplate.Instantiate();
        beatSpawnUI.rootVisualElement.Q("Lane1L").Add(beatSpawnContainer);
    }

    //Checks if beat needs to be deleted each frame and calculates score for hitting beat at this time.
    void Update()
    {
        // transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        timer += Time.deltaTime;
        windowtime = beatmapScript.windowtime;

        if (timer > (beatmapScript.delay + (beatmapScript.windowtime)) && dontDelete == false)
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
            moveSpeed = 0.2f;
            alpha -= (1f / beatManager.deleteDelay) * Time.deltaTime;
            BeatHighlight();
        }
    }

//Function for changing colour of beat and activating alpha manipuation for fading.
public void BeatHighlight()
    {
        Color color = Color.grey;
        if (highlight)
        {
            color = Color.green;
        }
        beatRenderer = gameObject.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Standard"));
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

        gameObject.transform.localScale += baseScale * 0.05f;

        beatRenderer.material = newMaterial;
    }
}
