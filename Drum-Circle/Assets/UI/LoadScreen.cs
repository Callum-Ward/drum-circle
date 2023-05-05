using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    VisualElement loadScreen;
    VisualElement loader;
    VisualElement endScore;
    VisualElement ending;
    VisualElement beatSpawnUI;
    bool fadeIn = false;
    bool fadeOut = false;
    bool load = false;
    float timer = 0;

    bool endIn = false;
    bool endout = false;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        loadScreen = GameObject.Find("LoadScreen").GetComponent<UIDocument>().rootVisualElement;
        loader = loadScreen.Q<VisualElement>("Loader");
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeIn == true) {            
            float value = loader.resolvedStyle.opacity;
            value += Time.deltaTime;            
            loader.style.opacity = new StyleFloat(value);
            if(value >= 1) {
                fadeIn = false;
            }
        }
        if(fadeOut == true && fadeIn == false) {            
            float value = loader.resolvedStyle.opacity;
            value -= Time.deltaTime;          
            loader.style.opacity = new StyleFloat(value);
            if(value <= 0) {
                fadeOut = false;
            }
        }
        if(endIn == true) { 
            endScore = GameObject.Find("EndScore").GetComponent<UIDocument>().rootVisualElement;
            ending = endScore.Q<VisualElement>("Canvas");
            beatSpawnUI = GameObject.Find("BeatSpawnUI").GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ScreenContainer");
            float value = ending.resolvedStyle.opacity;
            value += Time.deltaTime;           
            ending.style.opacity = new StyleFloat(value);
            beatSpawnUI.style.opacity = new StyleFloat(1f - value);
            if(value >= 1) {
                endIn = false;
            }
        }
    }

    public void LoadScreenFadeIn()
    {
        fadeIn = true;
    }

    public void LoadScreenFadeOut() 
    {
        fadeOut = true;
    }

    public void EndScreenFade() {
        endIn = true;
    }
}
