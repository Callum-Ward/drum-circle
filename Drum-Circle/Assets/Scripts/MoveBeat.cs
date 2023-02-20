using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBeat : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float timer = 0f;
    private float windowtime = 0f;
    public bool window = false;
    public bool delete = false;
    public float windowScore = 0f;
    private float alpha = 1.0f;
    public bool fade = false;
    public bool highlight = false;

    public ScoreManager scoreManager;
    public BeatManager beatManager;
    public BeatmapScript beatmapScript;
    public AudioManager audioManager;
    MeshRenderer beatRenderer;

    void Awake()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        beatManager = GameObject.Find("BeatManager").GetComponent<BeatManager>();
        beatmapScript = GameObject.Find("Rhythm Logic").GetComponent<BeatmapScript>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        timer += Time.deltaTime;
        windowtime = beatmapScript.windowtime;

        if (timer > (beatmapScript.delay + (beatmapScript.windowtime)))
        {
            delete = true;
        }

        else if (timer >= (beatmapScript.delay - windowtime/3))
        {
            window = true;
            windowScore = Mathf.Abs(timer - beatmapScript.delay);
        }

        else if (timer < (beatmapScript.delay - (windowtime/3))) 
        {
            window = false;
        }
        if (fade == true) {
            moveSpeed = 0.2f;
            alpha -= (1f / beatManager.deleteDelay) * Time.deltaTime;
            BeatHighlight();
        }
    }

public void BeatHighlight()
    {
        Color color = Color.grey;
        if (highlight)
        {
            color = Color.yellow;
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

        //gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.00f);
        gameObject.transform.localScale *= 1.005f;

        beatRenderer.material = newMaterial;
    }
}
