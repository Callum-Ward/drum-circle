using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Lane : MonoBehaviour
{
    
    public VisualTreeAsset laneSpawnTemplate;
    UIDocument laneSpawnUI;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        laneSpawnUI = GetComponent<UIDocument>();

        TemplateContainer laneSpawnContainer = laneSpawnTemplate.Instantiate();
        laneSpawnUI.rootVisualElement.Q("Lane").Add(laneSpawnContainer);       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
