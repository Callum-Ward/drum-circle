using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UISelectMenu : MonoBehaviour
{    
   private VisualElement selectMenu;
   private MessageListener messageListener;

   public void Awake() {
        selectMenu = GameObject.Find("UIMissionSelect").GetComponent<UIDocument>().rootVisualElement;
        messageListener = GameObject.Find("SerialController").GetComponent<MessageListener>();
   }

   public void missionChoice(string mission) {
        SceneManager.LoadScene(mission);
   }

   public void Update() {
    VisualElement forestButton = selectMenu.Q<VisualElement>("ForestButton");
    VisualElement mountainButton = selectMenu.Q<VisualElement>("MountainButton");
    VisualElement beachButton = selectMenu.Q<VisualElement>("BeachButton");

    if (Input.GetKey(KeyCode.Alpha1)) {
        missionChoice("Forest");
    }
    if (Input.GetKey(KeyCode.Alpha2)) {
        missionChoice("Mountains");
    }
    if (Input.GetKey(KeyCode.Alpha3)) {
        missionChoice("Beach");
    }
    
    forestButton.RegisterCallback<ClickEvent>(evt => missionChoice("Forest"));
    mountainButton.RegisterCallback<ClickEvent>(evt => missionChoice("Mountains"));
    beachButton.RegisterCallback<ClickEvent>(evt => missionChoice("Beach"));
   }
}
