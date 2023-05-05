using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    VisualElement loadScreen;
    VisualElement loader;
    bool fadeIn = false;
    bool fadeOut = false;
    bool load = false;
    float timer = 0;

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
        if(fadeIn == true && fadeOut == false) {            
            float value = loader.resolvedStyle.opacity;
            value += Time.deltaTime;            
            Debug.Log("FADE VAL: "+ value);
            loader.style.opacity = new StyleFloat(value);
            if(value >= 1) {
                fadeIn = false;
            }
        }
        if(fadeOut == true && fadeIn == false) {            
            float value = loader.resolvedStyle.opacity;
            value -= Time.deltaTime;            
            Debug.Log("FADE VAL: "+ value);
            loader.style.opacity = new StyleFloat(value);
            if(value <= 0) {
                fadeOut = false;
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
}
