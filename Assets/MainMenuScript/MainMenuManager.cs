using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    [SerializeField] GameObject tutorial_image;
    public void LoadGame() {
        SceneManager.LoadScene("OverworldScene", LoadSceneMode.Single);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Tutorial() {
        tutorial_image.SetActive(!tutorial_image.activeSelf);
    }
}
