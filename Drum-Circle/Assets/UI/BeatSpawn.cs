using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BeatSpawn : MonoBehaviour
{

    UIDocument beatSpawnUI;
    public VisualTreeAsset beatSpawnTemplate;

    // Start is called before the first frame update
    void OnEnable()
    {
        beatSpawnUI = GetComponent<UIDocument>();

        TemplateContainer beatSpawnContainer = beatSpawnTemplate.Instantiate();
        beatSpawnUI.rootVisualElement.Q("Lane").Add(beatSpawnContainer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
