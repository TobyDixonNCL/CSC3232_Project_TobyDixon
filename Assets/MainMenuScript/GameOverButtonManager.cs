using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButtonManager : MonoBehaviour{
    public void MainMenu() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        PlayerStatTracker.instance.RevertDefaultValues();
    }
}
