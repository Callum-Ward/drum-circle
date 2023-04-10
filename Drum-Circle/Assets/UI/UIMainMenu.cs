using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
   private VisualElement mainMenu;

   public void Awake() {
        mainMenu = GameObject.Find("UIMainMenu").GetComponent<UIDocument>().rootVisualElement;
   }

   public void startGame() {
    SceneManager.LoadScene("2MissionSelect");
   }

   private void OnButtonClick() {
    startGame();
   }

   public void Update() {
    VisualElement startButton = mainMenu.Q<VisualElement>("StartButton");
    if (Input.GetKey(KeyCode.Alpha1)) {
        startGame();
    }
    startButton.RegisterCallback<ClickEvent>(evt => OnButtonClick());
   }
}
