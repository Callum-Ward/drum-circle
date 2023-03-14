using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LaneSpawn : MonoBehaviour
{

    UIDocument laneSpawnUI;
    public VisualTreeAsset laneSpawnTemplate;

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
